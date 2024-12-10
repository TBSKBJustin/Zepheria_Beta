using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyFightBoss : MonoBehaviour
{
    public GameObject enterBattleTrigger;

    public float rotationSpeedX = 30f;
    public float rotationSpeedZ = 30f;

    public AudioClip battleMusic; // ����Ƭ��
    private AudioSource audioSource; // ��Ƶ���
    private bool hasTriggered = false; // ��ֹ�ظ�����

    void Start()
    {
        // ��� AudioSource ���
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // ���Զ�����
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
            hasTriggered = true; // ȷ������ֻ����һ��
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
