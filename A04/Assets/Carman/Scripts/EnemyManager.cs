using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Types (with cost)")]
    public List<GameObject> enemyTypes;

    [Header("Room Settings")]
    public int minRoomCost = 50;
    public int maxRoomCost = 100;

    private Transform[] spawnPoints;
    private int currentCost;

    void Awake()
    {
        FindSpawnPoints();
    }

    void Start()
    {
        GenerateEnemies();
    }

    void FindSpawnPoints()
    {
        Transform spawnContainer = transform.Find("SpawnPoints");

        if (spawnContainer == null)
        {
            Debug.LogError($"EnemySpawner on {gameObject.name} cannot find a child named 'SpawnPoints'.");
            spawnPoints = new Transform[0];
            return;
        }

        int count = spawnContainer.childCount;
        spawnPoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            spawnPoints[i] = spawnContainer.GetChild(i);
        }
    }

    void GenerateEnemies()
    {
        currentCost = Random.Range(minRoomCost, maxRoomCost + 1);
        Debug.Log($"{gameObject.name} room cost: {currentCost}");

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points found — no enemies will spawn.");
            return;
        }

        int remainingCost = currentCost;
        int spawnIndex = 0;

        while (true)
        {
            // Only keep enemy prefabs whose BaseEnemy.cost fits
            List<GameObject> affordable = enemyTypes.FindAll(e =>
            {
                BaseEnemy baseEnemy = e.GetComponent<BaseEnemy>();
                return baseEnemy != null && baseEnemy.cost <= remainingCost;
            });

            if (affordable.Count == 0)
            {
                Debug.Log("No more enemies fit within remaining cost. Stopping.");
                break;
            }

            if (spawnIndex >= spawnPoints.Length)
            {
                Debug.LogWarning("Out of spawn points. Stopping early.");
                break;
            }

            // Pick a random prefab that fits
            GameObject pick = affordable[Random.Range(0, affordable.Count)];
            BaseEnemy pickInfo = pick.GetComponent<BaseEnemy>();

            // Spawn at point
            Transform point = spawnPoints[spawnIndex++];
            Instantiate(pick, point.position, point.rotation);

            // Deduct cost
            remainingCost -= pickInfo.cost;
        }
    }
}
