using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string targetSceneName; // 要传送的目标场景名称
    public AudioClip audioClip1;   // 第一段音频
    public AudioClip audioClip2;   // 第二段音频

    private AudioSource audioSource;
    private bool isActivated = false; // 防止重复触发

    void Start()
    {
        // 设置 AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        StartCoroutine(PlayAudioAndTeleport());
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检测是否是玩家进入触发区域
        if (!isActivated && other.CompareTag("Player"))
        {
            isActivated = true;
            //StartCoroutine(PlayAudioAndTeleport());
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private IEnumerator PlayAudioAndTeleport()
    {
        // 播放第一段音频
        if (audioClip1 != null)
        {
            audioSource.clip = audioClip1;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip1.length); // 等待音频播放完毕
        }

        // 播放第二段音频
        if (audioClip2 != null)
        {
            audioSource.clip = audioClip2;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip2.length); // 等待音频播放完毕
        }

        // 传送到目标场景
        //SceneManager.LoadScene(targetSceneName);
    }
}
