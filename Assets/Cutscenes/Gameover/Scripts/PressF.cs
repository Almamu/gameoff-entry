using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressF : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey (KeyCode.F) == true)
        {
            SceneManager.LoadScene ("Playground");
        }
    }
}
