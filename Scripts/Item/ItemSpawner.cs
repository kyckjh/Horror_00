using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    List<Transform> spawnPoint;

    

    // ���̺�, �ε� �� ������ ��ġ ��������� ����� ������ ������� ��ųʸ�
    Dictionary<ItemIDCode, List<Transform>> ItemSpawnPosition = new Dictionary<ItemIDCode, List<Transform>>();

    private void Awake()
    {
        spawnPoint = new List<Transform>(transform.childCount);
    }

    private void Start()
    {
        // spawnPoint List�� ���� ���� �迭
        int[] suffleArray = new int[transform.childCount];
        // �迭�� �ε��� �ֱ�
        for(int i = 0; i < transform.childCount; i++)
        {
            suffleArray[i] = i;
        }
        // �迭 ����
        for(int i = 0; i < transform.childCount; i++)
        {
            int size = transform.childCount - 1 - i;
            int randIndex = Random.Range(0, size - 1);
            (suffleArray[size], suffleArray[randIndex]) = (suffleArray[randIndex], suffleArray[size]);
        }
        // ���� �迭�� �ε��� ������� List�� �߰��ϱ�
        for(int i = 0; i < transform.childCount; i++)
        {
            spawnPoint.Add(transform.GetChild(suffleArray[i]));
        }

        // ������ �����ϰ� ���� ��ġ Dictionary�� �����ϱ�
        SpawnItem(ItemIDCode.Key);
        SpawnItem(ItemIDCode.Fuse);
        SpawnItem(ItemIDCode.Fuse);
        SpawnItem(ItemIDCode.Fuse);
        SpawnItem(ItemIDCode.USB);
        SpawnItem(ItemIDCode.CardKey);
    }

    // �������� �����ϴ� �Լ�
    void SpawnItem(ItemIDCode code)
    {
        if (spawnPoint.Count > 0)
        {
            // ������ ����
            ItemFactory.MakeItem(code, spawnPoint[0]);
            if(ItemSpawnPosition.ContainsKey(code) == false)
            {
                // ������ ������� ��ųʸ��� �ش� Key(ItemIDCode)�� ���� �߰����� �ʾ��� ��� ���� �߰��ϸ鼭 Data�� List ���� ����
                ItemSpawnPosition.Add(code, new List<Transform>());
            }
            // ��ųʸ��� Key�� �ش��ϴ� Data List�� ������ġ Transform �߰�
            ItemSpawnPosition[code].Add(spawnPoint[0]);
            // �̹� �߰��� spawnPoint�� List���� ����
            spawnPoint.RemoveAt(0);
        }
        else
        {
            Debug.LogError("�������� ������ spawnPoint�� �����մϴ�.");
        }
    }
}
