using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimBattleTrigger : MonoBehaviour
{
    private bool battleStarted = false;

    public GameObject FightTrigger; // Weakness Ball Trigger
    public GameObject enterBattleTrigger;
    public GameObject WeaknessBall;

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
        FightTrigger.SetActive(true); // Enable Weakness Ball
        FindObjectOfType<CombatMode>().EnterCombatMode();
        Debug.Log("Battle Started!");
        enterBattleTrigger.SetActive(false); // Hide entry trigger
        WeaknessBall.SetActive(true);
    }

    public void EndBattle()
    {
        FightTrigger.SetActive(false);
        Debug.Log("Battle Ended!");
        FindObjectOfType<CombatMode>().ExitCombatMode();
    }
}
