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
        Debug.LogError(gameObject.transform.position.x);
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

                float xDiff = (touchEndPos.x - touchStartPos.x) / speed;
                float yDiff = (touchEndPos.y - touchStartPos.y) / speed;

                float x = Mathf.Clamp(transform.position.x + xDiff, -2.3f, 2.3f);
                float y = Mathf.Clamp(transform.position.y + yDiff, -2.3f, 2.3f);

                gameObject.transform.position = new Vector3(x, y, 0);

                touchStartPos = touch.position;
            }
        }

        /*if(Input.touchCount > 0)
        {
            gameObject.transform.position = Input.GetTouch(0).position / speed;
        }*/
    }
}
