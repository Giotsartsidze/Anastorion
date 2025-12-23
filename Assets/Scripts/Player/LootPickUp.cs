using UnityEngine;

public class LootPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ვპოულობთ სქილს მოთამაშეზე და ვრთავთ
            OrbitingWisps skill = collision.GetComponent<OrbitingWisps>();
            if (skill != null)
            {
                skill.ActivateSkill();
            }
            
            // აქ შეგიძლია დაამატო აღების ხმა/ეფექტი
            Destroy(gameObject);
        }
    }
}