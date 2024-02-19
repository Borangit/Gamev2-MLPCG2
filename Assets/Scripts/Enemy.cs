// Enemy.cs
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hitpoints; // Default hitpoints value, you can adjust this in the inspector
    public GameObject explosionEffect;


    public void TakeDamage(int damage)
    {
        hitpoints -= damage;

        if(hitpoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Optional: Add any death animations or effects here

        
        Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        GameObject explosion = Instantiate(explosionEffect, effectPosition, transform.rotation);
        Destroy(gameObject); // Destroy the enemy
        //add money
        CurrencyManager.startingCurrency += 50;
        Destroy(explosion, 3f); // Assumes the effect lasts 5 seconds, adjust as necessary
    }
    public void setHitpoints(int hp)
    {
        hitpoints = hp;
    }
    public void setExplosionEffect(GameObject effect)
    {
        explosionEffect = effect;
    }
}
