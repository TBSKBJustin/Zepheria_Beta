using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FramePickupTrigger : MonoBehaviour
{
    public PlayableDirector timeline; // 引用 Timeline

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家的手部进入 Trigger
        if (other.CompareTag("Left Hand") || other.CompareTag("Right Hand"))
        {
            if (timeline != null)
            {
                timeline.Play(); // 播放 Timeline
            }
        }
    }
}



