using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    // gameManager변수
    public GameObject gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        //게임매니저의 인스턴스가 null일 경우에 게임 매니저 프리팹을 인스턴스화
        if (GameManager.instance == null)
            Instantiate(gameManager);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
