using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scrMenu : MonoBehaviour
{
    public Transform botaoFase;
    public Transform imgContainer;
    public Transform estrela1, estrela2, estrela3;
    public string nomeFase;
    public int estrelas = 0;
    public TextMeshProUGUI txtExpAtual;
    public TextMeshProUGUI txtNivelAtual;

    void Start()
    {
        AtualizarEstrelasAutomaticamente();
        AtualizarExperienciaUI();
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
        int progresso = scrGerenciaFase.instance.progresso;
        int nivel = scrGerenciaFase.instance.nivelJogador;

        txtExpAtual.text = $"{progresso}/100% EXP";
        txtNivelAtual.text = $"Nível {nivel}";

    }

}
