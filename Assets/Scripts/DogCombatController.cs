using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogCombatController : MonoBehaviour
{
    [Header("References")]
    public Animator animator; // ���˵� Animator
    public DodgeTrigger leftTrigger;  // �����������
    public DodgeTrigger midTrigger;   // �м���������
    public DodgeTrigger rightTrigger; // �ұ���������


    [Header("Action UI Sprites")]
    public Image rightActionSprite;
    public Image midActionSprite;
    public Image leftActionSprite;

    [Header("Weakness Balls")]
    public GameObject[] weaknessBalls;  // ���� attackSet ��Ӧ��������

    [Header("Health Settings")]
    public int maxHealth = 100; // �������Ѫ��
    public int EnemyDamage;     // �������ҵ��˺�
    [SerializeField] private Image _healthbarSprite;

    [Header("Attack Settings")]
    public float attackInterval = 2f; // ������������������ȴʱ�䣩
    private readonly int[][] attackCombinations =
    {
        new int[] { 1, 2},
        new int[] { 2, 1}
    };

    private int currentHealth;
    private bool isBattleStarted = false;
    private bool inWeaknessState = false;
    private int attackSet = 0;     // ��ǰ�������е����
    private int attackActionIndex = 0;
    private int attackCount = 0;   // ��ǰ�ѽ��еĹ�������

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        DisableWeaknessBalls();
        DisableActionSprites();
    }

    public void StartBattle()
    {
        if (!isBattleStarted)
        {
            isBattleStarted = true;
            // ��������������Э��
            StartCoroutine(AttackLoop());
        }
    }

    /// <summary>
    /// ������ѭ��Э�̣�  
    /// 1. ���ѡ�񹥻����  
    /// 2. ��������3�ι��������� attackSet ȷ������ģʽ��  
    /// 3. ��������״̬��������л�����  
    /// 4. ������δ�����ָ��µĹ���ѭ��
    /// </summary>
    IEnumerator AttackLoop()
    {
        while (isBattleStarted)
        {
            // ���ѡ��һ���������
            attackSet = Random.Range(0, attackCombinations.Length);
            attackActionIndex = 0;
            attackCount = 0;

            // ִ��3����ͨ����
            for (int i = 0; i < 2; i++)
            {
                if (!isBattleStarted) yield break;
                yield return StartCoroutine(PerformSingleAttack());
            }

            // 3�ι������������״̬
            yield return StartCoroutine(HandleWeaknessPhase());
        }
    }

    /// <summary>
    /// ִ�е��ι������̵�Э�̣�  
    /// - ��ʾ����ָ��UI  
    /// - ��������  
    /// - �ȴ��������  
    /// - �����������Ƿ�ɹ�  
    /// </summary>
    IEnumerator PerformSingleAttack()
    {
        if (inWeaknessState) yield break; // ��������״̬�в�������ͨ����

        int attackAction = attackCombinations[attackSet][attackActionIndex];
        ShowActionSprite(attackAction);
        animator.SetTrigger($"attack{attackAction}");
        attackCount++;

        // �ȴ����������ɺ���������ж�
        yield return new WaitForSeconds(attackInterval);

        CheckDodgeSuccess(attackAction);
        DisableActionSprites();

        // ׼����һ����������
        attackActionIndex = (attackActionIndex + 1) % 2;
    }

    /// <summary>
    /// ����״̬Э�̣�  
    /// ��������״̬ʱ��  
    /// - ���ٲ��Ŷ���  
    /// - ��ʾ��Ӧ��������  
    /// - �ȴ���Ҵ�������ʱ��ָ�����״̬
    /// </summary>
    IEnumerator HandleWeaknessPhase()
    {
        inWeaknessState = true;

        int weaknessAction = attackCombinations[attackSet][attackActionIndex];
        ShowActionSprite(weaknessAction);

        animator.speed = 0.2f;
        animator.SetTrigger($"attack{weaknessAction}");

        GameObject targetBall = weaknessBalls[attackSet];
        targetBall.SetActive(true);
        yield return StartCoroutine(FadeInWeaknessBall(targetBall));

        // �ȴ������3���ڹ�������
        float weaknessDuration = 3f;
        float timer = 0f;
        bool weaknessHit = false;

        while (timer < weaknessDuration)
        {
            if (!inWeaknessState)
            {
                // ��������㴰������Ҵ��������㣨ͨ�� OnPlayerHitWeakness ���ã�������ǰ�˳�����״̬
                weaknessHit = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // ������δ��������򷵻���ͨ����ѭ��
        if (inWeaknessState)
        {
            // ���û������������
            if (!weaknessHit)
            {
                Debug.Log("Player missed Weakness!");
                CheckDodgeSuccess(attackCombinations[attackSet][attackActionIndex]);
            }

            // ����״̬
            DisableActionSprites();
            DisableWeaknessBalls();
            animator.SetTrigger("idle");
            animator.speed = 1f;
            inWeaknessState = false;

            // ���¿�ʼ�µĹ���ѭ��ʱ�������� AttackLoop ��ѡ�� attackSet
        }
    }

    /// <summary>
    /// ����һ�������ʱ���ô˷�����  
    /// �ⲿ�߼���������ҹ�����������߼��е��ñ�������
    /// </summary>
    public void OnPlayerHitWeakness(GameObject hitBall)
    {
        if (inWeaknessState)
        {
            Debug.Log($"Player hit Weakness on {hitBall.name}!");
            // ��������״̬
            hitBall.SetActive(false);
            inWeaknessState = false;
            animator.SetTrigger("HitReact");
            animator.speed = 1f;

            // ����������˺�
            TakeDamage(20);

            DisableActionSprites();
        }
    }

    void CheckDodgeSuccess(int attackAction)
    {
        if (!isBattleStarted) return;
        bool dodgeSuccess = false;

        if (attackAction == 1 && leftTrigger.IsPlayerInside())
        {
            dodgeSuccess = true;
        }
        //else if (attackAction == 2 && midTrigger.IsPlayerInside())
        //{
        //    dodgeSuccess = true;
        //}
        else if (attackAction == 2 && rightTrigger.IsPlayerInside())
        {
            dodgeSuccess = true;
        }

        if (dodgeSuccess)
        {
            Debug.Log($"Player successfully dodged attack {attackAction}!");
        }
        else
        {
            Debug.Log($"Player failed to dodge attack {attackAction}!");

            // ���������˺�
            CombatMode playerCombatMode = FindObjectOfType<CombatMode>();
            if (playerCombatMode != null)
            {
                playerCombatMode.TakeDamage(EnemyDamage);
            }
            else
            {
                Debug.LogError("CombatMode script not found! Cannot deal damage to the player.");
            }
        }
    }

    IEnumerator FadeInWeaknessBall(GameObject ball)
    {
        Renderer renderer = ball.GetComponent<Renderer>();
        Material material = renderer.material;
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            SetMaterialTransparency(material, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetMaterialTransparency(material, 1);
    }

    void SetMaterialTransparency(Material material, float alpha)
    {
        if (material != null)
        {
            Color color = material.color;
            color.a = alpha;
            material.color = color;
        }
    }

    void DisableWeaknessBalls()
    {
        foreach (var ball in weaknessBalls)
        {
            ball.SetActive(false);
        }
    }

    void DisableActionSprites()
    {
        rightActionSprite.enabled = false;
        midActionSprite.enabled = false;
        leftActionSprite.enabled = false;
    }

    void ShowActionSprite(int attackAction)
    {
        DisableActionSprites(); // ����������ͼƬ
        switch (attackAction)
        {
            case 1:
                leftActionSprite.enabled = true;
                break;
            //case 2:
            //    midActionSprite.enabled = true;
            //    break;
            case 2:
                rightActionSprite.enabled = true;
                break;
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy Health: {currentHealth}/{maxHealth}");
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        isBattleStarted = false;
        animator.SetTrigger("dead");

        DogBattleTrigger battleTrigger = FindObjectOfType<DogBattleTrigger>();
        if (battleTrigger != null)
        {
            battleTrigger.EndBattle();
        }

        // ֪ͨ����˳�ս��ģʽ
        CombatMode playerCombatMode = FindObjectOfType<CombatMode>();
        if (playerCombatMode != null)
        {
            playerCombatMode.ExitCombatMode();
        }
    }

    void UpdateHealthBar()
    {
        if (_healthbarSprite != null)
        {
            _healthbarSprite.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
