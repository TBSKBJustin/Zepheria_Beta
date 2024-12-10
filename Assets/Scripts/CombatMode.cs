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
    public int maxHealth = 100; // ������Ѫ��
    private int currentHealth; // ��ҵ�ǰѪ��
    [SerializeField] private Image playerHealthBar; // ���Ѫ�� UI
    public GameObject HealthBarObject;

    [Header("Combat Music")]
    public AudioClip combatMusic; // ս��ģʽ����
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth; // ��ʼ��Ѫ��
        UpdatePlayerHealthBar(); // ��ʼ��Ѫ����ʾ
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = combatMusic;
        audioSource.loop = true; // ȷ������ѭ������
        audioSource.playOnAwake = false; // ��ֹ�Զ�����
        audioSource.volume = 0; // ��ʼ����Ϊ 0


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
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ȷ��Ѫ������Ч��Χ��
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
            playerHealthBar.fillAmount = (float)currentHealth / 105; // ���ݵ�ǰѪ������Ѫ��
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");
        // ���������������������߼������������������Ϸ
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Ѫ������+20��ͬʱ���䵱ǰѪ��
        Debug.Log($"Player max health increased by {amount}. Current max health: {maxHealth}");
    }

    private IEnumerator FadeInMusic()
    {
        float targetVolume = 0.5f; // Ŀ������
        float duration = 1.5f; // �������ʱ��
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

        audioSource.volume = targetVolume; // ȷ������������ȷ
    }

    private IEnumerator FadeOutMusic()
    {
        float duration = 1.5f; // ��������ʱ��
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0; // ȷ������������ȷ
        audioSource.Stop();
    }
}
