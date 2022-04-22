using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    AudioSource audio;
    Animator animator;
    Transform enemyTr;
    Transform playerTr;
    readonly int hashFire = Animator.StringToHash("Fire");
    readonly int hashReload = Animator.StringToHash("Reload");

    float nextFire = 0.0f;
    readonly float fireRate = 0.1f;
    readonly float damping = 10.0f;

    readonly float reloadTime = 2.0f;
    readonly int maxBullet = 10;
    private int currBullet = 10;
    private bool isReload = false;
    private WaitForSeconds wsReload;

    public bool isFire = false;
    public AudioClip fireSfx;
    public AudioClip reloadSfx;
    public GameObject Bullet;
    public Transform firePos;
    public MeshRenderer muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;
        enemyTr = GetComponent<Transform>();
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        wsReload = new WaitForSeconds(reloadTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(isFire && !isReload)
        {
            if(Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate + UnityEngine.Random.Range(0.0f, 0.3f);
            }

            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);

        }
    }

    private void Fire()
    {
        animator.SetTrigger(hashFire);
        audio.PlayOneShot(fireSfx, 1.0f);
        StartCoroutine(ShowMuzzleFlash());
        GameObject _bullet = Instantiate(Bullet, firePos.position, firePos.rotation);
        Destroy(_bullet, 3.0f);

        isReload = (--currBullet % maxBullet == 0);
        if(isReload)
        {
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        muzzleFlash.enabled = false;
        animator.SetTrigger(hashReload);
        audio.PlayOneShot(reloadSfx, 1.0f);

        yield return wsReload;


        currBullet = maxBullet;
        isReload = false;
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled = true;
        Quaternion rot = Quaternion.Euler(Vector3.forward * UnityEngine.Random.Range(0, 360));

        muzzleFlash.transform.localRotation = rot;
        muzzleFlash.transform.localScale = Vector3.one * UnityEngine.Random.Range(1.0f, 2.0f);
        Vector2 offset = new Vector2(UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 2)) * 0.5f;

        muzzleFlash.material.SetTextureOffset("_MainTex", offset);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.2f));
        muzzleFlash.enabled = false;

    }
}
