using UnityEngine;

public class LootPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. ვრთავთ ახალ სკილს (Wisps)
            OrbitingWisps skill = collision.GetComponent<OrbitingWisps>();
            if (skill != null) skill.ActivateSkill();

            // 2. ვპოულობთ Victory UI-ს და ვრთავთ
            VictoryManager victory = FindObjectOfType<VictoryManager>(true); // (true) ნიშნავს რომ გათიშულსაც იპოვის
            if (victory != null) victory.ShowVictory();

            Debug.Log("BOSS LOOT CLAIMED! VICTORY!");
            
            // 3. ვაქრობთ ნივთს
            Destroy(gameObject);
        }
    }
}