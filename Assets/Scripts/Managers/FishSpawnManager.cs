using UnityEngine;

public class FishSpawnManager : MonoBehaviour
{
    [SerializeField]
    private Transform playArea;

    [SerializeField]
    private SpawnProfile profile;


    private float angleCenter;
    private float angleRange;


    private float angleTimer;

    void Start()
    {
        StartCoroutine(profile.TimeRate.DelayCoroutine(new SpawnOperation(this)));
        angleTimer = 0;

    }

    private void Update()
    {
        angleCenter = profile.AngleCenter.Evaluate(angleTimer);
        angleRange = profile.AngleRange.Evaluate(angleTimer);
        angleTimer += Time.deltaTime;
    }

    private void SpawnFishAngle()
    {
        //�����_���Ȋp�x���擾
        float angle = angleCenter + Random.Range(-angleRange / 2, angleRange / 2);


        var pos = GetSplitedPositionWithAngles(new Vector2Int(Random.Range(0, profile.AreaSplit.x), Random.Range(0, profile.AreaSplit.y)), angle);

        //playArea�𒆐S��angle�x��]
        var spawnPos = playArea.position;
        spawnPos.z -= profile.FishDistance;
        var spawnPosRotated = Quaternion.Euler(0, angle, 0) * (pos - spawnPos) + spawnPos;

        //Distance���O�Ɉړ�
        spawnPosRotated.z += profile.FishDistance;

        GameObject fish = Instantiate(profile.TimeRate.GetPrefab(), spawnPosRotated, Quaternion.identity);
        fish.transform.LookAt(pos);
    }

    private void SpawnFishStraight()
    {
        var pos = GetSplitedRandomPosition();
        pos.z += profile.FishDistance;
        GameObject fish = Instantiate(profile.TimeRate.GetPrefab(), pos, Quaternion.Euler(0, -180, 0));
        fish.transform.SetParent(transform);
    }



    /// <summary>
    /// �v���C�G���A�𕪊����������_���Ȉʒu���擾
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSplitedRandomPosition()
    {
        //Splits�̒����烉���_���őI��
        var randIndex = new Vector2Int(Random.Range(0, profile.AreaSplit.x), Random.Range(0, profile.AreaSplit.y));
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
        startPos.x += size.x / (profile.AreaSplit.x - 1) * index.x;
        startPos.y += size.y / (profile.AreaSplit.y - 1) * index.y;
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
        Angle,
    }

    public class SpawnOperation
    {
        FishSpawnManager manager;
        public SpawnOperation(FishSpawnManager manager)
        {
            this.manager = manager;
        }


        public void SpawnFish()
        {
            if (manager.profile.SpawnMode == SpawnMode.Front)
            {
                manager.SpawnFishStraight();
            }
            else
            {
                manager.SpawnFishAngle();
            }
        }



    }
}
