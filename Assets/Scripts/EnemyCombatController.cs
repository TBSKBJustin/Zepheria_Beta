using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{
    public Animator animator; // ����ʹ�õ� Animator
    public float attackInterval = 2f; // ÿ�ι���֮��ļ��ʱ��
    private int attackCount = 0; // ��ǰ��������
    private bool inWeaknessState = false; // �Ƿ�������״̬
    private bool isBattleStarted = false; // �Ƿ����ս��״̬
    private int attackSet = 0; // ��ǰ�Ĺ������
    private int attackActionIndex = 0; // ��ǰ��������еĶ�������

    public DodgeTrigger leftTrigger;  // ������߶���������
    public DodgeTrigger rightTrigger; // �����ұ߶���������

    private readonly int[][] attackCombinations = new int[][]
    {
        new int[] { 1, 2, 3 }, // ��� 1
        new int[] { 2, 3, 1 }, // ��� 2
        new int[] { 3, 1, 2 }  // ��� 3
    };

    public GameObject weaknessTrigger; // �����Ĺ������� Trigger
    public int maxHealth = 100; // ��������Ѫ��
    private int currentHealth; // ��ǰѪ��
    private Material weaknessMaterial;

    private readonly Vector3[] weaknessPositions = new Vector3[]
    {
        new Vector3(0.7f, 1.5f, -0.6f), // ��� 1, 2, 3 ��λ��
        new Vector3(0.5f, 1.3f, -0.6f), // ��� 2, 3, 1 ��λ��
        new Vector3(0.28f, 1.3f, -0.4f) // ��� 3, 1, 2 ��λ��
    };

    void Start()
    {
        weaknessTrigger?.SetActive(false); // ȷ������ Trigger Ĭ�Ϲر�
        currentHealth = maxHealth; // ��ʼ��Ѫ��
        weaknessMaterial = weaknessTrigger.GetComponent<Renderer>().material; // ��ȡ����
        SetMaterialTransparency(0); // ��ʼ��Ϊ��ȫ͸��
    }

    void PerformAttack()
    {
        if (!isBattleStarted || inWeaknessState) return;

        // ÿ�ν��빥���߼�����˳��ѡ����
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        if (attackCount < 3)
        {
            // �ӵ�ǰ��������а�˳��ѡ����
            int attackAction = attackCombinations[attackSet][attackActionIndex];
            animator.SetTrigger($"attack{attackAction}");
            Debug.Log($"Enemy performed attack: {attackAction} | Attack Count: {attackCount + 1}");

            CheckDodgeSuccess(attackAction);

            attackActionIndex = (attackActionIndex + 1) % 3; // ѭ���л�����һ����
            attackCount++;
            yield return new WaitForSeconds(attackInterval);
        }
        else
        {
            TriggerWeakness();
        }
    }

    public void StartBattle()
    {
        isBattleStarted = true;
        // ���ѡ��һ���������
        attackSet = Random.Range(0, attackCombinations.Length);
        attackActionIndex = 0; // ���ù�����ϵ�����

        Debug.Log($"Starting Battle with Attack Set: {attackSet + 1}");
        InvokeRepeating(nameof(PerformAttack), 1f, attackInterval); // ��ʼ����ѭ��
    }

    void TriggerWeakness()
    {
        if (inWeaknessState) return; // ��ֹ�ظ���������״̬

        Debug.Log("Weakness Triggered!");
        inWeaknessState = true;

        // ��������
        int attackAction = attackCombinations[attackSet][attackActionIndex];
        animator.speed = 0.5f; // ��������
        animator.SetTrigger($"attack{attackAction}");
        Debug.Log($"Enemy performed weakness attack: {attackAction}");

        attackActionIndex = (attackActionIndex + 1) % 3; // ׼����һ������
        attackCount = 0; // ���ù�������

        weaknessTrigger.transform.position = weaknessPositions[attackSet];
        Debug.Log(weaknessTrigger.transform.position);
        weaknessTrigger?.SetActive(true); // ��ʾ���� Trigger
        StartCoroutine(FadeInWeaknessBall());

        // ������δ����������2 ��󷵻���ͨѭ��
        Invoke(nameof(EndWeaknessState), 2f);
    }
    IEnumerator FadeInWeaknessBall()
    {
        float duration = 1f; // �������ʱ��
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration); // ��͸������ȫ��͸��
            SetMaterialTransparency(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetMaterialTransparency(1); // ȷ��������ȫ��͸��
    }

    void SetMaterialTransparency(float alpha)
    {
        if (weaknessMaterial != null)
        {
            Color color = weaknessMaterial.color;
            color.a = alpha;
            weaknessMaterial.color = color;
        }
    }

    public void OnPlayerHitWeakness()
    {
        if (inWeaknessState)
        {
            Debug.Log("Player hit Weakness!");
            CancelInvoke(nameof(EndWeaknessState)); // ȡ���Զ��ص���ͨѭ��
            animator.SetTrigger("HitReact");

            weaknessTrigger?.SetActive(false); // �������� Trigger
            inWeaknessState = false;
            animator.speed = 1f; // �ָ������ٶ�
            TakeDamage(20);
        }
    }

    void EndWeaknessState()
    {
        if (inWeaknessState)
        {
            Debug.Log("Player missed Weakness!");
            animator.SetTrigger("idle"); // ���� Idle ״̬

            weaknessTrigger?.SetActive(false); // �������� Trigger
            inWeaknessState = false;
            animator.speed = 1f; // �ָ������ٶ�

            // ���»ص���ͨ����ѭ��
            attackSet = Random.Range(0, attackCombinations.Length);
            attackActionIndex = 0;
            attackCount = 0; // ���ù�������
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage; // ����Ѫ��
        Debug.Log($"Enemy Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die(); // Ѫ����Ϊ��ʱ��������
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        CancelInvoke(); // ֹͣ���й����߼�
        animator.SetTrigger("dead"); // ������������
        isBattleStarted = false; // ֹͣս��״̬
        weaknessTrigger?.SetActive(false); // ȷ������ Trigger ����
    }

    void CheckDodgeSuccess(int attackAction)
    {
        bool dodgeSuccess = false;

        if (attackAction == 1 || attackAction == 3)
        {
            if (leftTrigger.IsPlayerInside())
            {
                dodgeSuccess = true;
            }
        }
        else if (attackAction == 2)
        {
            if (rightTrigger.IsPlayerInside())
            {
                dodgeSuccess = true;
            }
        }

        if (dodgeSuccess)
        {
            Debug.Log($"Player successfully dodged attack {attackAction}!");
        }
        else
        {
            Debug.Log($"Player failed to dodge attack {attackAction}!");
        }
    }
}
