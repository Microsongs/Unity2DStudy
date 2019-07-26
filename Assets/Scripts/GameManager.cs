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
    [HideInInspector] public bool playerTurn = true;

    private int level = 4;  // Level이 3인 이유는 log2로 되어있기 때문

    // Start is called before the first frame update
    void Awake()
    {
        // Singleton 체크
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }
    
    void InitGame()
    {
        boardScript.SetUpScene(level);
    }

    // 게임 매니저를 비활성화시킬 곳인 GameOver 함수
    public void GameOver()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
