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
        if (!vp.isPlaying)
        {
            SceneManager.LoadScene("CinematicMainMenu");
        }
    }
}
