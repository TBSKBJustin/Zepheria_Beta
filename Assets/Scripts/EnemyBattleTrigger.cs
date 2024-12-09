using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleTrigger : MonoBehaviour
{
    private bool battleStarted = false;
    public SkeletonCombatController enemyController;

    public GameObject rightTrigger;
    public GameObject midTrigger;
    public GameObject leftTrigger;
    public GameObject enterBattleTrigger;
    public GameObject HealthBar;

    public float rotationSpeedX = 30f;
    public float rotationSpeedZ = 30f;

    void Update()
    {
        if (enterBattleTrigger != null)
        {
            enterBattleTrigger.transform.Rotate(new Vector3(rotationSpeedX, 0, rotationSpeedZ) * Time.deltaTime);
        }
    }

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
        midTrigger.SetActive(true);
        HealthBar.SetActive(true);

        FindObjectOfType<CombatMode>().EnterCombatMode();

        enemyController.StartBattle();

        Debug.Log("Battle Started!");
        enterBattleTrigger.SetActive(false);
    }
}
