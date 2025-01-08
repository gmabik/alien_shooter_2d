using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootingObj : NetworkBehaviour
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected float shootCD;

    protected void Start()
    {
        StartCoroutine(Shoot());
    }

    protected IEnumerator Shoot()
    {
        while (true)
        {
            InstBullet();
            yield return new WaitForSeconds(shootCD);
        }
    }

    protected void InstBullet() 
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = transform.position + (transform.up * (gameObject.GetComponent<EnemyScript>() == null ? 1 : -1));
        bullet.transform.eulerAngles = bulletPrefab.transform.eulerAngles + transform.eulerAngles;
        bullet.GetComponent<BulletScript>().shooter = gameObject;
    }
}
