using System;
using System.Collections;
using UnityEngine;
using static FishSpawnManager;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "NekoPunch/SpawnProfile")]
public class SpawnProfile : ScriptableObject
{
    public Vector2Int AreaSplit;
    public float FishDistance;
    [SerializeReference, SubclassSelector]
    public ISpawnTime TimeRate;
    public SpawnMode SpawnMode;
    public AnimationCurve AngleCenter;
    public AnimationCurve AngleRange;

}

public interface ISpawnTime
{

    IEnumerator DelayCoroutine(SpawnOperation op);

    GameObject GetPrefab();
}

[Serializable]
public class ConstantSpawnTime : ISpawnTime
{
    public float Delay;
    public GameObject[] FishPrefabs;

    public GameObject GetPrefab()
    {
        return FishPrefabs[Random.Range(0, FishPrefabs.Length)];
    }

    IEnumerator ISpawnTime.DelayCoroutine(SpawnOperation op)
    {
        while (true)
        {
            yield return new WaitForSeconds(Delay);
            op.SpawnFish();
        }
    }
}

[Serializable]
public class RandomSpawnTime : ISpawnTime
{
    public float MinDelay;
    public float MaxDelay;

    public GameObject[] FishPrefabs;

    public GameObject GetPrefab()
    {
        return FishPrefabs[Random.Range(0, FishPrefabs.Length)];
    }

    IEnumerator ISpawnTime.DelayCoroutine(SpawnOperation op)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinDelay, MaxDelay));
            op.SpawnFish();
        }
    }
}


[Serializable]
public class ArraySpawnTime : ISpawnTime
{
    public ArrayElement[] Delays;
    public bool IsLoop;
    public GameObject[] FishPrefabs;

    public GameObject GetPrefab()
    {
        return FishPrefabs[Random.Range(0, FishPrefabs.Length)];
    }

    IEnumerator ISpawnTime.DelayCoroutine(SpawnOperation op)
    {
        int index = 0;
        int count;
        if (Delays == null || Delays.Length == 0)
            yield break;

        while (true)
        {
            count = Delays[index].Count;
            yield return new WaitForSeconds(Delays[index].Delay);
            op.SpawnFish();

            count--;

            if (count > 0)
                continue;


            index++;

            if (index >= Delays.Length)
            {
                index = IsLoop ? 0 : Delays.Length - 1;
            }
        }
    }

    [Serializable]
    public class ArrayElement
    {
        public int Count;
        public float Delay;
    }
}

[Serializable]
public class MixedSpawnTime : ISpawnTime
{
    public MixedElement[] Elements;
    public bool IsLoop;

    private ISpawnTime current;

    public GameObject GetPrefab()
    {
        if (current == null)
            return null;

        return current.GetPrefab();
    }

    IEnumerator ISpawnTime.DelayCoroutine(SpawnOperation op)
    {
        if (Elements == null || Elements.Length == 0)
            yield break;

        int index = 0;
        Coroutine coroutine = null;
        float time = Elements[index].Time;

        while (true)
        {
            if (time <= 0)
            {
                index++;
                if (index >= Elements.Length)
                {
                    index = IsLoop ? 0 : Elements.Length - 1;
                }

                time = Elements[index].Time;
                StaticCoroutine.Stop(coroutine);
                coroutine = null;
            }

            if (coroutine == null)
            {
                current = Elements[index].SpawnTime;
                coroutine = StaticCoroutine.Start(current.DelayCoroutine(op));
            }

            if (op.GetManager().gameObject.activeSelf == false)
                yield break;

            yield return null;
            time -= Time.deltaTime;
        }
    }

    [Serializable]
    public class MixedElement
    {
        public float Time;
        [SerializeReference, SubclassSelector]
        public ISpawnTime SpawnTime;
    }

}


