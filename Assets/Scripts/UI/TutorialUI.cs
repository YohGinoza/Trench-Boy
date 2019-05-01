using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [Header("Fading Setting")]
    [SerializeField] private Image[] Elements;
    [SerializeField] private Color NormalColor;
    [SerializeField] private Color FadedColor;
    public float FadeTime = 0.1f;

    private bool fading = false;
    public bool showing = false;
    private Tutorials showingTutorial = Tutorials.PickUpCrate;

    [SerializeField] private Sprite[] TutorialSprite = new Sprite[7];
    [SerializeField] private Image Holder;

    [SerializeField] private CameraController cameraController;

    private void Start()
    {
        if (cameraController == null)
        {
            cameraController = FindObjectOfType<CameraController>();
        }
    }

    private void SetTutorial(Tutorials tutorial)
    {
        Holder.sprite = TutorialSprite[(int)tutorial];
        showingTutorial = tutorial;
    }

    public void TurnOn(Tutorials thistutorial,Transform CamTarget = null)
    {
        if (!showing)
        {
            showing = true;
            SetTutorial(thistutorial);
            this.gameObject.SetActive(true);

            if (CamTarget != null)
            {
                cameraController.SetTarget(CamTarget);
            }

            StartCoroutine(FadeInOut(true));
            Time.timeScale = 0.5f;
        }
    }

    public void TurnOff()
    {
        if (showing)
        {
            //showing = false;
            StartCoroutine(FadeInOut(false));
            //Time.timeScale = 1.0f;
            //GameController.TutorialFinished[(int)showingTutorial] = true;
        }
    }

    IEnumerator FadeInOut(bool toBlack)
    {
        if (!fading)
        {
            fading = true;
            float timer = 0;
            while (timer < FadeTime)
            {
                yield return new WaitForSecondsRealtime(0.02f);
                timer += 0.02f;

                //turn on
                if (toBlack)
                {
                    foreach (Image fader in Elements)
                    {
                        fader.color = Color.Lerp(NormalColor, FadedColor, timer / FadeTime);
                    }
                }
                //turn off
                else
                {
                    foreach (Image fader in Elements)
                    {
                        fader.color = Color.Lerp(NormalColor, FadedColor, 1 - (timer / FadeTime));
                    }
                }
            }
            if (!toBlack)
            {
                this.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
                showing = false;
                GameController.TutorialFinished[(int)showingTutorial] = true;
                cameraController.SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
            }
            fading = false;
        }
        else
        {
            yield return null;
        }
    }
}
