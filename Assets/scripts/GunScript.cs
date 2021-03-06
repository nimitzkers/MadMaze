﻿using UnityEngine;
using UnityEngine.Audio;

// defines the type of ammo
public enum AmmoType
{
    BLUE,
    RED,
    GREEN,
    WHITE,
    EMPTY
}

public class GunScript : MonoBehaviour {

    public GameObject newMat;
    public Texture green;
    public Texture white;
    public Texture blue;
    public Texture red;

    public float damage = 10f;
    public float range = 400f;
    public float impactForce = 30f;
    private int magazine = 0;
    public static AmmoType currAmmo = AmmoType.EMPTY;
    //public float fireRate = 15f;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public AudioClip empty;

    public static bool fireIsPressed = false;
    public static bool reloadIsPressed = false;


    // private float nextTimeToFire = 0f;

    // Update is called once per frame
    void Update()
    {

        float fireOVR = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
        float reloadOVR = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        // left mouse button used for shooting
        if (fireOVR > 0.0f || Input.GetButtonDown("Fire1")) // && Time.time >= nextTimeToFire
        {
            Debug.Log("fire");
            //nextTimeToFire = Time.time + 1f / fireRate;
            if (magazine > 0)
            {
                Shoot();
                magazine--;
            }
            else if (magazine == 0)
            {
                Debug.Log("it's empty");
                currAmmo = AmmoType.EMPTY;
                ShootEmpty();
                newMat.GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", white);
            }

            fireIsPressed = true;
        }

        // right mouse button used for reloading
        if (!reloadIsPressed && reloadOVR > 0.0f || Input.GetButtonDown("Fire2"))
        {
            RaycastHit hit;
            Debug.Log("reload");

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, 3f)) // need to be 5 units away to reload
            {
                //Debug.Log(hit.transform.name); // prints out to console
                Target target = hit.transform.GetComponent<Target>();
                Debug.Log(target);
                if (target != null) // checks for null pointer exception
                {
                    //Debug.Log("reloaded with red");
                    target.TakeDamage(30f);

                    if ((int)hit.rigidbody.mass == 499)
                    {
                        this.Reload(AmmoType.RED);
                        newMat.GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", red);
                    }
                    else if ((int)hit.rigidbody.mass == 999)
                    {
                        this.Reload(AmmoType.GREEN);
                        newMat.GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", green);
                    }
                    else if ((int)hit.rigidbody.mass == 1500)
                    {
                        this.Reload(AmmoType.BLUE);
                        newMat.GetComponent<MeshRenderer>().material.SetTexture("_EmissionMap", blue);
                    }
                }
            }

            reloadIsPressed = true;
        }

        if (reloadOVR == 0.0f)
        {

            reloadIsPressed = false;
        }

        if (fireOVR == 0.0f)
        {

            fireIsPressed = false;
        }
    }

    // reloads the magazine with 5 new bullets of defined type
    public void Reload(AmmoType newAmmo)
    {
        magazine = 5;
        currAmmo = newAmmo;
    }

    public void ShootEmpty()
    {
        Debug.Log("gun is empty");
    }


    public void Shoot ()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
    }
}
