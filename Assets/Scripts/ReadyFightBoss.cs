using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyFightBoss : MonoBehaviour
{
    public GameObject enterBattleTrigger;

    public float rotationSpeedX = 30f;
    public float rotationSpeedZ = 30f;

    public AudioClip battleMusic; // 音乐片段
    private AudioSource audioSource; // 音频组件
    private bool hasTriggered = false; // 防止重复触发

    void Start()
    {
        // 添加 AudioSource 组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // 不自动播放
    }

    void Update()
    {
        if (enterBattleTrigger != null)
        {
            enterBattleTrigger.transform.Rotate(new Vector3(rotationSpeedX, 0, rotationSpeedZ) * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true; // 确保音乐只播放一次
            PlayBattleMusic();
            enterBattleTrigger.SetActive(false);
        }
    }

    void PlayBattleMusic()
    {
        if (battleMusic != null)
        {
            audioSource.clip = battleMusic;
            audioSource.Play();
            Debug.Log("Battle music started!");
        }
        else
        {
            Debug.LogWarning("No battle music assigned!");
        }
    }
}
