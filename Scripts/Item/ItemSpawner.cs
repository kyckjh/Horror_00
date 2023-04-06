using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    List<Transform> spawnPoint;

    

    // 세이브, 로드 시 아이템 위치 저장용으로 사용할 아이템 생성목록 딕셔너리
    Dictionary<ItemIDCode, List<Transform>> ItemSpawnPosition = new Dictionary<ItemIDCode, List<Transform>>();

    private void Awake()
    {
        spawnPoint = new List<Transform>(transform.childCount);
    }

    private void Start()
    {
        // spawnPoint List를 섞기 위한 배열
        int[] suffleArray = new int[transform.childCount];
        // 배열에 인덱스 넣기
        for(int i = 0; i < transform.childCount; i++)
        {
            suffleArray[i] = i;
        }
        // 배열 섞기
        for(int i = 0; i < transform.childCount; i++)
        {
            int size = transform.childCount - 1 - i;
            int randIndex = Random.Range(0, size - 1);
            (suffleArray[size], suffleArray[randIndex]) = (suffleArray[randIndex], suffleArray[size]);
        }
        // 섞인 배열의 인덱스 순서대로 List에 추가하기
        for(int i = 0; i < transform.childCount; i++)
        {
            spawnPoint.Add(transform.GetChild(suffleArray[i]));
        }

        // 아이템 생성하고 생성 위치 Dictionary에 저장하기
        SpawnItem(ItemIDCode.Key);
        SpawnItem(ItemIDCode.Fuse);
        SpawnItem(ItemIDCode.Fuse);
        SpawnItem(ItemIDCode.Fuse);
        SpawnItem(ItemIDCode.USB);
        SpawnItem(ItemIDCode.CardKey);
    }

    // 아이템을 스폰하는 함수
    void SpawnItem(ItemIDCode code)
    {
        if (spawnPoint.Count > 0)
        {
            // 아이템 생성
            ItemFactory.MakeItem(code, spawnPoint[0]);
            if(ItemSpawnPosition.ContainsKey(code) == false)
            {
                // 아이템 생성목록 딕셔너리에 해당 Key(ItemIDCode)가 아직 추가되지 않았을 경우 새로 추가하면서 Data에 List 새로 생성
                ItemSpawnPosition.Add(code, new List<Transform>());
            }
            // 딕셔너리의 Key에 해당하는 Data List에 생성위치 Transform 추가
            ItemSpawnPosition[code].Add(spawnPoint[0]);
            // 이미 추가된 spawnPoint는 List에서 제거
            spawnPoint.RemoveAt(0);
        }
        else
        {
            Debug.LogError("아이템을 스폰할 spawnPoint가 부족합니다.");
        }
    }
}
