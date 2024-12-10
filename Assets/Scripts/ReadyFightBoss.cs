using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyFightBoss : MonoBehaviour
{
    public GameObject enterBattleTrigger;

    public float rotationSpeedX = 30f;
    public float rotationSpeedZ = 30f;

    public AudioClip battleMusic; // ����Ƭ��
    public AudioSource specifiedAudioSource; // ָ���� AudioSource
    private bool hasTriggered = false; // ��ֹ�ظ�����


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
            hasTriggered = true; // ȷ������ֻ����һ��
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
