using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrMenu : MonoBehaviour
{
    public Transform botaoFase;
    public Transform imgContainer;
    public Transform estrela1, estrela2, estrela3;
    public string nomeFase;
    public int estrelas = 0;
    public TextMeshProUGUI txtExpAtual;
    public TextMeshProUGUI txtNivelAtual;

    public int ultimaFaseConquistada;
    public Transform fasesPai;
    public RectTransform indicadorFase;

    void Start()
    {
        AtualizarEstrelasAutomaticamente();
        AtualizarExperienciaUI();
        GerenciarBotoesDeFase();
        AtualizarIndicadorDeFase();
    }

    void AtualizarEstrelasAutomaticamente()
    {
        foreach (Transform filho in transform)
        {
            botaoFase = filho;
            nomeFase = botaoFase.name;

            if (!scrGerenciaFase.instance.estrelasPorFase.TryGetValue(nomeFase, out estrelas))
                estrelas = 0;

            imgContainer = botaoFase.Find("estrelas");
            if (imgContainer == null)
                continue;

            estrela1 = imgContainer.Find("estrelas (1)");
            estrela2 = imgContainer.Find("estrelas (2)");
            estrela3 = imgContainer.Find("estrelas (3)");

            if (estrela1) estrela1.gameObject.SetActive(estrelas >= 1);
            if (estrela2) estrela2.gameObject.SetActive(estrelas >= 2);
            if (estrela3) estrela3.gameObject.SetActive(estrelas == 3);
        }
    }

    public void AtualizarExperienciaUI()
    {
        float progresso = scrGerenciaFase.instance.progresso;
        int nivel = scrGerenciaFase.instance.nivelJogador;

        txtExpAtual.text = $"{progresso}/100% EXP";
        txtNivelAtual.text = $"Nível {nivel}";

    }

    void GerenciarBotoesDeFase()
    {
        ultimaFaseConquistada = scrGerenciaFase.instance.ultimaFaseConquistada;

        int index = 1;

        foreach (Transform botaoFase in fasesPai)
        {
            Button botao = botaoFase.GetComponent<Button>();
            if (botao != null)
            {
                botao.interactable = index <= ultimaFaseConquistada;
            }
            index++;
        }
    }

    void AtualizarIndicadorDeFase()
    {
        if (indicadorFase == null)
        {
            Debug.LogWarning("Indicador de Fase não atribuído!");
            return;
        }

        // Garantir que tenha pelo menos a quantidade de fases necessária
        if (ultimaFaseConquistada <= 0 || ultimaFaseConquistada > fasesPai.childCount)
        {
            Debug.LogWarning("Última fase conquistada fora do intervalo!");
            indicadorFase.gameObject.SetActive(false);
            return;
        }

        Transform botaoUltimaFase = fasesPai.GetChild(ultimaFaseConquistada - 1);

        // Posição ao lado do botão
        Vector3 novaPosicao = botaoUltimaFase.position + new Vector3(30f, 0f, 0f); // ajuste o offset conforme necessário
        indicadorFase.position = novaPosicao;

        indicadorFase.gameObject.SetActive(true);
    }

}
