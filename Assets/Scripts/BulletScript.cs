using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour, IDamageable
{
    public GameObject shooter;
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().AddForce(-transform.up * speed, ForceMode2D.Force);
        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<IDamageable>(out IDamageable script) || collision.gameObject == shooter) return;
        script.Damage(damage);
        Destroy(gameObject);
    }

    public void Damage(int damage)
    {
        Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
