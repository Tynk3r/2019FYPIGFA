using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public void SpawnEnemy(GameObject _enemyToSpawn)
    {
        Instantiate(_enemyToSpawn, transform);
    }
}
