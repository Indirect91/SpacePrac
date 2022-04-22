using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}



public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;
    public float moveSpeed = 10;
    public float rotSpeed = 80;

    private Transform tr;

    public PlayerAnim playerAnim;
    public Animation anim;

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    private void UpdateSetup()
    {
        moveSpeed = GameManager.instance.gameData.speed;
    }



    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        anim.clip = playerAnim.idle;
        anim.Play();
        moveSpeed = GameManager.instance.gameData.speed;
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        //transform.Rotate(0, leftSpeed, 0);
        //thisBody.mass = 30;

        v = Input.GetAxis("Vertical");
        // transform.Translate(0, 0 , moveSpeed,Space.Self);

        r = Input.GetAxis("Mouse X");

        Vector3 movDir = Vector3.forward * v + Vector3.right * h;
        tr.Translate(movDir.normalized* moveSpeed * Time.deltaTime, Space.Self);
        tr.Rotate(Vector3.up, rotSpeed * Time.deltaTime*r);

        if(v>=0.1f)
        {
            anim.CrossFade(playerAnim.runF.name, 0.3f);
        }
        else if(v<=-0.1f)
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);
        }
        else 
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
        //if (Input.GetKey(KeyCode.RightArrow))
        //    //transform.Translate(Vector3.right);
        //    thisBody.AddForce(Vector3.right * 100);
        //if (Input.GetKey(KeyCode.LeftArrow))
        //    //            transform.Translate(Vector3.left);
        //    thisBody.AddForce(Vector3.left * 100);
        //if (Input.GetKey(KeyCode.UpArrow))
        //    //transform.Translate(Vector3.forward);
        //    thisBody.AddForce(Vector3.forward * 100);
        //if (Input.GetKey(KeyCode.DownArrow))
        //    thisBody.AddForce(Vector3.back*100);
        ////  transform.Translate(Vector3.back);

    }
}
