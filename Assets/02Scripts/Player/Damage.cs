using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    // Start is called before the first frame update

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;


    private const string bulletTag = "Bullet";
    private const string enemyTag = "ENEMY";
    private float initHp = 100.0f;
    public float currHp;
    public Image bloodScreen;
    public Image hpBar;

    private readonly Color initColor = new Vector4(0, 1.0f, 0.0f, 1.0f);
    private Color currColor;


    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    private void UpdateSetup()
    {
        initHp = GameManager.instance.gameData.hp;
        currHp += GameManager.instance.gameData.hp - currHp;
    }

    void Start()
    {
        initHp = GameManager.instance.gameData.hp;
        currHp = initHp;
        hpBar.color = initColor;
        currColor = initColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == bulletTag)
        {
            Destroy(other.gameObject);
            StartCoroutine(ShowBloodScreen());

            currHp -= 5.0f;
            DisplayHpbar();

            if(currHp<=0.0f)
            {
                playerDie();
            }
        }
    }

    private void playerDie()
    {

        //Debug.Log("Dead");
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        //foreach(var enemy in enemies)
        //{
        //    enemy.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
        OnPlayerDie();
        GameManager.instance.isGameOver = true;
    }

    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0, UnityEngine.Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        bloodScreen.color = Color.clear;
    }

    void DisplayHpbar()
    {
        if((currHp / initHp >0.5f))
        {
            currColor.r = (1 - (currHp / initHp)) * 2.0f;
        }
        else
        {
            currColor.g = (currHp / initHp) * 2.0f;
        }
        hpBar.color = currColor;
        hpBar.fillAmount = currHp / initHp;
    }
}
