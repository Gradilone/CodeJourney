using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPainelDeslisante : MonoBehaviour
{
    public RectTransform painel;
    public float velocidade = 500f;
    public Vector2 posicaoAberto;
    public Vector2 posicaoFechado;
    public bool aberto = true;
    public bool autoFechar = false;
    public bool autoAbrir = false;

    private float tempoFechar = 0f;
    private float tempoAbrir = 0f;

    public float tempoLimite = 10f;




    void Update()
    {
        Vector2 alvo = aberto ? posicaoAberto : posicaoFechado;
        painel.anchoredPosition = Vector2.Lerp(painel.anchoredPosition, alvo, Time.deltaTime * 2f);

        if (autoFechar && aberto)
        {
            tempoFechar += Time.deltaTime;
            if (tempoFechar >= tempoLimite)
            {
                aberto = false;
                autoFechar = false;
                tempoFechar = 0f;
            }
        }

        if (autoAbrir && !aberto)
        {
            tempoAbrir += Time.deltaTime;
            if (tempoAbrir >= tempoLimite)
            {
                aberto = true;
                autoAbrir = false;
                tempoAbrir = 0f;
                
            }
        }

    }

    public void AlternarPainel()
    {
        aberto = !aberto;
        tempoFechar = 0f;
        tempoAbrir = 0f;
    }
}
