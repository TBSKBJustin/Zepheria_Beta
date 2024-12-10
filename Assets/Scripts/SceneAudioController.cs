using System.Collections;
using UnityEngine;

public class SceneAudioController : MonoBehaviour
{
    public AudioClip audioClip1; // ��һ����Ƶ
    public AudioClip audioClip2; // �ڶ�����Ƶ
    public GameObject slimes;

    private AudioSource audioSource;

    void Start()
    {
        // ���� AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // ��ʼ������Ƶ����
        StartCoroutine(PlayAudioSequence());
    }

    private IEnumerator PlayAudioSequence()
    {
        // ���ŵ�һ����Ƶ
        if (audioClip1 != null)
        {
            audioSource.clip = audioClip1;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip1.length); // �ȴ���һ����Ƶ�������
        }

        // ���ŵڶ�����Ƶ
        if (audioClip2 != null)
        {
            slimes.SetActive(true);
            audioSource.clip = audioClip2;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip2.length); // �ȴ��ڶ�����Ƶ�������
        }
    }
}
