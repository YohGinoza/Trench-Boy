using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private GameController gameController;

    [Header("Fading Setting")]
    [SerializeField] private Image Fader;
    [SerializeField] private Color NormalColor;
    [SerializeField] private Color FadedColor;
    public float FadeTime = 1;

    [Header("Target Setting")]
    [SerializeField] private float SmoothTime = 0;
    private Vector3 moveRef = Vector3.zero;

    [Header("General Setting")]
    [SerializeField] private Transform Target;

    private bool fading = false;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    private void FixedUpdate()
    {
        switch (gameController.CurrentState)
        {
            case GameState.Day:
            case GameState.Stalling:
                break;
            case GameState.Night:
                break;
        }

        LookAtTarget();
    }

    void LookAtTarget()
    {
        //center
        Vector3 idealPosition = new Vector3(Target.position.x, Target.position.y + (Mathf.Sin(Mathf.Deg2Rad * this.transform.eulerAngles.x) * 30), Target.position.z - (Mathf.Cos(Mathf.Deg2Rad * this.transform.eulerAngles.x) * 30));

        this.transform.position = Vector3.SmoothDamp(this.transform.position, idealPosition, ref moveRef, SmoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        Target = newTarget;
    }

    IEnumerator FadeInOut(bool toBlack)
    {
        if (!fading)
        {
            fading = true;
            float timer = 0;
            while (timer < FadeTime)
            {
                yield return new WaitForFixedUpdate();
                timer += Time.deltaTime;

                if (toBlack)
                {
                    Fader.color = Color.Lerp(NormalColor, FadedColor, timer / FadeTime);
                }
                else
                {
                    Fader.color = Color.Lerp(NormalColor, FadedColor, 1 - (timer / FadeTime));
                }
            }
            fading = false;
        }
        else
        {
            yield return null;
        }
    }
}
