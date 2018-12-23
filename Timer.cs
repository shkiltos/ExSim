using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    Text text;
    float theTime = 600;
    public float speed = 1;
    bool playing;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playing == true)
        {
            theTime -= Time.deltaTime * speed;
            
            string minutes = Mathf.Floor((theTime % 3600) / 60).ToString("00");
            string seconds = (theTime % 60).ToString("00");
            text.text = minutes + ":" + seconds;

            if (theTime <= 0)
            {
                GameObject c = GameObject.FindGameObjectWithTag("GameController");
                timeIsOut(c.GetComponent<MainScript>().menu, c.GetComponent<MainScript>().guiTextLink);
                Stop();
            }
        }
    }

    public void Play()
    {
        playing = true;
    }

    public void Stop()
    {
        theTime = 600;
        playing = false;
        text.text = "00:00";
    }

    public void timeIsOut(Menu m, GameObject guiTextLink)
    {
        m.SetState(new ShowResult(), 5);
        m.Display(new List<string>(), guiTextLink, 1);
    }

    public float getTime()
    {
        return 600-theTime;
    }
}