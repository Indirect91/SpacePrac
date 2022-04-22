using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject sparkEffect;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag=="Bullet")
        {
            ShowEffect(collision);
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            
        }
    }

    private void ShowEffect(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        var spark = Instantiate(sparkEffect, contact.point, rot);
        spark.transform.SetParent(this.transform);
    }
}
