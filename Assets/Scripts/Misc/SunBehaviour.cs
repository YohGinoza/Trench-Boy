using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunBehaviour : MonoBehaviour
{
    private GameController gameController;
    [SerializeField] private Color DayLightColor;
    [SerializeField] private Color DawnLightColor;
    [SerializeField] private Color NightLightColor;
    [Range(0, 1)] [SerializeField] private float BrightestMorningTime;
    [Range(0, 1)] [SerializeField] private float BrightestAfterNoonTime;
    [Range(0, 1)] [SerializeField] private float DawnTime;

    [SerializeField]private Color CurrentLightColor;
    [Range(0,1)][SerializeField]float MappedTime = 0;
    //[SerializeField]private Vector3 SunAxis;
    private Light SunLight;

    Quaternion original;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        SunLight = this.GetComponent<Light>();
        //SunAxis = this.transform.forward;
        original = this.transform.rotation;
    }

    private void FixedUpdate()
    {
        switch (gameController.CurrentState)
        {
            case GameState.Day:
            case GameState.Stalling:
                //set light color
                if (gameController.TimeOfDay <= BrightestMorningTime)
                {
                    MappedTime = gameController.TimeOfDay / (BrightestMorningTime);
                    CurrentLightColor = Color.Lerp(DawnLightColor, DayLightColor, MappedTime);
                    SunLight.intensity = MappedTime;
                    //Debug.Log("Morning");
                }
                else if (gameController.TimeOfDay >= BrightestAfterNoonTime && gameController.TimeOfDay < DawnTime)
                {
                    MappedTime = (gameController.TimeOfDay - BrightestAfterNoonTime) / (DawnTime - BrightestAfterNoonTime);
                    CurrentLightColor = Color.Lerp(DayLightColor, DawnLightColor, MappedTime);
                    SunLight.intensity = 1 - (MappedTime - 0.1f);
                    //Debug.Log("Dawn");
                }
                else if (gameController.TimeOfDay >= DawnTime)
                {
                    MappedTime = (gameController.TimeOfDay - DawnTime) / (1 - DawnTime);
                    CurrentLightColor = Color.Lerp(DawnLightColor, NightLightColor, MappedTime);
                    SunLight.intensity = 0.1f;
                    //Debug.Log("Night");
                }

                //rotate sun
                if (gameController.TimeOfDay < 1)
                {
                    this.transform.Rotate(0, 0, -Time.fixedDeltaTime / gameController.DayLenght[(int)gameController.CurrentDay] * 180, Space.World);
                }
                break;
            case GameState.Night:
                SunLight.intensity = 0;
                break;

            default:
                SunLight.intensity = 1;
                CurrentLightColor = DayLightColor;
                break;
        }

        SunLight.color = CurrentLightColor;

        //this.transform.rotation = Quaternion.AngleAxis(190 * gameController.TimeOfDay, SunAxis);
    }

    public void FastForwardToDayTime (float FastForwardTime,ref bool Finished)
    {
        float Timer = 0;
        while (!Finished)
        {
            //rotate sun
            this.transform.Rotate(0, 0, -Time.fixedDeltaTime / FastForwardTime * 180, Space.World);

            Timer += Time.fixedDeltaTime;

            if(Timer >= FastForwardTime)
            {
                Finished = true;
            }
        }
    }
}
