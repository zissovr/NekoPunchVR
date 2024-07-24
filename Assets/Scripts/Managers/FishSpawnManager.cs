using System.Collections;
using UnityEngine;

public class FishSpawnManager : MonoBehaviour
{
    [SerializeField]
    private Transform playArea;
    [SerializeField]
    private Vector2Int areaSplit;
    [SerializeField]
    private float fishDistance;
    [SerializeField]
    private SpawnMode spawnMode;

    [Delayed]
    [SerializeField]
    private float forceAngle;
    //魚の種類
    public GameObject[] fishPrefabs;

    private int indexFish;


    //魚スポーンのタイミング
    public float timeRate;

    void Start()
    {
        if (spawnMode == SpawnMode.Front)
            StartCoroutine(CreateFishes());
        else
            StartCoroutine(AroundSpawn());
    }


    private IEnumerator AroundSpawn()
    {
        Vector2 range = spawnMode switch
        {
            SpawnMode.Front90 => new Vector2(-45, 45),
            SpawnMode.Front180 => new Vector2(-90, 90),
            SpawnMode.Around => new Vector2(-180, 180),
            _ => new Vector2(-30, 30),
        };

        while (true)
        {
            //ランダムな角度を取得
            float angle = Random.Range(range.x, range.y);


            if (forceAngle != 0)
            {
                angle = forceAngle;
            }
            var pos = GetSplitedPositionWithAngles(new Vector2Int(Random.Range(0, areaSplit.x), Random.Range(0, areaSplit.y)), angle);

            //playAreaを中心にangle度回転
            var spawnPos = playArea.position;
            spawnPos.z -= fishDistance;
            var spawnPosRotated = Quaternion.Euler(0, angle, 0) * (pos - spawnPos) + spawnPos;

            //Distance分前に移動
            spawnPosRotated.z += fishDistance;

            indexFish = Random.Range(0, 2);
            GameObject fish = Instantiate(fishPrefabs[indexFish], spawnPosRotated, Quaternion.identity);
            fish.transform.LookAt(pos);
            yield return new WaitForSeconds(timeRate);
        }
    }

    private IEnumerator CreateFishes()
    {
        while (true)
        {
            var pos = GetSplitedRandomPosition();
            pos.z += fishDistance;
            indexFish = Random.Range(0, 2);
            GameObject fish = Instantiate(fishPrefabs[indexFish], pos, Quaternion.Euler(0, -180, 0));
            fish.transform.SetParent(transform);
            yield return new WaitForSeconds(timeRate);
        }
    }

    /// <summary>
    /// プレイエリアを分割したランダムな位置を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSplitedRandomPosition()
    {
        //Splitsの中からランダムで選択
        var randIndex = new Vector2Int(Random.Range(0, areaSplit.x), Random.Range(0, areaSplit.y));
        return GetSplitedPosition(randIndex);
    }

    /// <summary>
    /// プレイエリアを分割した位置を取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Vector3 GetSplitedPosition(Vector2Int index)
    {
        var size = playArea.lossyScale;
        var areaPos = playArea.position;
        var startPos = new Vector3(areaPos.x - size.x / 2, areaPos.y - size.y / 2, areaPos.z);

        //PlayAreaの中での位置を計算
        startPos.x += size.x / (areaSplit.x - 1) * index.x;
        startPos.y += size.y / (areaSplit.y - 1) * index.y;
        return startPos;
    }

    /// <summary>
    /// プレイエリアを分割した位置を取得
    /// 回転対応版
    /// </summary>
    /// <param name="index"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    private Vector3 GetSplitedPositionWithAngles(Vector2Int index, float angle)
    {
        //Splitsの中からランダムで選択
        var areaPos = playArea.position;

        var startPos = GetSplitedPosition(index);
        //startPosをareaPosを中心にangle度回転
        return Quaternion.Euler(0, angle, 0) * (startPos - areaPos) + areaPos;


    }

    /// <summary>
    /// プレイエリア内のランダムな位置を取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPosition()
    {
        var size = playArea.lossyScale;
        var areaPos = playArea.position;

        //PlayAreaの中での位置を計算
        return new Vector3(areaPos.x + Random.Range(areaPos.x - (size.x / 2), areaPos.x + (size.x / 2)),
                           areaPos.y + Random.Range(areaPos.y - (size.y / 2), areaPos.x + (size.y / 2)),
                           areaPos.z);
    }

    public enum SpawnMode
    {
        Front,
        Front90,
        Front180,
        Around,
    }

}
