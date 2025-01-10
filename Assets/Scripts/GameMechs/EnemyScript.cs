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

    public override void FixedUpdateNetwork()
    {
        if(Runner.IsServer) transform.position = new(Mathf.Lerp(transform.position.x, placeToGo.x, lerpCoeff), Mathf.Lerp(transform.position.y, placeToGo.y, lerpCoeff), transform.position.z);
    }

    private IEnumerator pathSelection()
    {
        while (Runner.IsServer)
        {
            placeToGo = new(Random.Range(minPosToGo.x, maxPosToGo.x), Random.Range(minPosToGo.y, maxPosToGo.y));
            yield return new WaitForSeconds(1);
        }
    }

    private new IEnumerator Shoot()
    {
        while (Runner.IsServer)
        {
            RpcInstBullet();
            shootCD = Random.Range(minShootCD, maxShootCD);
            yield return new WaitForSeconds(shootCD);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcDamage(int damage)
    {
        hp -= damage;

        if (hp < 1)
        {
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {
        enemyDeath(GetComponent<NetworkObject>());
        yield return null;
    }

    public Action<NetworkObject> enemyDeath;
}
