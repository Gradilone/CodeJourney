using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrRespostas : MonoBehaviour
{
    public bool eCorreto;
    public scrQuiz quiz;
    public void Answer()
    {
        if (eCorreto)
        {
            Debug.Log("Acertou!");
            quiz.Correto();
        }
        else
        {
            Debug.Log("Errou!");
            quiz.Correto();
        }

    }
}
