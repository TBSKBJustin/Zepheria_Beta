using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{
    public Animator animator; // 敌人使用的 Animator
    public float attackInterval = 2f; // 每次攻击之间的间隔时间
    private int attackCount = 0; // 当前攻击次数
    private bool inWeaknessState = false; // 是否处于破绽状态
    private bool isBattleStarted = false; // 是否进入战斗状态
    private int attackSet = 0; // 当前的攻击组合
    private int attackActionIndex = 0; // 当前攻击组合中的动作索引

    public DodgeTrigger leftTrigger;  // 引用左边躲闪触发器
    public DodgeTrigger rightTrigger; // 引用右边躲闪触发器

    private readonly int[][] attackCombinations = new int[][]
    {
        new int[] { 1, 2, 3 }, // 组合 1
        new int[] { 2, 3, 1 }, // 组合 2
        new int[] { 3, 1, 2 }  // 组合 3
    };

    public GameObject weaknessTrigger; // 破绽的攻击球体 Trigger
    public int maxHealth = 100; // 怪物的最大血量
    private int currentHealth; // 当前血量
    private Material weaknessMaterial;

    private readonly Vector3[] weaknessPositions = new Vector3[]
    {
        new Vector3(0.7f, 1.5f, -0.6f), // 组合 1, 2, 3 的位置
        new Vector3(0.5f, 1.3f, -0.6f), // 组合 2, 3, 1 的位置
        new Vector3(0.28f, 1.3f, -0.4f) // 组合 3, 1, 2 的位置
    };

    void Start()
    {
        weaknessTrigger?.SetActive(false); // 确保破绽 Trigger 默认关闭
        currentHealth = maxHealth; // 初始化血量
        weaknessMaterial = weaknessTrigger.GetComponent<Renderer>().material; // 获取材质
        SetMaterialTransparency(0); // 初始化为完全透明
    }

    void PerformAttack()
    {
        if (!isBattleStarted || inWeaknessState) return;

        // 每次进入攻击逻辑，按顺序选择动作
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        if (attackCount < 3)
        {
            // 从当前攻击组合中按顺序选择动作
            int attackAction = attackCombinations[attackSet][attackActionIndex];
            animator.SetTrigger($"attack{attackAction}");
            Debug.Log($"Enemy performed attack: {attackAction} | Attack Count: {attackCount + 1}");

            CheckDodgeSuccess(attackAction);

            attackActionIndex = (attackActionIndex + 1) % 3; // 循环切换到下一动作
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
        // 随机选择一个攻击组合
        attackSet = Random.Range(0, attackCombinations.Length);
        attackActionIndex = 0; // 重置攻击组合的索引

        Debug.Log($"Starting Battle with Attack Set: {attackSet + 1}");
        InvokeRepeating(nameof(PerformAttack), 1f, attackInterval); // 开始攻击循环
    }

    void TriggerWeakness()
    {
        if (inWeaknessState) return; // 防止重复进入破绽状态

        Debug.Log("Weakness Triggered!");
        inWeaknessState = true;

        // 破绽动画
        int attackAction = attackCombinations[attackSet][attackActionIndex];
        animator.speed = 0.5f; // 动画减速
        animator.SetTrigger($"attack{attackAction}");
        Debug.Log($"Enemy performed weakness attack: {attackAction}");

        attackActionIndex = (attackActionIndex + 1) % 3; // 准备下一个动作
        attackCount = 0; // 重置攻击次数

        weaknessTrigger.transform.position = weaknessPositions[attackSet];
        Debug.Log(weaknessTrigger.transform.position);
        weaknessTrigger?.SetActive(true); // 显示破绽 Trigger
        StartCoroutine(FadeInWeaknessBall());

        // 如果玩家未攻击破绽，2 秒后返回普通循环
        Invoke(nameof(EndWeaknessState), 2f);
    }
    IEnumerator FadeInWeaknessBall()
    {
        float duration = 1f; // 淡入持续时间
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration); // 从透明到完全不透明
            SetMaterialTransparency(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetMaterialTransparency(1); // 确保最终完全不透明
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
            CancelInvoke(nameof(EndWeaknessState)); // 取消自动回到普通循环
            animator.SetTrigger("HitReact");

            weaknessTrigger?.SetActive(false); // 隐藏破绽 Trigger
            inWeaknessState = false;
            animator.speed = 1f; // 恢复动画速度
            TakeDamage(20);
        }
    }

    void EndWeaknessState()
    {
        if (inWeaknessState)
        {
            Debug.Log("Player missed Weakness!");
            animator.SetTrigger("idle"); // 返回 Idle 状态

            weaknessTrigger?.SetActive(false); // 隐藏破绽 Trigger
            inWeaknessState = false;
            animator.speed = 1f; // 恢复动画速度

            // 重新回到普通攻击循环
            attackSet = Random.Range(0, attackCombinations.Length);
            attackActionIndex = 0;
            attackCount = 0; // 重置攻击计数
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage; // 减少血量
        Debug.Log($"Enemy Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die(); // 血量降为零时触发死亡
        }
    }

    void Die()
    {
        Debug.Log("Enemy Died!");
        CancelInvoke(); // 停止所有攻击逻辑
        animator.SetTrigger("dead"); // 播放死亡动画
        isBattleStarted = false; // 停止战斗状态
        weaknessTrigger?.SetActive(false); // 确保弱点 Trigger 隐藏
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
