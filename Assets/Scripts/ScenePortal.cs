using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string targetSceneName; // Ҫ���͵�Ŀ�곡������

    private void OnTriggerEnter(Collider other)
    {
        // ����Ƿ�����ҽ��봥������
        if (other.CompareTag("Player"))
        {
            // ���͵�Ŀ�곡��
            SceneManager.LoadScene(targetSceneName);
        }
    }
}

