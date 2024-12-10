using System.Collections;
using UnityEngine;

public class SceneAudioController : MonoBehaviour
{
    public AudioClip audioClip1; // 第一段音频
    public AudioClip audioClip2; // 第二段音频
    public GameObject slimes;

    private AudioSource audioSource;

    void Start()
    {
        // 设置 AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // 开始播放音频序列
        StartCoroutine(PlayAudioSequence());
    }

    private IEnumerator PlayAudioSequence()
    {
        // 播放第一段音频
        if (audioClip1 != null)
        {
            audioSource.clip = audioClip1;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip1.length); // 等待第一段音频播放完毕
        }

        // 播放第二段音频
        if (audioClip2 != null)
        {
            slimes.SetActive(true);
            audioSource.clip = audioClip2;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip2.length); // 等待第二段音频播放完毕
        }
    }
}
