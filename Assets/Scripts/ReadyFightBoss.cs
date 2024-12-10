using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyFightBoss : MonoBehaviour
{
    public GameObject enterBattleTrigger;

    public float rotationSpeedX = 30f;
    public float rotationSpeedZ = 30f;

    public AudioClip battleMusic; // 音乐片段
    public AudioSource specifiedAudioSource; // 指定的 AudioSource
    private bool hasTriggered = false; // 防止重复触发


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
        if (specifiedAudioSource != null)
        {
            if (battleMusic != null)
            {
                specifiedAudioSource.clip = battleMusic;
                specifiedAudioSource.Play();
                Debug.Log("Battle music started on specified AudioSource!");
            }
            else
            {
                Debug.LogWarning("No battle music assigned!");
            }
        }
        else
        {
            Debug.LogError("No AudioSource assigned! Please assign an AudioSource in the Inspector.");
        }
    }
}
