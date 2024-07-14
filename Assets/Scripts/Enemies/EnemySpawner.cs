using GameUtils;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemiesPrefabs;
    [field: SerializeField] public Bounds Bounds { get; protected set; }

    public readonly List<Enemy> SpawnedEnemies = new();
    [field: SerializeField] public int MaxSpawnedEnemiesCount { get; protected set; } = 1;
    [field: SerializeField] public Timer SpawnTimer { get; protected set; }


    private void Awake()
    {
        SpawnTimer.onCompleted.AddListener(OnSpawnTimerOver);
        SpawnTimer.Restart();
    }
    void OnSpawnTimerOver()
    {
        SpawnRandomEnemy();
        SpawnTimer.Restart();
    }

    private void Update()
    {
        SpawnTimer.Update();
    }

    public Enemy SpawnEnemy(GameObject enemyPrefab, Vector3 position)
    {
        if (SpawnedEnemies.Count >= MaxSpawnedEnemiesCount)
            return null;

        GameObject gm = Instantiate(enemyPrefab);
        if (!gm.TryGetComponent(out Enemy enemy))
            return null;

        enemy.transform.position = position;
        SpawnedEnemies.Add(enemy);
        return enemy;
    }

    public Enemy SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 randomPos = GUtils.GetRandomPositionInBounds(Bounds);
        randomPos.y = 0;
        return SpawnEnemy(enemyPrefab, transform.position + randomPos);
    }

    public Enemy SpawnRandomEnemy()
    {
        GameObject enemyPrefab = enemiesPrefabs.GetOneRandom();
        return SpawnEnemy(enemyPrefab);
    }

    private void OnDrawGizmosSelected()
    {
        EditorDraw.DrawBounds(Bounds, transform.position, Color.red);
    }
}
