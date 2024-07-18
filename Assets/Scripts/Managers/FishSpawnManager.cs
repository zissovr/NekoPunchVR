using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawnManager : MonoBehaviour
{
    //15箇所の出現場所
    public GameObject[] spawnPoints;

    //魚の種類
    public GameObject[] fishPrefabs;

    private int index;
    private int indexFish;

    //魚スポーンのタイミング
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
