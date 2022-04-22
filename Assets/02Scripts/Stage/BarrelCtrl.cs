using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;
    private int hitCount = 0;

    private Rigidbody rb;
    public Mesh[] meshes;
    private MeshFilter meshFilter;
    public Texture[] textures;
    private MeshRenderer _renderer;
    public float expRadius = 10.0f;
    private AudioSource _audio;
    public AudioClip exSfx;
    private Shake shake;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        rb = GetComponent<Rigidbody>();
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.mainTexture = textures[UnityEngine.Random.Range(0, textures.Length)];
        _audio = GetComponent<AudioSource>();
        //shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        StartCoroutine(GetShake());
    }

    IEnumerator GetShake()
    {
        while(!UnityEngine.SceneManagement.SceneManager.GetSceneByName("Play").isLoaded)
        {
            yield return null;
        }
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="Bullet")
        {
            if(++hitCount==3)
            {
                ExpBarrel();
            }
        }
    }

    private void ExpBarrel()
    {
        var effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);
        //rb.mass = 1.0f;
        //rb.AddForce(Vector3.up * 1000.0f);

        IndirectDamage(transform.position);

        int rdNum = UnityEngine.Random.Range(0, meshes.Length);
        meshFilter.sharedMesh = meshes[rdNum];
        _audio.PlayOneShot(exSfx, 1.0f);
        StartCoroutine(shake.ShakeCamera());
    }

    private void IndirectDamage(Vector3 position)
    {
        Collider[] colls = Physics.OverlapSphere(position, expRadius, 1 << 8);
        foreach(var coll in colls)
        {
            var _rb = coll.GetComponent<Rigidbody>();
            _rb.mass = 1.0f;
            _rb.AddExplosionForce(1200.0f, position, expRadius, 1000.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
