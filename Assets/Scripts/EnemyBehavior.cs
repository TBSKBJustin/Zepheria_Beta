using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyBehavior : MonoBehaviour
    {
        private Animator _animator;

        private static readonly int GotHit = Animator.StringToHash("GotHit");
        private static readonly int HitSourceRight = Animator.StringToHash("HitSourceRight");

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // ��ȡ�����ߵ����
            var damager = other.gameObject.GetComponent<Damager>();
            if (damager == null) return;

            // �жϹ�������
            var hitPosition = other.ClosestPoint(transform.position);
            float fromRight = FromRight(hitPosition);

            // ���ö�������
            _animator.SetFloat(HitSourceRight, fromRight);
            _animator.SetTrigger(GotHit);
        }

        private void OnTriggerExit(Collider other)
        {
            // ���ö���Trigger
            _animator.ResetTrigger(GotHit);
        }

        private float FromRight(Vector3 otherPosition)
        {
            Vector3 right = transform.right;
            Vector3 directionToOther = (otherPosition - transform.position).normalized;

            // ������transform.right��Dotֵ����ӳ�䵽0-1֮��
            float dot = Vector3.Dot(right, directionToOther);
            return Mathf.Clamp01((dot / 2f) + 0.5f);
        }

        private float FromFront(Vector3 otherPosition)
        {
            Vector3 forward = transform.forward;
            Vector3 directionToOther = (otherPosition - transform.position).normalized;

            // ������transform.forward��Dotֵ����ӳ�䵽0-1֮��
            float dot = Vector3.Dot(forward, directionToOther);
            return Mathf.Clamp01((dot / 2f) + 0.5f);
        }
    }

    public class Damager : MonoBehaviour
    {
        // ʾ����Damager�࣬���Ը�����Ҫ��չ
    }
}

