using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player 클래스는 MovingObject의 상속을 받는다.
public class Player : MovingObject
{
    // 변수 선언
    // wallDamage는 플레이어가 벽에 부술 때 벽 오브젝트에 적용될 데미지
    public int wallDamage = 1;
    // 보드 위에 음식이나 소다를 집었을 때 오르는 점수
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;

    public float restartLevelDelay = 1f;

    // 애니메이터 컴포넌트의 래퍼런스를 가져오기 위해 사용되는 변수
    public Animator animator;

    //레벨을 변경하면서 스코어를 다시 게임매니저로 넣기 전에 해당 레벨 동안 플레이어 스코어를 저장할 변수
    public int food;

    // Start is called before the first frame update
    // 오버라이드
    protected override void Start()
    {
        // 애니메이터 컴포넌트의 래퍼런스를 가져온다.
        animator = GetComponent<Animator>();
        // 게임매니저의 playerFoodPoints를 지정하여 플레이 동안의 점수 관리+레벨 변경 시 게임매니저로 저장 가능
        food = GameManager.instance.playerFoodPoints;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어의 턴이 아니면 코드가 실행되지 않게 한다.
        if (!GameManager.instance.playerTurn)
            return;
        // 수평, 수직 변수를 선언 이들은 1이나 -1로 저장해서 사용
        int horizontal = 0;
        int vertical = 0;

        // 입력을 받는다.
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        // 대각선 이동을 방지
        if (horizontal != 0)
            wallDamage = vertical = 0;

        // 입력이 들어왔으면 움직일 수 있도록 한다.
        // Wall은 벽과의 상호작용을 위해 추가
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    }

    // MovingObject에서는 OnCantMove를 내부구현 없는 추상함수로 하용하였음
    // 이 함수는 플레이어가 이동하려는 공간에 벽이 있고, 이에 막히는 경우의 행동 작성
    protected override void OnCantMove<T>(T component)
    {
        // hitwall 변수를 component형으로 캐스팅
        Wall hitwall = component as Wall;
        // 우리가 타격한 벽이 얼마나 피해를 입었는지 알 기 위해 호출
        hitwall.DamageWall(wallDamage);
        // 애니메이터 컴포넌트 playerChop 트리거 호출
        animator.SetTrigger("playerChop");
    }

    // 레벨을 다시 로드하는 함수
    // 플레이어가 출구 오브젝트와 충돌하였을 때, Restart를 호출하여 다음 레벨로 넘어간다.
    // 보통 많은 게임들은 다른 레벨을 로드하기위해 다른 Scene을 호출한다.
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    // 적이 플레이어 공격 시 얼만큼의 점수를 잃게 될지 가리킴
    // 총 음식점수에서 loss만큼의 점수를 빼고, CheckIfGameOver를 호출해
    // 게임 오버 여부를 확인
    public void LoadFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }

    // 플레이어가 출구, 소다, 음식 오브젝트 같은 보드 위의 오브젝트들과 상호작용 할 수 있도록 하는 함수
    // 유니티API에 포함된 OnTriggerEnter2D를 사용한다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(other.tag == "Food")
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
        }
        else if(other.tag == "Soda")
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
        }
    }

    // 게임오브젝트가 비활성화되는 순간(레벨이 바뀔 때) 게임 매니저에 food값을 저장
    // 유니티 API에 속해있는 함수이다.
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    // 음식 점수가 0 이하이면 게임 오버시키는 함수
    private void CheckIfGameOver()
    {
        if (food == 0)
            GameManager.instance.GameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // 움직일 떄 마다 음식 점수 -1
        food--;
        // AttempMOve
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        CheckIfGameOver();
        GameManager.instance.playerTurn = false;
    }
}
