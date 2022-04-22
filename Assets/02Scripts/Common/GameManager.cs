using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataInfo;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]

    public Transform[] points;
    public GameObject enemy;
    public float createTime = 2.0f;
    public int maxEnemy = 10;

    public bool isGameOver = false;

    public static GameManager instance = null;

    [Header("Object Pool")]
    public GameObject bulletPrefab;

    public int maxPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();
    private bool isPaused;
    public CanvasGroup inventoryCG;

    //[HideInInspector]
    //public int killCount;

    public Text killCountTxt;

    private DataManager dataManager;
    //public GameData gameData;
    public GameDataObject gameData;


    public delegate void ItemChangeDelegate();
    public static event ItemChangeDelegate OnItemChange;

    private GameObject slotList;
    public GameObject[] itemObjects;


    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else if(instance != this.gameObject)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        dataManager = GetComponent<DataManager>();
        dataManager.Initialize();


        slotList = inventoryCG.transform.Find("SlotList").gameObject;

        LoadGameData();

        CreatePooling();
    }

    void LoadGameData()
    {
        //killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        //killCountTxt.text = "KILL" + killCount.ToString("0000");
        //GameData data = dataManager.Load();
        //gameData.hp = data.hp;
        //gameData.damage = data.damage;
        //gameData.speed = data.speed;
        //gameData.killCount = data.killCount;
        //gameData.equipItem = data.equipItem;

        if(gameData.equipItem.Count>0)
        {
            inventorySetup();
        }
        killCountTxt.text = "KILL " + gameData.killCount.ToString("0000");

    }

    void inventorySetup()
    {
        var slots = slotList.GetComponentsInChildren<Transform>();
        for(int i = 0; i<gameData.equipItem.Count;i++)
        {
            for(int j = 1; j<slots.Length;j++)
            {
                if (slots[j].childCount > 0) continue;


                int itemIndex = (int)gameData.equipItem[i].itemType;
                itemObjects[itemIndex].GetComponent<Transform>().SetParent(slots[j]);
                itemObjects[itemIndex].GetComponent<ItemInfo>().itemData = gameData.equipItem[i];

                break;
            }
        }
    }

    void SaveGameData()
    {
        //dataManager.Save(gameData);
        UnityEditor.EditorUtility.SetDirty(gameData);
    }

    public void RemoveItem(Item item)
    {
        gameData.equipItem.Remove(item);
        switch (item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.hp -= item.value;
                }
                else
                {
                    gameData.hp = gameData.hp / (1.0f + item.value);
                }
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.damage += item.value;
                }
                else
                {
                    gameData.damage += gameData.damage / (1.0f + item.value);
                }
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.speed += item.value;
                }
                else
                {
                    gameData.speed += gameData.speed / (1.0f + item.value);
                }
                break;
            case Item.ItemType.GRENADE:
                break;

        }
        UnityEditor.EditorUtility.SetDirty(gameData);
        OnItemChange();

    }


    public void AddItem(Item item)
    {
        if (gameData.equipItem.Contains(item)) return;
        gameData.equipItem.Add(item);

        switch(item.itemType)
        {
            case Item.ItemType.HP:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.hp += item.value;
                }
                else
                {
                    gameData.hp += gameData.hp * item.value;
                }
                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.damage += item.value;
                }
                else
                {
                    gameData.damage += gameData.damage * item.value;
                }
                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.INC_VALUE)
                {
                    gameData.speed += item.value;
                }
                else
                {
                    gameData.speed += gameData.speed * item.value;
                }
                break;
            case Item.ItemType.GRENADE:
                break;

        }
        UnityEditor.EditorUtility.SetDirty(gameData);

        OnItemChange();

    }



    private void OnApplicationQuit()
    {
        SaveGameData();
    }


    // Start is called before the first frame update
    void Start()
    {
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        if(points.Length>0)
        {
            StartCoroutine(this.CreateEnemy());
        }
        inventoryCG = GameObject.Find("Inventory").GetComponent<CanvasGroup>();
        OnInventoryOpen(false);

    }

    public void OnInventoryOpen(bool isOpened)
    {
        inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;
    }

    public void OnPauseClick()
    {
        isPaused = !isPaused;
        Time.timeScale = (isPaused) ? 0.0f : 1.0f;
        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        var scrips = playerObj.GetComponents<MonoBehaviour>();
        foreach(var scrip in scrips)
        {
            scrip.enabled = !isPaused;
        }
        var canvasGroup = GameObject.Find("Panel - Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;

    }


    public void IncKillCount()
    {
        //++killCount;
        //killCountTxt.text = "KILL" + killCount.ToString("0000");
        //PlayerPrefs.SetInt("KILL_COUNT", killCount);

        ++gameData.killCount;
        killCountTxt.text = "KILL " + gameData.killCount.ToString("0000");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CreateEnemy()
    {
        while(!isGameOver)
        {
            int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;
            if(enemyCount<maxEnemy)
            {
                yield return new WaitForSeconds(createTime);
                int idx = UnityEngine.Random.Range(1, points.Length);
                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }

    public GameObject GetBullet()
    {
        for(int i = 0; i<bulletPool.Count; i++)
        {
            if(bulletPool[i].activeSelf ==false)
            {
                return bulletPool[i];
            }
        }
        return null;
    }


    public void CreatePooling()
    {
        GameObject objectPools = new GameObject("ObjectPools");
        for(int i = 0; i<maxPool;i++)
        {
            var obj = Instantiate<GameObject>(bulletPrefab, objectPools.transform);
            obj.name = "Bullet_" + i.ToString("00");
            obj.SetActive(false);
            bulletPool.Add(obj);
            
        }
        
    }
}
