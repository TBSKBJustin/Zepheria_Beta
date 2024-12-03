using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private bool battleStarted = false; // 确保战斗只触发一次
    public EnemyCombatController enemyController; // 引用敌人行为脚本

    void OnTriggerEnter(Collider other)
    {
        if (!battleStarted && other.CompareTag("Player"))
        {
            battleStarted = true;
            StartBattle();
        }
    }

    void StartBattle()
    {
        // 1. 切换玩家为战斗模式
        FindObjectOfType<CombatMode>().EnterCombatMode();

        // 2. 启动敌人行为
        enemyController.StartBattle();

        Debug.Log("Battle Started!");
    }
}
