using UnityEngine;

public class LootPickup : MonoBehaviour
{
 private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        // 1. სკილის ჩართვა
        collision.GetComponent<OrbitingWisps>()?.ActivateSkill();

        // 2. აფგრეიდების დამატება "ბარაბანში"
        FindObjectOfType<UpgradeManager>()?.UnlockWispUpgrades();
        
        FindObjectOfType<VictoryManager>(true)?.ShowVictory();
        Destroy(gameObject);
    }
}
}