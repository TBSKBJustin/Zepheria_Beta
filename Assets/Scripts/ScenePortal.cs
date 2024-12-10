using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string targetSceneName; // Ҫ���͵�Ŀ�곡������
    public AudioClip audioClip1;   // ��һ����Ƶ
    public AudioClip audioClip2;   // �ڶ�����Ƶ

    private AudioSource audioSource;
    private bool isActivated = false; // ��ֹ�ظ�����

    void Start()
    {
        // ���� AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        StartCoroutine(PlayAudioAndTeleport());
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����Ƿ�����ҽ��봥������
        if (!isActivated && other.CompareTag("Player"))
        {
            isActivated = true;
            //StartCoroutine(PlayAudioAndTeleport());
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private IEnumerator PlayAudioAndTeleport()
    {
        // ���ŵ�һ����Ƶ
        if (audioClip1 != null)
        {
            audioSource.clip = audioClip1;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip1.length); // �ȴ���Ƶ�������
        }

        // ���ŵڶ�����Ƶ
        if (audioClip2 != null)
        {
            audioSource.clip = audioClip2;
            audioSource.Play();
            yield return new WaitForSeconds(audioClip2.length); // �ȴ���Ƶ�������
        }

        // ���͵�Ŀ�곡��
        //SceneManager.LoadScene(targetSceneName);
    }
}
