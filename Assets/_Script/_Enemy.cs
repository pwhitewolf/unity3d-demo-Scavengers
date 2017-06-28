using UnityEngine;
using System.Collections;
using System;

public class _Enemy : _MovingObject {

    public int playerDamage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;      //玩家位置
    private bool skipMove;      //每回合移动一次
    
	// Use this for initialization
	protected override void Start ()
    {
        _GameManager.instance.AddEnemyToList(this);      //enemy添加进List
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}

    protected override void AttempMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttempMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)       //先判断enemy与player是否在同一列再决定x轴与y轴移动
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttempMove<_Player>(xDir, yDir);
        
    }

    protected override void OnCantMove<T>(T component)
    {
        if (component is _Player)
        {
            _Player hitplayer = component as _Player;

            hitplayer.LoseFood(playerDamage);
            animator.SetTrigger("enemyAttack");
            _SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        }
    }

    
}
