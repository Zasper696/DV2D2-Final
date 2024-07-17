using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum DamageType { Physical, Fire, Ice, Poison, Magical } // Tipos de da�o posibles

    [SerializeField] private int damage; // Cantidad de da�o que causa la bala
    [SerializeField] private DamageType damageType; // Tipo de da�o que causa la bala

    // Devuelve la cantidad de da�o de la bala
    public int GetDamage()
    {
        return damage;
    }

    // Devuelve el tipo de da�o de la bala
    public DamageType GetDamageType()
    {
        return damageType;
    }
}
