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

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    private void Start()
    {
        base.Start();
        hpText.text = "" + hp;
    }

    private void Update()
    {
        #region movement
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began )
            {
                touchStartPos = touch.position;
            }
            else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;

                float xDiff = (touchEndPos.x - touchStartPos.x) / speed;
                float yDiff = (touchEndPos.y - touchStartPos.y) / speed;

                float x = Mathf.Clamp(transform.position.x + xDiff, -4.5f, 4.5f);
                float y = Mathf.Clamp(transform.position.y + yDiff, -8.5f, 8f);

                gameObject.transform.position = new Vector3(x, y, 0);

                touchStartPos = touch.position;
            }
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
        SceneManager.LoadScene("Game");
    }
}
