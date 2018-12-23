using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Help : MonoBehaviour
{

    public Camera cam;
    public GameObject hitObj;
    public GameObject txt;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            if (hit.transform.name == hitObj.transform.name)
            {
                if (Input.GetKeyDown(KeyCode.F) && Session.focusedQuestion >=0)
                {
                    var a = Session.card.questions[Session.focusedQuestion].answers.Where(i => i.isCorrect == true).FirstOrDefault();
                    string s = a.text;
                    if (s.Length > 40) s = s.Substring(0, 40) + "...";
                    txt.GetComponent<Text>().text = s;
                }
            }
        }
    }
}
