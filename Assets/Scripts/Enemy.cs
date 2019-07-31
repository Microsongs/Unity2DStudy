using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    // 변수 선언

    // 적이 공격할 때 뺄셈할 음식 포인트
    public int playerDamage;
    // 애니메이터 변수
    private Animator animator;
    // 플레이어의 위치 저장 + 적의 이동할 곳을 알려줌
    private Transform target;
    // 적이 턴마다 움직이게 하는 데 사용
    private bool skipMove;
    
    // Start is called before the first frame update
    // 플레이어를 찾아 공격할 수 있는 기능을 추가하여 오버라이드
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    // 1턴에 1번만 움직일 수 있도록 하기 위해 AttemptMove를 재정의
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }
    public void moveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyAttack");

        hitPlayer.LoseFood(playerDamage);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
