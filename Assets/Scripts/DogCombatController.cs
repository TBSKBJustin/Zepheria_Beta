using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DogCombatController : MonoBehaviour
{
    [Header("References")]
    public Animator animator; // 敌人的 Animator
    public DodgeTrigger leftTrigger;  // 左边闪避区域
    public DodgeTrigger midTrigger;   // 中间闪避区域
    public DodgeTrigger rightTrigger; // 右边闪避区域


    [Header("Action UI Sprites")]
    public Image rightActionSprite;
    public Image midActionSprite;
    public Image leftActionSprite;

    [Header("Weakness Balls")]
    public GameObject[] weaknessBalls;  // 根据 attackSet 对应的弱点球

    [Header("Health Settings")]
    public int maxHealth = 100; // 怪物最大血量
    public int EnemyDamage;     // 怪物对玩家的伤害
    [SerializeField] private Image _healthbarSprite;

    [Header("Attack Settings")]
    public float attackInterval = 2f; // 攻击间隔（攻击间的冷却时间）
    private readonly int[][] attackCombinations =
    {
        new int[] { 1, 2},
        new int[] { 2, 1}
    };

    private int currentHealth;
    private bool isBattleStarted = false;
    private bool inWeaknessState = false;
    private int attackSet = 0;     // 当前攻击序列的组合
    private int attackActionIndex = 0;
    private int attackCount = 0;   // 当前已进行的攻击次数

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
            // 启动攻击流程主协程
            StartCoroutine(AttackLoop());
        }
    }

    /// <summary>
    /// 主攻击循环协程：  
    /// 1. 随机选择攻击组合  
    /// 2. 连续进行3次攻击（根据 attackSet 确定攻击模式）  
    /// 3. 进入弱点状态，让玩家有机会打断  
    /// 4. 如果玩家未打断则恢复新的攻击循环
    /// </summary>
    IEnumerator AttackLoop()
    {
        while (isBattleStarted)
        {
            // 随机选择一个攻击组合
            attackSet = Random.Range(0, attackCombinations.Length);
            attackActionIndex = 0;
            attackCount = 0;

            // 执行3次普通攻击
            for (int i = 0; i < 2; i++)
            {
                if (!isBattleStarted) yield break;
                yield return StartCoroutine(PerformSingleAttack());
            }

            // 3次攻击后进入弱点状态
            yield return StartCoroutine(HandleWeaknessPhase());
        }
    }

    /// <summary>
    /// 执行单次攻击流程的协程：  
    /// - 显示攻击指令UI  
    /// - 触发动画  
    /// - 等待攻击间隔  
    /// - 检测玩家闪避是否成功  
    /// </summary>
    IEnumerator PerformSingleAttack()
    {
        if (inWeaknessState) yield break; // 若在弱点状态中不进行普通攻击

        int attackAction = attackCombinations[attackSet][attackActionIndex];
        ShowActionSprite(attackAction);
        animator.SetTrigger($"attack{attackAction}");
        attackCount++;

        // 等待攻击间隔完成后进行闪避判定
        yield return new WaitForSeconds(attackInterval);

        CheckDodgeSuccess(attackAction);
        DisableActionSprites();

        // 准备下一个动作序列
        attackActionIndex = (attackActionIndex + 1) % 2;
    }

    /// <summary>
    /// 弱点状态协程：  
    /// 进入弱点状态时：  
    /// - 慢速播放动画  
    /// - 显示对应的弱点球  
    /// - 等待玩家打击弱点或超时后恢复正常状态
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

        // 等待玩家在3秒内攻击弱点
        float weaknessDuration = 3f;
        float timer = 0f;
        bool weaknessHit = false;

        while (timer < weaknessDuration)
        {
            if (!inWeaknessState)
            {
                // 如果在弱点窗口内玩家打中了弱点（通过 OnPlayerHitWeakness 调用），会提前退出弱点状态
                weaknessHit = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 如果玩家未打击弱点则返回普通攻击循环
        if (inWeaknessState)
        {
            // 玩家没有命中弱点球
            if (!weaknessHit)
            {
                Debug.Log("Player missed Weakness!");
                CheckDodgeSuccess(attackCombinations[attackSet][attackActionIndex]);
            }

            // 重置状态
            DisableActionSprites();
            DisableWeaknessBalls();
            animator.SetTrigger("idle");
            animator.speed = 1f;
            inWeaknessState = false;

            // 重新开始新的攻击循环时会重新在 AttackLoop 中选择 attackSet
        }
    }

    /// <summary>
    /// 当玩家击中弱点时调用此方法。  
    /// 外部逻辑：请在玩家攻击弱点球的逻辑中调用本方法。
    /// </summary>
    public void OnPlayerHitWeakness(GameObject hitBall)
    {
        if (inWeaknessState)
        {
            Debug.Log($"Player hit Weakness on {hitBall.name}!");
            // 重置弱点状态
            hitBall.SetActive(false);
            inWeaknessState = false;
            animator.SetTrigger("HitReact");
            animator.speed = 1f;

            // 给敌人造成伤害
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

            // 对玩家造成伤害
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
        DisableActionSprites(); // 先隐藏所有图片
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

        // 通知玩家退出战斗模式
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
