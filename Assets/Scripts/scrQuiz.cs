using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrQuiz : MonoBehaviour
{
    public Question[] perguntas;
    public TextMeshProUGUI perguntaText;
    public GameObject[] opcoes;
    public int questaoAtual = 0;
    private bool[] perguntasRespondidas;
    public float velocidadeDigitacao = 0.05f;

    private Coroutine animacaoPergunta;

    public int proximaCenaIndex;

    public scrPainelDeslisante painelTransicao;


    private void Start()
    {
        perguntasRespondidas = new bool[perguntas.Length];
        if (perguntas.Length > 0)
        {
            SetQuestion();
        }
    }

    public void SetQuestion()
    {
        if (animacaoPergunta != null)
            StopCoroutine(animacaoPergunta);

        animacaoPergunta = StartCoroutine(AnimarPergunta(perguntas[questaoAtual].pergunta));
        SetAnswers();
    }

    IEnumerator AnimarPergunta(string texto)
    {
        perguntaText.text = "";
        foreach (char letra in texto.ToCharArray())
        {
            perguntaText.text += letra;
            yield return new WaitForSeconds(velocidadeDigitacao);
        }
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

    private int ObterIndiceAleatorio()
    {
        List<int> indicesDisponiveis = new List<int>();
        for (int i = 0; i < perguntas.Length; i++)
        {
            if (!perguntasRespondidas[i])
            {
                indicesDisponiveis.Add(i);
            }
        }

        if (indicesDisponiveis.Count == 0)
            return -1;

        int indiceAleatorio = UnityEngine.Random.Range(0, indicesDisponiveis.Count);
        return indicesDisponiveis[indiceAleatorio];
    }

    public void Correto()
    {
        StartCoroutine(ExecutarCorretoComDelay());
    }
    private IEnumerator ExecutarCorretoComDelay()
    {
        yield return new WaitForSeconds(1.5f);

        // Marca a pergunta atual como respondida
        perguntasRespondidas[questaoAtual] = true;
        int novoIndice = ObterIndiceAleatorio();

        if (novoIndice != -1)
        {
            questaoAtual = novoIndice;
            SetQuestion();
        }
        else
        {
            // Todas as perguntas foram respondidas.
            perguntaText.text = "Parabéns! Você concluiu.";
            foreach (GameObject opcao in opcoes)
            {
                opcao.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }

            StartCoroutine(ExecutarDepoisDeDoisSegundos());
        }
    }

    IEnumerator ExecutarDepoisDeDoisSegundos()
    {
        yield return new WaitForSeconds(2f);
        Invoke("CarregarProximaCena", 1.5f);

        painelTransicao.aberto = true;
    }

    void CarregarProximaCena()
    {
        SceneManager.LoadScene(proximaCenaIndex);
    }

}
