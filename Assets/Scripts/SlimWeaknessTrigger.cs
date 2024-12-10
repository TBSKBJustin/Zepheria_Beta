using UnityEngine;

public class SlimWeaknessTrigger : MonoBehaviour
{
    public SlimeBehavior slimeController;

    void Start()
    {
        if (slimeController == null)
        {
            Debug.LogError("WeaknessTrigger: Missing reference to SlimeBehavior! Please assign it in the Inspector.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            if (slimeController != null)
            {
                Debug.Log($"{gameObject.name} hit by Sword!");
                slimeController.OnPlayerHitWeakness(gameObject); // 传入当前弱点球对象
            }
            else
            {
                Debug.LogError("WeaknessTrigger: slimeController is null. Cannot process OnPlayerHitWeakness.");
            }
        }
    }

}
