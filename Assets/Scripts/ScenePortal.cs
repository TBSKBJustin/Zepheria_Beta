using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string targetSceneName; // 要传送的目标场景名称

    private void OnTriggerEnter(Collider other)
    {
        // 检测是否是玩家进入触发区域
        if (other.CompareTag("Player"))
        {
            // 传送到目标场景
            SceneManager.LoadScene(targetSceneName);
        }
    }
}

