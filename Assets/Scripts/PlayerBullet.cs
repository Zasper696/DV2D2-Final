using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int damage; // Da�o que causa la bala del jugador
    public Enemy.DamageType damageType; // Tipo de da�o que causa la bala

    // Se ejecuta cuando la bala colisiona con otro objeto
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, damageType); // Aplica da�o al enemigo
            Destroy(gameObject); // Destruye la bala
        }
    }

    // Establece la direcci�n y velocidad de la bala
    public void SetDirection(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * 10f; // Normaliza la direcci�n y aplica velocidad
    }
}
