using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform target;
    [Header("Attributes")]
    public float range = 1f;
    public float turnSpeed = 5f;
    public int cost = 100;
    [Header("Unity Setup Fields")]
    public float fireRate = 1f;
    public string enemyTag = "Enemy";
    private float fireCountdown = 0f;
    

    public GameObject bulletPrefab;
    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null){
            return;
        }
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation,lookRotation,Time.deltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if(fireCountdown <= 0f){
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Dynamically add the Bullet script if it's not attached
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        if (bullet == null)
        {
            bullet = bulletGO.AddComponent<Bullet>();
        }

        bullet.Seek(target);
    }

    void UpdateTarget(){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemy in enemies){
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < range){
                target = enemy.transform;
                break;
            }
            else{
                target = null;
            }
        }
    }
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
