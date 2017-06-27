using UnityEngine;
using System.Collections;
using System;

public class _Player : _MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    private Animator animator;
    private int food;


	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();
        food = _GameManager.instance.playerFoodPoints;

        base.Start();
	}

    private void OnDisable()
    {
        _GameManager.instance.playerFoodPoints = food;
    }



    // Update is called once per frame
    void Update ()
    {
        if (!_GameManager.instance.playerTurn)
            return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
            AttempMove<_Wall>(horizontal, vertical);      //player与wall交互


	}

    protected override void AttempMove<T>(int xDir, int yDir)
    {
        food--;

        base.AttempMove<T>(xDir, yDir);          //调用基函数并传递参数

        RaycastHit2D hit;

        CheckIfGameOver();

        _GameManager.instance.playerTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("ReStart", restartLevelDelay);  //安全更新界面+延时
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false);    //禁用已拾取的food
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        _Wall hitWall = component as _Wall;

        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");        //破坏墙壁动画
    }

    private void ReStart()
    {
        Application.LoadLevel(Application.loadedLevel);   //重置随机关卡。弃用？？
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            _GameManager.instance.GameOver();
        }
    }


}
