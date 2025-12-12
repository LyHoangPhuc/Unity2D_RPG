//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Rendering;

//public class DayNightCycle : MonoBehaviour
//{
//    public TextMeshProUGUI timeDisplay; //Display Time
//    public TextMeshProUGUI dayDisplay;
//    public Volume ppv; //This is the post processing volume

//    public float tick; // increasing the tick, increases second rate
//    public float seconds;
//    public int mins;
//    public int hours;
//    public int days = 1;

//    public bool activateLights; //check if lights are on
//    public GameObject[] lights; //all the lights we want on when its dark
//    public SpriteRenderer[] stars; //star sprites
//    // Start is called before the first frame update
//    void Start()
//    {
//        ppv = gameObject.GetComponent<Volume>();  
//    }

//    // Update is called once per frame
//    void FixedUpdate() // we used fixed update, since update is frame dependant
//    {
//        CalcTime();
//        DislayTime();
//    }
//    public void CalcTime() //used to calculate sec, min and hours
//    {
//        seconds += Time.fixedDeltaTime * tick; //mutiply time between fixed update by tick
//        if(seconds >= 60) // 60 sec = 1 min
//        {
//            seconds = 0;
//            mins += 1;
//        }

//        if(mins >= 60) // 60 min = 1 hr
//        {
//            mins = 0;
//            hours += 1;
//        }

//        if(hours >= 24) //24h = 1 day
//        {
//            hours = 0;
//            days += 1;
//        }
//        ControlPPV(); //changes post processing volume after calculation
//    }

//    public void ControlPPV() // used to adjust the post processing slider.
//    {
//        //ppv.weight = 0
//        if(hours >= 21 && hours < 22) //dusk at 21:00/9pm - until 22:00/10 pm
//        {
//            ppv.weight = (float)mins / 60; //since dusk is 1 hr, we just divide the min by 60 which will slowly increase from 0 - 1
//            for (int i = 0; i < stars.Length; i++)
//            {
//                stars[i].color = new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, (float)mins / 60); //change the alpha value of the stars so they become visit  
//            }
//            if (activateLights == false)// if light havent been turned on
//            {
//                if (mins > 45)// wait untill pretty dark
//                {
//                    for(int i = 0; i< lights.Length; i++)
//                    {
//                        lights[i].SetActive(true);// turn them all on              
//                    }
//                    activateLights = true;
//                }
//            } 
//        }


//        if (hours >= 6 && hours < 7)//Dawn at 6 am - until 7 am
//        {
//            ppv.weight = 1 - (float)mins / 60; // we minus 1 because we want it to go from 1 - 0
//            for (int i = 0; i < stars.Length; i++)
//            {
//                stars[i].color = new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, 1 - (float)mins / 60); //make stars invisible 
//            }
//            if (activateLights == true) // if lights are on
//            {
//                if (mins > 45) // wait until pretty bright
//                {
//                    for (int i = 0; i < lights.Length; i++)
//                    {
//                        lights[i].SetActive(false); //shut them off
//                    }
//                    activateLights = false;
//                }
//            }
//        }

//    }
//    public void DislayTime()
//    {
//        timeDisplay.text = string.Format("{0:00}:{1:00}", hours, mins);
//        dayDisplay.text = "Day: " + days;
//    }
//}
