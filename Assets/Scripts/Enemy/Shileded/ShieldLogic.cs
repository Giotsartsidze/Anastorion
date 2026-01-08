using UnityEngine;

public class ShieldLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // დარწმუნდი, რომ შენს ტყვიებს (Stellar Darts) აქვთ Tag: "Projectile"
        if (collision.CompareTag("Projectile")) 
        {
            Debug.Log("Bullet Blocked!"); // ამით მივხვდებით, რომ ფარი მუშაობს
            
            // ვანადგურებთ ტყვიას
            Destroy(collision.gameObject);
        }
    }
}