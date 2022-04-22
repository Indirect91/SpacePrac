using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    private const string bulletTag = "Bullet";
    [SerializeField]
    private float hp = 100.0f;
    private float initHp = 100.0f;

    private GameObject bloodEffect;

    public GameObject hpBarPrefab;
    public Vector3 hpbarOffset = new Vector3(0, 2.2f, 0);
    private Canvas uiCanvas;
    private Image hpBarImage;

    // Start is called before the first frame update
    void Start()
    {
        bloodEffect = Resources.Load<GameObject>("BulletImpactFleshBigEffect");
        setHpBar();
    }

    void setHpBar()
    {
        uiCanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        GameObject hpBar = Instantiate<GameObject>(hpBarPrefab, uiCanvas.transform);
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];
        var _hpBar = hpBar.GetComponent<EnemyHpBar>();
        _hpBar.targetTr = this.gameObject.transform;
        _hpBar.offset = hpbarOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == bulletTag)
        {
            ShowBloodEffect(collision);
            
            hp -= collision.gameObject.GetComponent<bulletCtrl>().damage;
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            hpBarImage.fillAmount = hp / initHp;

            if(hp<=0.0f)
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;

                GameManager.instance.IncKillCount();
                GetComponent<CapsuleCollider>().enabled = false;

            }
        }
    }




    // Update is called once per frame
    void Update()
    {
        
    }
    void ShowBloodEffect(Collision coll)
    {
        Vector3 pos = coll.contacts[0].point;
        Vector3 _normal = coll.contacts[0].normal;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }

}
