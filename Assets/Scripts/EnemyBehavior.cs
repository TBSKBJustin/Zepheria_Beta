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
            // 获取攻击者的组件
            var damager = other.gameObject.GetComponent<Damager>();
            if (damager == null) return;

            // 判断攻击方向
            var hitPosition = other.ClosestPoint(transform.position);
            float fromRight = FromRight(hitPosition);

            // 设置动画参数
            _animator.SetFloat(HitSourceRight, fromRight);
            _animator.SetTrigger(GotHit);
        }

        private void OnTriggerExit(Collider other)
        {
            // 重置动画Trigger
            _animator.ResetTrigger(GotHit);
        }

        private float FromRight(Vector3 otherPosition)
        {
            Vector3 right = transform.right;
            Vector3 directionToOther = (otherPosition - transform.position).normalized;

            // 计算与transform.right的Dot值，并映射到0-1之间
            float dot = Vector3.Dot(right, directionToOther);
            return Mathf.Clamp01((dot / 2f) + 0.5f);
        }

        private float FromFront(Vector3 otherPosition)
        {
            Vector3 forward = transform.forward;
            Vector3 directionToOther = (otherPosition - transform.position).normalized;

            // 计算与transform.forward的Dot值，并映射到0-1之间
            float dot = Vector3.Dot(forward, directionToOther);
            return Mathf.Clamp01((dot / 2f) + 0.5f);
        }
    }

    public class Damager : MonoBehaviour
    {
        // 示例的Damager类，可以根据需要扩展
    }
}

