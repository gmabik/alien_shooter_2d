using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour, IDamageable
{
    public GameObject shooter;
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    public NetworkRunner runner;
    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().AddForce(-transform.up * speed, ForceMode2D.Force);
        StartCoroutine(Despawn());
        StartCoroutine(DestroyIfStill());
    }

    private IEnumerator DestroyIfStill()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            print(GetComponent<Rigidbody2D>().velocity.sqrMagnitude);
            if (GetComponent<Rigidbody2D>().velocity.sqrMagnitude < 0.1f && GetComponent<Rigidbody2D>().velocity.sqrMagnitude > -0.1f) Destroy(gameObject);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<IDamageable>(out IDamageable script) || collision.gameObject == shooter) return;
        if(runner.IsServer) script.RpcDamage(damage);
        Destroy(gameObject);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcDamage(int damage)
    {
        StartCoroutine(Die());
    }

    public IEnumerator Die()
    {
        Destroy(gameObject);
        yield return null;
    }
}
