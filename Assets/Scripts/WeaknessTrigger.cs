using UnityEngine;

public class WeaknessTrigger : MonoBehaviour
{
    [Tooltip("Reference to the EnemyCombatController.")]
    public EnemyCombatController enemyController;

    void Start()
    {
        if (enemyController == null)
        {
            Debug.LogError("WeaknessTrigger: Missing reference to EnemyCombatController! Please assign it in the Inspector.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            if (enemyController != null)
            {
                enemyController.OnPlayerHitWeakness();
            }
            else
            {
                Debug.LogError("WeaknessTrigger: enemyController is null. Cannot process OnPlayerHitWeakness.");
            }
        }
    }
}
