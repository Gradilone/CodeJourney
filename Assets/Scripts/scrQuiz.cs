using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scrQuiz : MonoBehaviour
{
    public Question[] perguntas;
    public TextMeshProUGUI perguntaText;
    public GameObject[] opcoes;
    public int questaoAtual = 0;
    public scrRespostas respostas;

    private void Start()
    { 
        if (perguntas.Length > 0)
        {
            SetQuestion();
        }
    }

    public void SetQuestion()
    {
        perguntaText.text = perguntas[questaoAtual].pergunta;
        SetAnswers();
    }
    public void SetAnswers()
    {
        for (int i = 0; i < opcoes.Length; i++)
        {
            scrRespostas respostaScript = opcoes[i].GetComponent<scrRespostas>();
            respostaScript.eCorreto = (perguntas[questaoAtual].Correct == i + 1);
            opcoes[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = perguntas[questaoAtual].Answers[i];

        }
    }

    public void Correto()
    {
        SetQuestion();
    }

}
