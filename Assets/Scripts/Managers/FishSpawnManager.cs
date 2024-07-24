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
    //���̎��
    public GameObject[] fishPrefabs;

    private int indexFish;


    //���X�|�[���̃^�C�~���O
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
            //�����_���Ȋp�x���擾
            float angle = Random.Range(range.x, range.y);


            if (forceAngle != 0)
            {
                angle = forceAngle;
            }
            var pos = GetSplitedPositionWithAngles(new Vector2Int(Random.Range(0, areaSplit.x), Random.Range(0, areaSplit.y)), angle);

            //playArea�𒆐S��angle�x��]
            var spawnPos = playArea.position;
            spawnPos.z -= fishDistance;
            var spawnPosRotated = Quaternion.Euler(0, angle, 0) * (pos - spawnPos) + spawnPos;

            //Distance���O�Ɉړ�
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
    /// �v���C�G���A�𕪊����������_���Ȉʒu���擾
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSplitedRandomPosition()
    {
        //Splits�̒����烉���_���őI��
        var randIndex = new Vector2Int(Random.Range(0, areaSplit.x), Random.Range(0, areaSplit.y));
        return GetSplitedPosition(randIndex);
    }

    /// <summary>
    /// �v���C�G���A�𕪊������ʒu���擾
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private Vector3 GetSplitedPosition(Vector2Int index)
    {
        var size = playArea.lossyScale;
        var areaPos = playArea.position;
        var startPos = new Vector3(areaPos.x - size.x / 2, areaPos.y - size.y / 2, areaPos.z);

        //PlayArea�̒��ł̈ʒu���v�Z
        startPos.x += size.x / (areaSplit.x - 1) * index.x;
        startPos.y += size.y / (areaSplit.y - 1) * index.y;
        return startPos;
    }

    /// <summary>
    /// �v���C�G���A�𕪊������ʒu���擾
    /// ��]�Ή���
    /// </summary>
    /// <param name="index"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    private Vector3 GetSplitedPositionWithAngles(Vector2Int index, float angle)
    {
        //Splits�̒����烉���_���őI��
        var areaPos = playArea.position;

        var startPos = GetSplitedPosition(index);
        //startPos��areaPos�𒆐S��angle�x��]
        return Quaternion.Euler(0, angle, 0) * (startPos - areaPos) + areaPos;


    }

    /// <summary>
    /// �v���C�G���A���̃����_���Ȉʒu���擾
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomPosition()
    {
        var size = playArea.lossyScale;
        var areaPos = playArea.position;

        //PlayArea�̒��ł̈ʒu���v�Z
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
