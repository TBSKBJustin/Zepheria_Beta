using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMode : MonoBehaviour
{
    public GameObject rightHandModel;  // 玩家右手模型
    public GameObject swordModel;
    public GameObject rightDirectModel;
    public GameObject rightTeleport;
    public GameObject leftMenu;
    public GameObject leftMenuButton;
    public GameObject leftHand;
    public GameObject leftDirect;

    public void EnterCombatMode()
    {
        rightHandModel.SetActive(false);
        swordModel.SetActive(true);
        rightDirectModel.SetActive(false);
        rightTeleport.SetActive(false);
        leftMenu.SetActive(false);
        leftMenuButton.SetActive(false);
        leftHand.SetActive(false);
        leftDirect.SetActive(false);

    }

    public void ExitCombatMode()
    {
        rightHandModel.SetActive(true);
        swordModel.SetActive(false);
        rightDirectModel.SetActive(true);
        rightTeleport.SetActive(true);
        leftMenu.SetActive(true);
        leftMenuButton.SetActive(true);
        leftHand.SetActive(true);
        leftDirect.SetActive(true);
    }
}

