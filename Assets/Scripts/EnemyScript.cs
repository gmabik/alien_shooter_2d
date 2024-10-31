using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyScript : ShootingObj, IDamageable
{
    [SerializeField] private int hp;

    [SerializeField] private Vector2 placeToGo;
    [SerializeField] private float lerpCoeff;

    [SerializeField] private float minShootCD;
    [SerializeField] private float maxShootCD;

    public new void Start()
    {
        StartCoroutine(Shoot());
        StartCoroutine(pathSelection());
    }

    private void Update()
    {
        transform.position = new(Mathf.Lerp(transform.position.x, placeToGo.x, lerpCoeff), Mathf.Lerp(transform.position.y, placeToGo.y, lerpCoeff), transform.position.z);
    }

    private IEnumerator pathSelection()
    {
        while (true)
        {
            placeToGo = new(Random.Range(-5f, 5f), Random.Range(2.75f, 8.75f));
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
        Destroy(gameObject);
    }

}
