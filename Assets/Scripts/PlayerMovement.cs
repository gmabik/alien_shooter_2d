using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IDamageable
{
    public void Damage(int damage);

    public void Die();
}

public class PlayerMovement : ShootingObj, IDamageable
{
    [SerializeField] private int hp;
    [SerializeField] private TMP_Text hpText;

    [SerializeField] private float speed;

    private float horizontal;
    private float vertical;

    private new void Start()
    {
        base.Start();
        hpText.text = "" + hp;
    }

    private void Update()
    {
        #region input
        horizontal = Input.GetAxisRaw("Horizontal") * speed;
        vertical = Input.GetAxisRaw("Vertical") * speed;

        #endregion input

    }

    private void FixedUpdate()
    {
        #region movement
        float x = Mathf.Clamp(transform.position.x + horizontal, -15f, 15f);
        float y = Mathf.Clamp(transform.position.y + vertical, -8.5f, 0f);

        gameObject.transform.position = new Vector3(x, y, 0);
        #endregion movement
    }

    public void Damage(int damage)
    {
        hp -= damage;
        hpText.text = "" + hp;
        if (hp < 1)
        {
            Die();
        }
    }

    public void Die()
    {
        SceneManager.LoadScene("Game");
    }
}
