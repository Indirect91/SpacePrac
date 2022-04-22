using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletCtrl : MonoBehaviour
{
    public float damage = 20.0f;
    public float speed = 500.0f;
    private Transform tr;
    private Rigidbody rb;
    private TrailRenderer trail;



    // Start is called before the first frame update
    void Start()
    {
    //    GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

    

    private void UpdateSetup()
    {
        damage = GameManager.instance.gameData.damage;
    }


    private void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        damage = GameManager.instance.gameData.damage;
    }


    private void OnEnable()
    {
        rb.AddForce(transform.forward * speed);
        GameManager.OnItemChange += UpdateSetup;
    }

    private void OnDisable()
    {
        trail.Clear();
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
