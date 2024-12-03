using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private bool battleStarted = false;
    public EnemyCombatController enemyController;

    public GameObject rightTrigger;
    public GameObject leftTrigger;
    public GameObject enterBattleTrigger;

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
        rightTrigger.SetActive(true);
        leftTrigger.SetActive(true);

        FindObjectOfType<CombatMode>().EnterCombatMode();

        enemyController.StartBattle();

        Debug.Log("Battle Started!");
        enterBattleTrigger.SetActive(false);
    }
}
