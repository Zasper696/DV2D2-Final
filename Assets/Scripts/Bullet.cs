using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum DamageType { Physical, Fire, Ice, Poison, Magical } // Tipos de daño posibles

    [SerializeField] private int damage; // Cantidad de daño que causa la bala
    [SerializeField] private DamageType damageType; // Tipo de daño que causa la bala

    // Devuelve la cantidad de daño de la bala
    public int GetDamage()
    {
        return damage;
    }

    // Devuelve el tipo de daño de la bala
    public DamageType GetDamageType()
    {
        return damageType;
    }
}
