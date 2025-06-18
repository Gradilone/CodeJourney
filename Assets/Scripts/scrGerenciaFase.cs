using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using static scrConexaoAPI;

public class scrGerenciaFase : MonoBehaviour
{
    public static scrGerenciaFase instance;

    public string jornadaAtual; // Nome da jornada atual, pode ser alterado conforme necessário

    public int errosDaFase = 0;          
    public int estrelasDaFase = 0;
    public string nomeFaseAtual = "Fase1";

    public int ultimaFaseConquistada;

    public Dictionary<string, int> estrelasPorFase = new Dictionary<string, int>();

    public int nivelJogador = 1;
    public float progresso = 0;

    public int progressoAdicionado;

    public scrAutenticador autenticador;
    public scrConexaoAPI apiConexao;

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{\"Items\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.Items;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    private void Awake()
    {
        GameObject objAutenticador = GameObject.Find("Autenticador");
        autenticador = objAutenticador.GetComponent<scrAutenticador>();

        
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

        StartCoroutine(CarregarNivelEProgressoDoServidor());
    

    }

    private IEnumerator CarregarNivelEProgressoDoServidor()
    {
        int userId = autenticador.usuarioId;
        string token = autenticador.bearerToken;

        Debug.Log($"Carregando dados do servidor para o usuário {userId}...");

        yield return apiConexao.GetNivelEProgresso(userId, token,
            onSuccess: (dados) =>
            { 
                nivelJogador = dados.nivel;
                progresso = dados.progresso;

                if (nivelJogador == 0)
                {
                    nivelJogador = 1;
                }

                Debug.Log($"Dados carregados: Nível {nivelJogador}, Progresso {progresso}");
            },
            onError: (code, err) =>
            {
                Debug.LogError($"Erro ao carregar dados de nível e progresso: {err} (Código {code})");

            });
    }

    private IEnumerator CarregarDadosDaJornadaCoroutine()
    {
        int usuarioId = autenticador.usuarioId;
        string jornNome = jornadaAtual;

        Debug.Log($"Iniciando busca da jornada para o usuário {usuarioId}, Jornada: {jornNome}");

        yield return apiConexao.BuscarJornada(usuarioId, jornNome, autenticador.bearerToken,
            onSuccess: (jsonRetornado) =>
            {
                Debug.Log("JSON recebido da API: " + jsonRetornado);

                JornadaData[] dadosRecebidos = JsonHelper.FromJson<JornadaData>(jsonRetornado);

                estrelasPorFase.Clear();

                foreach (var dado in dadosRecebidos)
                {
                    string fase = dado.jornFase;
                    int estrelas = dado.jornEstrelas;

                    estrelasPorFase[fase] = estrelas;

                    Debug.Log($"Atualizado: Fase {fase} -> {estrelas} estrelas.");
                }

                if (dadosRecebidos.Length > 0)
                {
                    int ultimoIndex = dadosRecebidos.Length - 1;
                    ultimaFaseConquistada = scrGerenciaFase.instance.ultimaFaseConquistada;
                    ultimaFaseConquistada = dadosRecebidos[ultimoIndex].jornUltimaFase;

                    Debug.Log($"jornUltimaFase atualizado para: {dadosRecebidos[ultimoIndex].jornUltimaFase}");
                }

                Debug.Log("Dados da Jornada carregados com sucesso.");
            },
            onError: (code, err) =>
            {
                Debug.LogError($"Erro ao carregar dados da jornada: {err} (Código {code})");
            }
        );
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

        if (progresso > 99)
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

    public void JornadaEscolhida()
    {
        GameObject botaoClicado = EventSystem.current.currentSelectedGameObject;

        if (botaoClicado != null)
        {
            string tagBotao = botaoClicado.tag;
            jornadaAtual = tagBotao;
            Debug.Log("Jornada escolhida: " + jornadaAtual);

            StartCoroutine(CarregarDadosDaJornadaCoroutine());
        }
        else
        {
            Debug.LogWarning("Nenhum botão foi clicado.");
        }
    }
}
