using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CombatMode : MonoBehaviour
{
    public GameObject rightHandModel;
    public GameObject swordModel;
    public GameObject rightDirectModel;
    public GameObject rightTeleport;
    public GameObject leftMenu;
    public GameObject leftMenuButton;
    public GameObject leftHand;
    public GameObject leftDirect;

    public MonoBehaviour dynamicMoveProvider;
    public MonoBehaviour continuousTurnProvider;
    public MonoBehaviour snapTurnProvider;
    public MonoBehaviour teleportationProvider;

    private InputActionProperty originalLeftHandMoveAction;
    public InputActionProperty emptyAction;

    [Header("Player Health")]
    public int maxHealth = 100; // 玩家最大血量
    private int currentHealth; // 玩家当前血量
    [SerializeField] private Image playerHealthBar; // 玩家血条 UI

    void Start()
    {
        currentHealth = maxHealth; // 初始化血量
        UpdatePlayerHealthBar(); // 初始化血条显示


    }

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

        if (continuousTurnProvider != null) continuousTurnProvider.enabled = false;
        if (snapTurnProvider != null) snapTurnProvider.enabled = false;
        if (teleportationProvider != null) teleportationProvider.enabled = false;


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

        if (continuousTurnProvider != null) continuousTurnProvider.enabled = true;
        if (snapTurnProvider != null) snapTurnProvider.enabled = true;
        if (teleportationProvider != null) teleportationProvider.enabled = true;

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 确保血量在有效范围内
        UpdatePlayerHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdatePlayerHealthBar()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.fillAmount = (float)currentHealth / maxHealth; // 根据当前血量更新血条
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // 可以在这里添加玩家死亡逻辑，例如重生或结束游戏
    }
}
