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

    private bool inDialogue = false;
    
    public bool zoomIn = false;
    public bool zoomOut = false;

    float DefaultSize;

    private void Start()
    {
        DefaultSize = this.GetComponent<Camera>().orthographicSize;
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
        

        if (zoomIn && !zoomOut)
        {

           

            if (this.GetComponent<Camera>().orthographicSize > 2.4f)
            {
                this.GetComponent<Camera>().orthographicSize -= 0.2f;
            }
            else
            {
                zoomIn = false;
            }
        }

        if (zoomOut && !zoomIn)
        {

            if (this.GetComponent<Camera>().orthographicSize < DefaultSize)
            {
                this.GetComponent<Camera>().orthographicSize += 0.2f;
            }
            else
            {
                zoomOut = false;
            }
        }

    }

    void LookAtTarget()
    {
        Vector3 idealPosition;

        if (inDialogue)
        {
            //move camera up a little bit
            idealPosition = new Vector3(Target.position.x, Target.position.y + 1.0f + (Mathf.Sin(Mathf.Deg2Rad * this.transform.eulerAngles.x) * 30), Target.position.z - (Mathf.Cos(Mathf.Deg2Rad * this.transform.eulerAngles.x) * 30));

        }
        else
        {
            //center
            idealPosition = new Vector3(Target.position.x, Target.position.y + (Mathf.Sin(Mathf.Deg2Rad * this.transform.eulerAngles.x) * 30), Target.position.z - (Mathf.Cos(Mathf.Deg2Rad * this.transform.eulerAngles.x) * 30));

        }

        this.transform.position = Vector3.SmoothDamp(this.transform.position, idealPosition, ref moveRef, SmoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        Target = newTarget;
    }

    public void DialogueZoomIn()
    {

        if (!inDialogue)
        {
            zoomIn = true;
            zoomOut = false;
            inDialogue = true;
            //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);
        }

    }

    public void DialogueZoomOut()
    {
        //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1.5f, this.transform.position.z);

        //this.GetComponent<Camera>().orthographicSize = 7.0f;

        zoomIn = false;
        zoomOut = true;

        inDialogue = false;
    }

    public IEnumerator FadeInOut(bool toBlack)
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
