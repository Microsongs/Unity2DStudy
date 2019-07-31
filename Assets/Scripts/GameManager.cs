using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton 변수
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    // HideInspector는 변수가 public이지만 에디터에서 숨겨줌
    [HideInInspector] public bool playersTurn = true;
    // turnDelay선언 -> 턴을 얼마나 대기할 것인가
    public float turnDelay = .1f;

    private int level = 4;  // Level이 3인 이유는 log2로 되어있기 때문
    private List<Enemy> enemies;    // 적들의 위치를 계속 추적하고 이동
    private bool enemiesMoving;

    // Start is called before the first frame update
    void Awake()
    {
        // Singleton 체크
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }
    
    void InitGame()
    {
        // 게임 시작 시 적 리스트를 초기화
        enemies.Clear();
        boardScript.SetUpScene(level);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnDelay);

        for(int i=0; i<enemies.Count; i++)
        {
            enemies[i].moveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    // 게임 매니저를 비활성화시킬 곳인 GameOver 함수
    public void GameOver()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
}
