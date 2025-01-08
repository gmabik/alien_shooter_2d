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
    public TMP_Text hpText;

    [SerializeField] private float speed;

    private float horizontal;
    private float vertical;

    private new void Start()
    {
        base.Start();
        hpText = LobbyManager.Instance.hpText;
        hpText.text = "" + hp;
    }

    private void Update()
    {
        //#region input
        //if (HasInputAuthority)
        //{
        //    horizontal = Input.GetAxisRaw("Horizontal") * speed;
        //    vertical = Input.GetAxisRaw("Vertical") * speed;
        //}

        //#endregion input
    }

    public override void FixedUpdateNetwork()
    {
        #region movement
        if (GetInput(out NetworkInputData data))
        {
            float x = Mathf.Clamp(transform.position.x + (data.movementInput.x * speed), -15f, 15f);
            float y = Mathf.Clamp(transform.position.y + (data.movementInput.y * speed), -8.5f, 0f);

            gameObject.transform.position = new Vector3(x, y, 0);
        }
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
        Application.Quit();
    }
}
