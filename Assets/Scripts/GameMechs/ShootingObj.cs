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
        while (Runner.IsServer)
        {
            RpcInstBullet();
            yield return new WaitForSeconds(shootCD);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void RpcInstBullet() 
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = transform.position + (transform.up * (gameObject.GetComponent<EnemyScript>() == null ? 1 : -1));
        bullet.transform.eulerAngles += transform.eulerAngles;
        bullet.GetComponent<BulletScript>().shooter = gameObject;
        bullet.GetComponent<BulletScript>().runner = Runner;
    }
}
