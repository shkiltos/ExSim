using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question
{
    public int Id;
    public string text;
    public int typeId;
    public List<Answer> answers;
    public int userAnswerIndex;
    public float multiplier;
    public bool isAnswered = false;
}
