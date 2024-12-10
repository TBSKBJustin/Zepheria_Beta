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
    public GameObject HealthBarObject;

    [Header("Combat Music")]
    public AudioClip combatMusic; // 战斗模式音乐
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth; // 初始化血量
        UpdatePlayerHealthBar(); // 初始化血条显示
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = combatMusic;
        audioSource.loop = true; // 确保音乐循环播放
        audioSource.playOnAwake = false; // 禁止自动播放
        audioSource.volume = 0; // 初始音量为 0


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

        StartCoroutine(FadeInMusic());


    }

    public void ExitCombatMode()
    {
        rightHandModel.SetActive(true);
        swordModel.SetActive(false);
        rightDirectModel.SetActive(true);
        //rightTeleport.SetActive(true);
        //leftMenu.SetActive(true);
        leftMenuButton.SetActive(true);
        leftHand.SetActive(true);
        leftDirect.SetActive(true);

        if (continuousTurnProvider != null) continuousTurnProvider.enabled = true;
        if (snapTurnProvider != null) snapTurnProvider.enabled = true;
        if (teleportationProvider != null) teleportationProvider.enabled = true;

        StartCoroutine(FadeOutMusic());

        currentHealth = maxHealth;
        UpdatePlayerHealthBar();

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

    public void UpdatePlayerHealthBar()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.fillAmount = (float)currentHealth / 105; // 根据当前血量更新血条
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // 可以在这里添加玩家死亡逻辑，例如重生或结束游戏
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // 血量上限+20，同时补充当前血量
        Debug.Log($"Player max health increased by {amount}. Current max health: {maxHealth}");
    }

    private IEnumerator FadeInMusic()
    {
        float targetVolume = 0.5f; // 目标音量
        float duration = 1.5f; // 渐入持续时间
        float elapsedTime = 0f;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        while (elapsedTime < duration)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume; // 确保最终音量正确
    }

    private IEnumerator FadeOutMusic()
    {
        float duration = 1.5f; // 渐出持续时间
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0; // 确保最终音量正确
        audioSource.Stop();
    }
}
