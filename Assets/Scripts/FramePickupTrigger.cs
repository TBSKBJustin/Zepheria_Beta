using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FramePickupTrigger : MonoBehaviour
{
    public PlayableDirector timeline; // ���� Timeline

    private void OnTriggerEnter(Collider other)
    {
        // ����Ƿ�����ҵ��ֲ����� Trigger
        if (other.CompareTag("Left Hand") || other.CompareTag("Right Hand"))
        {
            if (timeline != null)
            {
                timeline.Play(); // ���� Timeline
            }
        }
    }
}



