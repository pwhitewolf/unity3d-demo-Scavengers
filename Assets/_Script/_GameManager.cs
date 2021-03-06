﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class _GameManager : MonoBehaviour {

    //设置单例
    public static _GameManager instance = null;   //static表示它属于这个类本身，并作为类的实例。即我们可从其他任何脚本访问它的公有部分

    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;      //回合间隔
    public _BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playerTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<_Enemy> enemies;   //获取enemy位置
    private bool enemiesMoving;
    private bool doingSetup;       //检测关卡是否在生成，避免主角在生成完毕前移动
    
    // Use this for initialization
	void Awake () {

        //设置单例，若已存在instance且不等于当前_GameManager则销毁
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //读取新场景不销毁对象
        DontDestroyOnLoad(gameObject);

        enemies = new List<_Enemy>();

        boardScript = GetComponent<_BoardManager>();
        InitGame();
	}

    private void OnLevelWasLoaded(int index)
    {
        level++;

        InitGame();
    }

    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " Days, you died.";
        levelImage.SetActive(true);
        enabled = false;
    }

	// Update is called once per frame
	void Update ()
    {
        if (playerTurn || enemiesMoving||doingSetup)          //对象移动时不被打断
            return;

        StartCoroutine(MoveEnemies());        //调用协程
	}

    public void AddEnemyToList(_Enemy script)      //在_GameManager中注册enemy
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()           //协程，定义enemy移动顺序
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if (enemies.Count==0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playerTurn = true;
        enemiesMoving = false;
    }
}
