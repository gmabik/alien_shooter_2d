using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public interface IDamageable
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcDamage(int damage);

    public IEnumerator Die();
}

public class PlayerMovement : ShootingObj, IDamageable
{
    [SerializeField] private int hp;
    public TMP_Text hpText;
    [SerializeField] private GameObject playerPointer;

    [SerializeField] private float speed;

    private float horizontal;
    private float vertical;

    private new void Start()
    {
        base.Start();
        hpText = LobbyManager.Instance.hpText;
        hpText.text = "" + hp;
        if(HasInputAuthority) playerPointer.SetActive(true);
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcDamage(int damage)
    {
        hp -= damage;

        if (HasInputAuthority) hpText.text = "" + hp;

        if (hp < 1)
        {
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {
        if (!Runner.IsServer)
        {
            Runner.Shutdown();
            SceneManager.LoadScene("Menu");
        }
        else if(!HasInputAuthority) Runner.Despawn(Object);

        yield return new WaitForSeconds(0.3f);
        if (HasInputAuthority)
        {
            Runner.Shutdown();
            SceneManager.LoadScene("Menu");
        }
    }
}
