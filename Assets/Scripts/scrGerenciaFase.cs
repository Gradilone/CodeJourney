using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrGerenciaFase : MonoBehaviour
{
    public static scrGerenciaFase instance;

    public int errosDaFase = 0;          
    public int estrelasDaFase = 0;       
    public string nomeFaseAtual = "Fase1"; 

    public Dictionary<string, int> estrelasPorFase = new Dictionary<string, int>();

    public int nivelJogador = 1;
    public int progresso = 0;

    public int progressoAdicionado;

    private void Awake()
    {
        // Garantir que só existe um GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Mantém entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DefinirFase(string nomeFase)
    {
        nomeFaseAtual = nomeFase;
        ResetarErros();
    }

    public void ResetarErros()
    {
        errosDaFase = 0;
    }

    public void CalcularEstrelas()
    {
        if (errosDaFase <= 1)
            estrelasDaFase = 3;
        else if (errosDaFase <= 4)
            estrelasDaFase = 2;
        else
            estrelasDaFase = 1;

        if (estrelasPorFase.ContainsKey(nomeFaseAtual))
            estrelasPorFase[nomeFaseAtual] = estrelasDaFase;
        else
            estrelasPorFase.Add(nomeFaseAtual, estrelasDaFase);
    }

    public int ObterEstrelasDaFase(string nomeFase)
    {
        if (estrelasPorFase.ContainsKey(nomeFase))
            return estrelasPorFase[nomeFase];
        return 0;
    }

    public void MostrarEstrelasNoConsole()
    {
        Debug.Log("Progresso de Estrelas por Fase:");

        if (estrelasPorFase.Count == 0)
        {
            Debug.Log("Nenhuma estrela registrada ainda.");
            return;
        }

        foreach (var par in estrelasPorFase)
        {
            Debug.Log($"Fase: {par.Key} - Estrelas: {par.Value}");
        }
    }

    public void AdicionarProgresso()
    {
        progressoAdicionado = CalcularExperiencia(errosDaFase);
        Debug.Log("Progresso da fase adicionado: " + progressoAdicionado);

        progresso += progressoAdicionado;

        while (progresso >= 100)
        {
            progresso -= 100;
            nivelJogador++;

        }
    }


    private int CalcularExperiencia(int erros)
    {
        if (erros >= 7)
            return Random.Range(1, 20);       // 1 a 10
        else if (erros == 6)
            return Random.Range(21, 40);      // 61 a 80
        else if (erros == 5)
            return Random.Range(41, 50);     // 81 a 100
        else if (erros == 4)
            return Random.Range(51, 60);     // 81 a 100
        else if (erros == 3)
            return Random.Range(61, 70);     // 81 a 100
        else if (erros <= 2)
            return Random.Range(71, 99);     // 81 a 100
        else
            return 1;
    }
}
