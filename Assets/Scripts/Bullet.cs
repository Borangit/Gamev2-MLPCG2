using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    public GameObject impactEffect;

    public float speed = 30f;

    public int damage = 50;

    public void Seek(Transform _target){
        target = _target;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null){
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame){
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);

    }

    void HitTarget(){
        // Define an offset position
        Vector3 effectPosition = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

        // Instantiate the explosion effect at the offset position with the same rotation as the bullet
        GameObject explosionEffect = Instantiate(impactEffect, effectPosition, transform.rotation);

        // Optionally destroy the explosion effect after it finishes playing if it doesn't destroy itself
        Destroy(explosionEffect, 3f); // Assumes the effect lasts 5 seconds, adjust as necessary

        Destroy(gameObject); // Destroy the bullet
        DamageEnemy(target);
        
    }

    void DamageEnemy(Transform enemy){
        Enemy e = enemy.GetComponent<Enemy>();

        if(e != null){
            e.TakeDamage(damage);
        }
    }

}
