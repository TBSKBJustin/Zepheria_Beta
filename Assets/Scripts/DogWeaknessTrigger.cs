using UnityEngine;

public class DogWeaknessTrigger : MonoBehaviour
{
    [Tooltip("Reference to the EnemyCombatController.")]
    public DogCombatController enemyController;

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
                Debug.Log($"{gameObject.name} hit by Sword!");
                enemyController.OnPlayerHitWeakness(gameObject);
            }
            else
            {
                Debug.LogError("WeaknessTrigger: enemyController is null. Cannot process OnPlayerHitWeakness.");
            }
        }
    }
}
