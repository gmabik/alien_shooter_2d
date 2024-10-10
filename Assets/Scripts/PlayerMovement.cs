using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began )
            {
                touchStartPos = touch.position;
            }
            else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;

                float x = touchEndPos.x - touchStartPos.x;
                float y = touchEndPos.y - touchStartPos.y;

                gameObject.transform.position += new Vector3(x, y) * speed;

                touchStartPos = touch.position;
            }
        }
    }
}
