using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using Assets.Scripts;
using System;

public interface IMenuState
{
    void ShowMenu(List<string> l, GameObject guiTextLink, int totalLines);
    int Action(int index, Menu m, GameObject guiTextLink);
}

public class MainMenu : IMenuState
{ 
    public void ShowMenu(List<string> l, GameObject guiTextLink, int totalLines)
    {
        guiTextLink.GetComponent<Text>().text = "\nWelcome to virtual exam simulator!\n\n\n";
        guiTextLink.GetComponent<Text>().text += "Pick card\n\n";
        guiTextLink.GetComponent<Text>().text += "History\n\n";
        guiTextLink.GetComponent<Text>().text += "Exit\n\n";
    }

    public int Action(int index, Menu m, GameObject guiTextLink)
    {
        if (index == 2) { Application.Quit(); return 3; }
        if (index == 1)
        {
            m.SetState(new ShowAllResults(), 6);
            m.Display(new List<string>(), guiTextLink, 3);
            return 3;
        }
        if (index == 0)
        {
            int amount = 0;
            
            Session.dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = Session.dbconn.CreateCommand();
            string sqlQuery = "SELECT count(*) FROM Card";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            if (reader.Read())
            {
                amount = reader.GetInt32(0);
                //Debug.Log("amount= " + amount);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            Session.dbconn.Dispose();
            Session.dbconn.Close();

            List<string> list = new List<string>();
            for (int i = 1; i <= amount; i++)
            {
                list.Add("Card №" + i + "\n\n");
            }

            Session.totalCardAmount = amount;
            m.SetState(new PickCard(), 2);
            m.Display(list, guiTextLink, amount);
            return amount;
        }
        else return 3;
    }
}

public class PickCard : IMenuState
{
    public void ShowMenu(List<string> l, GameObject guiTextLink, int totalLines)
    {
        GameObject go = GameObject.FindGameObjectWithTag("TimerText1");
        go.GetComponent<Timer>().Stop();

        guiTextLink.GetComponent<Text>().text += ("\nPick card from the list below:\n\n\n");
        foreach (var i in l)
        {
            guiTextLink.GetComponent<Text>().text += i;
        }
    }

    public int Action(int index, Menu m, GameObject guiTextLink)
    {
        int i = 0;
        Session.dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = Session.dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM Question q join Answer a on q.question_id = a.question_id where card_id=" + (index + 1) + ";";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        Session.card = new Card();
        Session.card.questions = new List<Question>();
        Session.card.ID = index + 1;
        reader.Read();
        Question q = new Question();
        q.answers = new List<Answer>();

        q.Id = reader.GetInt32(0);
        q.multiplier = (float)reader.GetFloat(1);
        q.typeId = reader.GetInt32(2);
        q.text = reader.GetString(4);

        bool isRead = true;
        do
        {
            
            Answer a = new Answer();
            a.isCorrect = reader.GetBoolean(6);
            a.text = reader.GetString(8);
            q.answers.Add(a);
            while ((isRead = reader.Read()) && reader.GetInt32(0) == q.Id)
            {
                Answer a1 = new Answer();
                a1.isCorrect = reader.GetBoolean(6);
                a1.text = reader.GetString(8);
                q.answers.Add(a1);
            }
            Session.card.questions.Add(q);
            i++;
            if (isRead)
            {
                q = new Question();
                q.answers = new List<Answer>();

                q.Id = reader.GetInt32(0);
                q.multiplier = (float)reader.GetFloat(1);
                q.typeId = reader.GetInt32(2);
                q.text = reader.GetString(4);

                a = new Answer();
                a.isCorrect = reader.GetBoolean(6);
                a.text = reader.GetString(8);
                q.answers.Add(a);
            }
        } while (reader.Read());


        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        Session.dbconn.Dispose();
        Session.dbconn.Close();

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
            list.Add(s);
        }
        m.SetState(new PickQuestion(), 3);
        m.Display(list, guiTextLink, i+1);

        GameObject go = GameObject.FindGameObjectWithTag("TimerText1");
        go.GetComponent<Timer>().Play();
        return i + 1;
    }
}

public class PickQuestion : IMenuState
{
    public void ShowMenu(List<string> l, GameObject guiTextLink, int totalLines)
    {
        guiTextLink.GetComponent<Text>().text += ("\nPick question from the list below:\n\n\n");
        l.Add("Finish exam");
        foreach (var i in l)
        {
            guiTextLink.GetComponent<Text>().text += i;
        }
        Session.focusedQuestion = -1;
    }

    public int Action(int index, Menu m, GameObject guiTextLink)
    {
        if (index == Session.card.questions.Count)
        {
            m.SetState(new ShowResult(), 5);
            m.Display(new List<string>(), guiTextLink, 1);
            return 1;
        }
        else
        {
            List<string> list = new List<string>();
            Question q = new Question();
            q = Session.card.questions[index];
            int i = q.answers.Count;
            if (q.text.Length > 80) list.Add("\n" + q.text + "\n");
            if (q.text.Length >= 40 && q.text.Length <= 80) list.Add("\n" + q.text + "\n\n");
            if (q.text.Length < 40) list.Add("\n" + q.text + "\n\n\n");
            for (int j = 1; j <= i; j++)
            {
                string s;
                if (q.answers[j - 1].text.Length > 40)
                {
                    s = q.answers[j - 1].text.Substring(0, 40) + "...\n\n";
                }
                else
                {
                    s = q.answers[j - 1].text + "\n\n";
                }

                list.Add(s);
            }
            
            Session.focusedQuestion = index;
            m.SetState(new PickAnswer(), 4);
            m.Display(list, guiTextLink, i);
            return i;
        }
    }
    
}

public class PickAnswer : IMenuState
{
    public void ShowMenu(List<string> l, GameObject guiTextLink, int totalLines)
    {
        guiTextLink.GetComponent<Text>().text += (l[0]);
        for(int i=1; i<l.Count; i++)
        {
            guiTextLink.GetComponent<Text>().text += l[i];
        }
    }

    public int Action(int index, Menu m, GameObject guiTextLink)
    {
        Session.card.questions[Session.focusedQuestion].userAnswerIndex = index;
        Session.card.questions[Session.focusedQuestion].isAnswered = true;
        List<string> list = new List<string>();
        for (int j = 1; j <= Session.card.questions.Count; j++)
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

            if(Session.card.questions[j - 1].isAnswered)list.Add("✓ " + s);
            else list.Add(s);
        }
        GameObject go = GameObject.FindGameObjectWithTag("help");
        go.GetComponent<Text>().text = "";
        
        Session.focusedQuestion = -1;
        m.SetState(new PickQuestion(), 3);
        m.Display(list, guiTextLink, Session.card.questions.Count + 1);
        return Session.card.questions.Count + 1;
    }
}

public class ShowResult : IMenuState
{
    public void ShowMenu(List<string> l, GameObject guiTextLink, int totalLines)
    {
        GameObject go = GameObject.FindGameObjectWithTag("TimerText1");
        GameObject goArr = GameObject.FindGameObjectWithTag("Arrow");
        goArr.GetComponent<Image>().enabled = false;
        guiTextLink.GetComponent<Text>().text += "\nExam is finished\n\n";
        Session.points = 0f;
        float maxPoints = 0f;
        foreach (var q in Session.card.questions)
        {
            maxPoints += q.multiplier;
            if (q.answers[q.userAnswerIndex].isCorrect)
                Session.points += q.multiplier;
        }
        guiTextLink.GetComponent<Text>().text += "Your results:\n\n";
        guiTextLink.GetComponent<Text>().text += "You got " + Session.points + "/" + maxPoints + "  points\n\n";

        
        string strTime = Mathf.Floor((go.GetComponent<Timer>().getTime() % 3600) / 60).ToString("00") + ":" + (go.GetComponent<Timer>().getTime() % 60).ToString("00");
        guiTextLink.GetComponent<Text>().text += "Your time: " + strTime + "\n\n";
        go.GetComponent<Timer>().Stop();
        guiTextLink.GetComponent<Text>().text += "Press \"Return\" to go to main menu...\n";
    }

    public int Action(int index, Menu m, GameObject guiTextLink)
    {
        Session.SaveSession();
        GameObject goArr = GameObject.FindGameObjectWithTag("Arrow");
        goArr.GetComponent<Image>().enabled = true;
        m.SetState(new MainMenu(), 1);
        m.Display(new List<string>(), guiTextLink, 3);
        return 3;
    }
}

public class ShowAllResults : IMenuState
{
    public void ShowMenu(List<string> l, GameObject guiTextLink, int totalLines)
    {
        guiTextLink.GetComponent<Text>().text += "\nPress \"Return\" to go to main menu...\n\nExam history :\n\n";
        GameObject goArr = GameObject.FindGameObjectWithTag("Arrow");
        goArr.GetComponent<Image>().enabled = false;
        Session.dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = Session.dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM Session;";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        
        while (reader.Read())
        {
            string str = "";
            string dt = reader.GetString(1); 
            if(dt.Length>18)dt = dt.Substring(0,18);
            str +=  reader.GetInt32(0) + ". " + dt + " Points: " + reader.GetFloat(2).ToString() + " Card №" + reader.GetInt32(3).ToString();
            guiTextLink.GetComponent<Text>().text += str + "\n";
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        Session.dbconn.Dispose();
        Session.dbconn.Close();
    }

    public int Action(int index, Menu m, GameObject guiTextLink)
    {
        GameObject goArr = GameObject.FindGameObjectWithTag("Arrow");
        goArr.GetComponent<Image>().enabled = true;
        m.SetState(new MainMenu(), 1);
        m.Display(new List<string>(), guiTextLink, 3);
        return 3;
    }
}

public class Menu        //context
{
    public IMenuState State { get; set; } 
    public int stateId { get; set; }

    public Menu(IMenuState state, int sid)
    {
        this.State = state;
        this.stateId = sid;
    }

    public void SetState(IMenuState state, int sid)
    {
        this.State = state;
        this.stateId = sid;
    }

    public void Display(List<string> l, GameObject guiTextLink, int totalLines)
    {
        guiTextLink.GetComponent<Text>().text = "";
        State.ShowMenu(l, guiTextLink, l.Count);

    }

    public int Action(int index, Menu m, GameObject guiTextLink)
    {
        return State.Action(index, m, guiTextLink);
    }
    
}
