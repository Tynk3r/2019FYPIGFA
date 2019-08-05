using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // References 
    GameController controllerReference;
    public List<GameObject> enemies = new List<GameObject>();
    private float m_countdown;
    private int m_currEnemyCount;
    // Max enemies in an instance
    const int maxDocileEnemies = 6;
    const int maxAngryEnemies = 12;
    const int maxEnragedEnemies = 25;
    const int maxInsaneEnemies = 50;
    // SpawnCount
    const int docileEnemySpawnCount = maxDocileEnemies / 2;
    const int angryEnemySpawnCount = maxAngryEnemies / 2;
    const int enragedEnemySpawnCount = maxEnragedEnemies / 2;
    const int insaneEnemySpawnCount = maxInsaneEnemies / 2;
    // Spawnrate
    public float spawnRate = 1f;
    List<EnemySpawn> enemySpawners;
    // Start is called before the first frame update
    void Start()
    {
        enemySpawners = new List<EnemySpawn>();
        // Find all the enemy spawner objects in the game
        EnemySpawn[] spawnersFound = (EnemySpawn[])FindObjectsOfType(typeof(EnemySpawn));
        foreach (EnemySpawn spawner in spawnersFound)
        {
            enemySpawners.Add(spawner);
        }
        m_countdown = 0f;
        controllerReference = (GameController)FindObjectOfType(typeof(GameController));
    }

    // Update is called once per frame
    void Update()
    {
        m_countdown -= Time.deltaTime;
        if (m_countdown > 0f)
            return;
        m_countdown = spawnRate;
        int enemiesToSpawn, enemySpawnCount, maxEnemyCount = enemiesToSpawn = enemySpawnCount = 0;
        switch(controllerReference.aggressionLevel)
        {
            case GameController.AGGRESSION_LEVELS.DOCILE:
                enemySpawnCount = docileEnemySpawnCount;
                maxEnemyCount = maxDocileEnemies;
                break;
            case GameController.AGGRESSION_LEVELS.ANGRY:
                enemySpawnCount = angryEnemySpawnCount;
                maxEnemyCount = maxAngryEnemies;
                break;
            case GameController.AGGRESSION_LEVELS.ENRAGED:
                enemySpawnCount = enragedEnemySpawnCount;
                maxEnemyCount = maxEnragedEnemies;
                break;
            case GameController.AGGRESSION_LEVELS.INSANE:
                enemySpawnCount = insaneEnemySpawnCount;
                maxEnemyCount = maxInsaneEnemies;
                break;
        }
        enemySpawnCount = insaneEnemySpawnCount;
        maxEnemyCount = maxInsaneEnemies;
        enemiesToSpawn = Mathf.Min(maxEnemyCount - m_currEnemyCount, enemySpawnCount);
        while (enemiesToSpawn > 0)
        {
            SpawnEnemy();
            --enemiesToSpawn;
            ++m_currEnemyCount;
        }
    }

    void SpawnEnemy()
    {
        int num1 = Random.Range(0, enemySpawners.Count - 1);
        int num2 =  Random.Range(0, enemies.Count);
        enemySpawners[num1].SpawnEnemy(enemies[num2]);
    }
}
