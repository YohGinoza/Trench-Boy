using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StartUp : MonoBehaviour
{
    [SerializeField] private VideoPlayer vp;

    private void LateUpdate()
    {
        if (!vp.isPlaying && Time.time > 10 || Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("CinematicMainMenu");
        }
    }
}
