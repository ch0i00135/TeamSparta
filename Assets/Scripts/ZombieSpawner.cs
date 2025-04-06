using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombiePrefab;
    public Transform[] spawnPoints;
    public float spawnRate = 1.5f;
    public int maxZombies = 50;

    private float nextSpawnTime;
    private List<GameObject> zombies = new List<GameObject>();

    void Start()
    {
        nextSpawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time > nextSpawnTime && zombies.Count < maxZombies)
        {
            SpawnZombie();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnZombie()
    {
        // 랜덤 열 선택
        int laneIndex = Random.Range(0, spawnPoints.Length);

        // 좀비 생성
        GameObject zombie = Instantiate(
            zombiePrefab[laneIndex],
            spawnPoints[laneIndex].position,
            Quaternion.identity
        );
        switch (laneIndex)
        {
            case 0:
                zombie.layer = 8;
                break;
            case 1:
                zombie.layer = 9;
                break;
            case 2:
                zombie.layer = 10;
                break;
        }

        zombies.Add(zombie);
    }

    public void RemoveZombie(GameObject zombie)
    {
        if (zombies.Contains(zombie))
        {
            zombies.Remove(zombie);
            Destroy(zombie);
        }
    }
}
