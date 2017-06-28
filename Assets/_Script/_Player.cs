using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class _Player : _MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;
    private Vector2 touchOrigin = -Vector2.one;


	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();
        food = _GameManager.instance.playerFoodPoints;

        foodText.text = "Food: " + food;

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

#if UNITY_EDITOR||UNITY_STANDALONE||UNITY_WEBPLAYER

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

#else

        if(Input.touchCount>0)
        {
            Touch myTouch = Input.touches[0];

            if (myTouch.phase == TouchPhase.Began)
                touchOrigin = myTouch.position;
            else if(myTouch.phase==TouchPhase.Ended&&touchOrigin.x>=0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                    horizontal = x > 0 ? 1 : -1;
                else
                    vertical = y > 0 ? 1 : -1;
            }
        }

#endif

        if (horizontal != 0 || vertical != 0)
            AttempMove<_Wall>(horizontal, vertical);      //player与wall交互


	}

    protected override void AttempMove<T>(int xDir, int yDir)
    {
        food--;

        foodText.text = "Food: " + food;

        base.AttempMove<T>(xDir, yDir);          //调用基函数并传递参数

        RaycastHit2D hit;

        if (Move(xDir, yDir, out hit))
            _SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);

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
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            _SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);    //禁用已拾取的food
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            _SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        if (component is _Wall)
        {
            _Wall hitWall = component as _Wall;

            hitWall.DamageWall(wallDamage);
            animator.SetTrigger("playerChop");  //破坏墙壁动画
        }
    }

    private void ReStart()
    {
        Application.LoadLevel(Application.loadedLevel);   //重置随机关卡。弃用？？
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            _SoundManager.instance.PlaySingle(gameOverSound);
            _SoundManager.instance.musicSource.Stop();
            _GameManager.instance.GameOver();
            
        }
    }


}
