using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

    int index = 0;
    public int totalLines = 3;
    public float yOffset = 60.2f;
    public Menu menu;
    public GameObject guiTextLink;
    public GameObject guiArrowLink;
    public Scrollbar guiScrollLink;
    Vector3 arrStartPos;
    // Use this for initialization
    void Start()
    {
        menu = new Menu(new MainMenu(), 1);
        arrStartPos = guiArrowLink.transform.localPosition;
        menu.Display(new List<string>(), guiTextLink, totalLines);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (index < totalLines - 1)
            {
                index++;
                Vector3 position = guiArrowLink.transform.localPosition;
                position.y -= yOffset;
                guiArrowLink.transform.localPosition = position;
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (index > 0)
            {
                index--;
                Vector3 position = guiArrowLink.transform.localPosition;
                position.y += yOffset;
                guiArrowLink.transform.localPosition = position;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            totalLines = menu.Action(index, menu, guiTextLink);
            guiArrowLink.transform.localPosition = arrStartPos;
            index = 0;
            guiScrollLink.value = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
        {
            
            if (menu.stateId == 2)
            {
                totalLines = 3;
                menu.SetState(new MainMenu(), 1);
                menu.Display(new List<string>(), guiTextLink, 0);
            }
            if (menu.stateId == 3)
            {
                menu.SetState(new PickCard(), 2);
                List<string> list = new List<string>();
                for (int i = 1; i <= Session.totalCardAmount; i++)
                {
                    list.Add("Card №" + i + "\n\n");
                }
                totalLines = Session.totalCardAmount;
                menu.Display(list, guiTextLink, Session.totalCardAmount);
            }
            if (menu.stateId == 4)
            {
                menu.SetState(new PickQuestion(), 3);
                int i = Session.card.questions.Count;
                List<string> list = new List<string>();
                for (int j = 1; j <= i; j++)
                {
                    string s;
                    if (Session.card.questions[j - 1].text.Length > 40)
                    {
                        s = Session.card.questions[j - 1].text.Substring(0, 40) + "...\n\n";
                    }
                    else
                    {
                        s = Session.card.questions[j - 1].text + "\n\n";
                    }
                    if (Session.card.questions[j - 1].isAnswered) list.Add("✓ " + s);
                    else list.Add(s);
                }
                GameObject go = GameObject.FindGameObjectWithTag("help");
                go.GetComponent<Text>().text = "";
                
                Session.focusedQuestion = -1;
                totalLines = i + 1;
                menu.Display(list, guiTextLink, i);
            }
            guiArrowLink.transform.localPosition = arrStartPos;
            index = 0;
            guiScrollLink.value = 1f;
        }
    }
}
