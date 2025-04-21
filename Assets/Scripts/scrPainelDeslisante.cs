using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPainelDeslisante : MonoBehaviour
{
    public RectTransform painel;
    public float velocidade = 500f;
    public Vector2 posicaoAberto;
    public Vector2 posicaoFechado;
    private bool aberto = true;

    void Start()
    {
        // Começa na posição fechada
        painel.anchoredPosition = posicaoAberto;
    }

    void Update()
    {
        Vector2 alvo = aberto ? posicaoAberto : posicaoFechado;
        painel.anchoredPosition = Vector2.Lerp(painel.anchoredPosition, alvo, Time.deltaTime * 5f);
    }

    public void AlternarPainel()
    {
        aberto = !aberto;
    }
}
