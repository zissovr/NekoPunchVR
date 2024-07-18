using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawnManager : MonoBehaviour
{
    //15�ӏ��̏o���ꏊ
    public GameObject[] spawnPoints;

    //���̎��
    public GameObject[] fishPrefabs;

    private int index;
    private int indexFish;

    //���X�|�[���̃^�C�~���O
    public float timeRate;

    void Start()
    {
        StartCoroutine(CreateFishes());
    }


    private IEnumerator CreateFishes()
    {
        while (true)
        {
            index = Random.Range(0, 15);
            indexFish = Random.Range(0, 2);
            GameObject fish = Instantiate(fishPrefabs[indexFish], spawnPoints[index].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            fish.transform.SetParent(transform);
            yield return new WaitForSeconds(timeRate);
        }
    }
}
