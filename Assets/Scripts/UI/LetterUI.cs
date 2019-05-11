using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterUI : MonoBehaviour
{
    private bool recieved = false;      //the envelope is in place?
    private bool opened = false;        //player opened the envelope?
    private bool openning = false;      //letter being opened prevent double openning
    private bool reading = false;       //player read(toggle text) the lettes?
    private bool openningReader = false;//text being faded prevent double reading

    [SerializeField] private Image envelope;
    [SerializeField] private float envelopeSlideTime = 0.5f;
    private Vector2 refvel = Vector2.zero;

    [SerializeField] private Sprite[] LetterImages = new Sprite[5];
    [SerializeField] private Image Letter;

    [SerializeField] private string[] Messages = new string[5];
    [SerializeField] private Text ReadingText;

    [SerializeField] private Image ReaderBGPanel;
    [SerializeField] private Color ReaderBGDefaultColor;

    [SerializeField] private Text InstructorText;

    [Range(0, 0.5f)] [SerializeField] private float FadeTime = 0.2f;

    private void FixedUpdate()
    {
        if (recieved)
        {
            if (!opened && !openning)
            {
                //click mouse or press e
                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(Open());
                }
            }
            else
            {
                if (!reading && !openningReader)
                {
                    //pressed e read
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(Read());
                    }
                }
                else
                {
                    //pressed e unread
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        StartCoroutine(UnRead());
                    }
                }
            }
            //pressed q dissmiss
            if (Input.GetKeyDown(KeyCode.F))
            {
                Dissmiss();
            }
        }
    }

    void Dissmiss()
    {
        recieved = false;
        //remove envelop
        envelope.rectTransform.position = new Vector3(-2000, 0, 0);
        //remove instruction text
        InstructorText.enabled = false;

        //remove letter
        StartCoroutine(ThrowLetter());

        //remove reader
        StartCoroutine(UnRead());
    }

    public IEnumerator RecieveLetter(int day)
    {
        //show instructor
        InstructorText.enabled = true;

        //setup letter
        ReadingText.text = Messages[day];
        Letter.sprite = LetterImages[day];

        //setup envelope
        envelope.rectTransform.anchoredPosition = new Vector3(-2000, 0, 0);
        while (envelope.rectTransform.anchoredPosition.x < 0)
        {
            yield return new WaitForFixedUpdate();
            envelope.rectTransform.anchoredPosition = Vector2.SmoothDamp(envelope.rectTransform.anchoredPosition, Vector2.right * 10.0f, ref refvel, envelopeSlideTime);
        }

        recieved = true;
    }

    IEnumerator Open()
    {
        openning = true;
        float alpha = 0;
        while (Letter.color.a < 1)
        {
            yield return new WaitForFixedUpdate();
            alpha += Time.fixedDeltaTime / FadeTime;
            Letter.color = new Color(1, 1, 1, alpha);
        }

        openning = false;
        opened = true;
    }

    IEnumerator ThrowLetter()
    {
        opened = false;

        float alpha = ReaderBGPanel.color.a;
        while (Letter.color.a > 0)
        {
            yield return new WaitForFixedUpdate();
            alpha -= Time.fixedDeltaTime / FadeTime;
            Letter.color = new Color(ReaderBGDefaultColor.r, ReaderBGDefaultColor.g, ReaderBGDefaultColor.b, alpha);
        }
    }

    IEnumerator Read()
    {
        float alpha = 0;
        while (ReaderBGPanel.color.a < ReaderBGDefaultColor.a)
        {
            yield return new WaitForFixedUpdate();
            alpha += Time.fixedDeltaTime / FadeTime;
            ReaderBGPanel.color = new Color(ReaderBGDefaultColor.r, ReaderBGDefaultColor.g, ReaderBGDefaultColor.b, alpha);
        }

        //open text
        ReadingText.enabled = true;

        reading = true;
    }

    IEnumerator UnRead()
    {
        //close text
        ReadingText.enabled = false;

        float alpha = ReaderBGPanel.color.a;
        while (ReaderBGPanel.color.a > 0)
        {
            yield return new WaitForFixedUpdate();
            alpha -= Time.fixedDeltaTime / FadeTime;
            ReaderBGPanel.color = new Color(ReaderBGDefaultColor.r, ReaderBGDefaultColor.g, ReaderBGDefaultColor.b, alpha);
        }

        reading = false;
    }
}
