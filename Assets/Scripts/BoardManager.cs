using System.Collections;
using System.Collections.Generic;   //List의 사용 가능
using System;   //직렬화 사용
using Random = UnityEngine.Random;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count(int min, int max)
        {
            this.minimum = min;
            this.maximum = max;
        }
    }

    // 게임 보드의 공간을 위한 변수 선언
    public int columns = 8; //열
    public int rows = 8;    //행
    // 레벨마다 얼마나 많은 벽을 랜덤하게 생성할지 범위를 특정
    public Count wallCount = new Count(5, 9);   //아래 코드는 최소 5~최대 9개의 벽이 존재
    public Count foodCount = new Count(1, 5);   //최소 1~ 최대 5개의 음식 존재
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    void InitializeList()
    {
        //기존 벡터 초기화
        gridPositions.Clear();

        //외벽제외 가장자리를 빼고 벡터로 채움
        for(int x = 1; x < columns - 1; x++)
        {
            for(int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for(int x = -1; x < columns + 1; x++)
        {
            for(int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // 랜덤 위치에 오브젝트 생성
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) 
    {
        // 오브젝트 위치 선택
        int objectCount = Random.Range(minimum, maximum + 1);

        for(int i=0; i<objectCount; i++)
        {
            // 랜덤 포지션 위치를 가져온다.
            Vector3 randomPosition = RandomPosition();
            // 타일을 고른다.
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            // 고른 타일을 인스턴스화한다.
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetUpScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);
    }
}
