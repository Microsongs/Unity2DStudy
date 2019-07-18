using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton 변수
    public static GameManager instance = null;

    public BoardManager boardScript;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
