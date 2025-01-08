using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyScript : ShootingObj, IDamageable
{
    [SerializeField] private int hp;

    public bool isBoss;
    [SerializeField] private Vector2 minPosToGo;
    [SerializeField] private Vector2 maxPosToGo;
    private Vector2 placeToGo;
    [SerializeField] private float lerpCoeff;

    [SerializeField] private float minShootCD;
    [SerializeField] private float maxShootCD;

    public new void Start()
    {
        StartCoroutine(Shoot());
        StartCoroutine(pathSelection());
    }

    private void FixedUpdate()
    {
        transform.position = new(Mathf.Lerp(transform.position.x, placeToGo.x, lerpCoeff), Mathf.Lerp(transform.position.y, placeToGo.y, lerpCoeff), transform.position.z);
    }

    private IEnumerator pathSelection()
    {
        while (true)
        {
            placeToGo = new(Random.Range(minPosToGo.x, maxPosToGo.x), Random.Range(minPosToGo.y, maxPosToGo.y));
            yield return new WaitForSeconds(1);
        }
    }

    private new IEnumerator Shoot()
    {
        while (true)
        {
            InstBullet();
            shootCD = Random.Range(minShootCD, maxShootCD);
            yield return new WaitForSeconds(shootCD);
        }
    }

    public void Damage(int damage)
    {
        hp -= damage;

        if (hp < 1)
        {
            Die();
        }
    }

    public void Die()
    {
        enemyDeath(GetComponent<NetworkObject>());
    }

    public Action<NetworkObject> enemyDeath;
}
