using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; } // Instancia única de PlayerData

    public int health; // Salud actual del jugador
    public int maxHealth = 100; // Salud máxima del jugador

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

    // Inicializa la salud del jugador
    public void InitializeHealth()
    {
        health = maxHealth;
    }

    // Aplica daño al jugador
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
    }

    // Cura al jugador
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
    }
}
