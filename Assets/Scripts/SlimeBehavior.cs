using System.Collections;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    public float bounceHeight = 0.5f;  // Height of each bounce
    public float bounceSpeed = 2f;     // Speed of bounce
    public AudioClip bounceSound;      // Sound to play during bouncing
    public AudioClip deathSound;
    //public AudioClip theDoorSound;
    public GameObject Triggers;

    private Vector3 originalPosition;
    private AudioSource audioSource;
    private Coroutine bounceCoroutine;
    public bool isDead = false;

    public GameObject TheDoor;

    void Start()
    {
        // Store the slime's original position
        originalPosition = transform.position;

        // Set up the audio source for the bounce sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bounceSound;
        audioSource.loop = true;

        // Start the bounce coroutine
        StartBouncing();
    }

    private IEnumerator Bounce()
    {
        while (true)
        {
            Vector3 upPosition = new Vector3(originalPosition.x, originalPosition.y + bounceHeight, originalPosition.z);
            Vector3 downPosition = originalPosition;

            yield return MoveToPosition(transform.position, upPosition, 1f / bounceSpeed);
            yield return MoveToPosition(upPosition, downPosition, 1f / bounceSpeed);
        }
    }

    private IEnumerator MoveToPosition(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    private void StartBouncing()
    {
        if (bounceCoroutine == null)
        {
            bounceCoroutine = StartCoroutine(Bounce());
        }

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopBouncing()
    {
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
            bounceCoroutine = null;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = deathSound;
            audioSource.loop = false; // 确保音效只播放一次
            audioSource.Play();
        }
    }



    public void OnPlayerHitWeakness(GameObject weaknessBall)
    {
        if (isDead) return;

        isDead = true;

        // 让 Weakness Ball 消失
        if (weaknessBall != null)
        {
            Destroy(weaknessBall); // 直接销毁弱点球
        }

        StopBouncing();

        // 调用玩家的退出战斗模式和增加血量
        StartCoroutine(ScaleUpAndDestroy());
    }

    private IEnumerator ScaleUpAndDestroy()
    {
        float duration = 1.5f; // 持续时间
        float elapsedTime = 0f;

        Vector3 originalScale = transform.localScale; // 初始大小
        Vector3 targetScale = originalScale * 2f;    // 最终大小，放大到 2 倍

        while (elapsedTime < duration)
        {
            // 插值计算 scale
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保到达最终大小后销毁对象
        transform.localScale = targetScale;
        if (deathSound != null)
        {
            audioSource.clip = deathSound;
            audioSource.loop = false; // 确保音效只播放一次
            audioSource.Play();
        }

        Triggers.SetActive(false);
        CombatMode playerCombatMode = FindObjectOfType<CombatMode>();
        if (playerCombatMode != null)
        {
            playerCombatMode.ExitCombatMode();
            playerCombatMode.IncreaseMaxHealth(20); // 玩家血量上限+20
            playerCombatMode.UpdatePlayerHealthBar();
            //if (playerCombatMode.maxHealth == 105)
            //{
            //    TheDoor.SetActive(true);
            //    audioSource.clip = theDoorSound;
            //    audioSource.Play();
            //}
        }
        Destroy(gameObject);
    }


}
