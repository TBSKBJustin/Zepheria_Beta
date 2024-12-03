using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private bool battleStarted = false; // ȷ��ս��ֻ����һ��
    public EnemyCombatController enemyController; // ���õ�����Ϊ�ű�

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
        // 1. �л����Ϊս��ģʽ
        FindObjectOfType<CombatMode>().EnterCombatMode();

        // 2. ����������Ϊ
        enemyController.StartBattle();

        Debug.Log("Battle Started!");
    }
}
