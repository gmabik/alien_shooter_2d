using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void Update()
    {
        if (Input.touchCount > 0) SceneManager.LoadScene("Game");
    }
}
