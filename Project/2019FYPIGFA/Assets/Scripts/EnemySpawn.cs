using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public void SpawnEnemy(GameObject _enemyToSpawn, GameController gameController)
    {
        Enemy enemy = Instantiate(_enemyToSpawn, transform).GetComponent<Enemy>();
        gameController.enemyList.Add(enemy);
    }
}
