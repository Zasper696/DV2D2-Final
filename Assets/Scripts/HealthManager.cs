// HealthManager.cs
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; } // Instancia única de HealthManager

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Aplica daño al jugador
    public void ApplyDamageToPlayer(int damage, Player.DamageType damageType)
    {
        Player.Instance.TakeDamage(damage, damageType); // Llama al método TakeDamage del jugador
    }

    // Aplica daño al enemigo
    public void ApplyDamageToEnemy(GameObject enemy, int damage, Enemy.DamageType damageType)
    {
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(damage, damageType); // Llama al método TakeDamage del enemigo
        }
    }
}
