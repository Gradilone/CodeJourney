using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static scrConexaoAPI;

public class scrValidadorCodigo : MonoBehaviour
{
    [Header("Entradas do usuário (as lacunas do desafio)")]
    public List<TMP_InputField> lacunas;

    [Header("Respostas corretas (na mesma ordem das lacunas)")]
    public List<string> respostasCorretas;
    [Header("Prefabs para feedback visual")]
    public GameObject corretoPrefab;
    public GameObject erradoPrefab;

    [Header("Cores de Feedback")]
    public Color corCerta = new Color(0.8f, 1f, 0.8f); // verde claro
    public Color corErrada = new Color(1f, 0.8f, 0.8f); // vermelho claro
    public float fadeDuration = 0.3f;

    private List<GameObject> feedbacksInstanciados = new List<GameObject>();
    private List<Color> coresOriginais = new List<Color>();

    public RectTransform painelFinal;
    public Vector2 posicaoInicial = new Vector2(0, -500);
    public Vector2 posicaoFinal = new Vector2(0, 0);
    public float duracaoAnimacao = 1f;

    public GameObject imgEstrelaAcesa1;
    public GameObject imgEstrelaAcesa2;
    public GameObject imgEstrelaAcesa3;

    public TextMeshProUGUI txtProgressoAdicionado;
    public TextMeshProUGUI txtNivel;
    public GameObject txtNovoNivel;

    public int progressoGanho;
    public int progressoAtual;
    public int progressoAntes;

    public Slider sliderProgresso;

    public scrConexaoAPI apiConexao;
    public scrAutenticador autenticador;

    public bool estaConcluida = false;

    public List<Selectable> tabOrder;

    void Start()
    {
        apiConexao = FindObjectOfType<scrConexaoAPI>();

        GameObject objAutenticador = GameObject.Find("Autenticador");
        autenticador = objAutenticador.GetComponent<scrAutenticador>();


        foreach (var input in lacunas)
        {
            coresOriginais.Add(input.image.color);
            TMP_InputField captured = input;
            captured.onSelect.AddListener(delegate { LimparFeedback(captured); });


        };


    }

    private void Update()
    {
        NavegarComTab();
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            VerificarRespostas();
        }

    }

    void NavegarComTab()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;
            if (current != null)
            {
                int index = tabOrder.IndexOf(current.GetComponent<Selectable>());
                if (index != -1)
                {
                    int nextIndex = (index + 1) % tabOrder.Count;
                    EventSystem.current.SetSelectedGameObject(tabOrder[nextIndex].gameObject);
                }
            }
        }
    }

    public void VerificarRespostas()
    {
        bool todasCorretas = true;
        LimparFeedbacksAtuais();

        for (int i = 0; i < lacunas.Count && i < respostasCorretas.Count; i++)
        {

            TMP_InputField input = lacunas[i];
            string respostaEsperada = respostasCorretas[i];
            string respostaDigitada = input.text.Trim();

            bool correto = respostaDigitada == respostaEsperada;
            GameObject prefabUsado = correto ? corretoPrefab : erradoPrefab;

            // Instanciar feedback corretamente no canvas
            GameObject feedbackObj = Instantiate(prefabUsado, input.transform.parent);
            feedbackObj.transform.SetParent(input.transform.parent, false);

            // Posicionar feedback acima do input
            RectTransform feedbackRT = feedbackObj.GetComponent<RectTransform>();
            RectTransform inputRT = input.GetComponent<RectTransform>();

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, inputRT.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)input.transform.parent,
                screenPoint + new Vector2(0, inputRT.rect.height + 5),
                null,
                out Vector2 localPoint
            );
            feedbackRT.localPosition = localPoint;

            // Cor do input
            input.image.color = correto ? corCerta : corErrada;

            feedbacksInstanciados.Add(feedbackObj);

            // Console info
            if (!correto)
            {
                Debug.Log($"Resposta incorreta na lacuna {i + 1}. Digitado: '{respostaDigitada}', Esperado: '{respostaEsperada}'");
                
            }
            else
            {
                Debug.Log($"Resposta correta na lacuna {i + 1}.");
                input.interactable = false;
            }

            if (lacunas[i].interactable)
            {
                todasCorretas = false;
            }

        }

        if (todasCorretas)
        {
            scrGerenciaFase.instance.CalcularEstrelas();
            int estrelas = scrGerenciaFase.instance.estrelasDaFase;

            Debug.Log("Estrelas recebidas: " + estrelas);

            
            SubirPainel();

            SalvarDados(estrelas);
        }
        else
        {
            scrGerenciaFase.instance.errosDaFase++;
        }


    }

    public void SalvarDados(int estrelas, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        JornadaData dadosAtualizados = new JornadaData
        {
            usuarioId = autenticador.usuarioId,
            jornNome = scrGerenciaFase.instance.jornadaAtual,
            jornFase = scrGerenciaFase.instance.nomeFaseAtual,
            jornEstrelas = estrelas,
            jornUltimaFase = scrGerenciaFase.instance.ultimaFaseConquistada
        };

        Debug.Log($"Enviando dados da jornada: {dadosAtualizados.usuarioId}");

        StartCoroutine(apiConexao.InserirJornada(dadosAtualizados, autenticador.bearerToken,
            onSuccess: (resposta) =>
            {
                Debug.Log("Jornada enviada com sucesso: " + resposta);
                onSuccess?.Invoke(resposta);
            },
            onError: (codigo, erro) =>
            {
                Debug.LogError($"Erro ao enviar jornada: Código {codigo}, Erro: {erro}");
                onError?.Invoke(codigo, erro);
            }
        ));

    }

    public void LimparFeedback(TMP_InputField input)
    {
        int index = lacunas.IndexOf(input);
        if (index >= 0 && index < coresOriginais.Count)
        {
            input.image.color = coresOriginais[index];
        }

        foreach (var feedback in feedbacksInstanciados)
        {
            if (feedback != null)
            {
                CanvasGroup cg = feedback.GetComponent<CanvasGroup>();
                if (cg != null)
                    StartCoroutine(FadeOutAndDestroy(cg.gameObject));
                else
                    Destroy(feedback);
            }
        }

        feedbacksInstanciados.Clear();
    }

    private void LimparFeedbacksAtuais()
    {
        foreach (var feedback in feedbacksInstanciados)
        {
            if (feedback != null)
            {
                Destroy(feedback);
            }
        }
        feedbacksInstanciados.Clear();

        for (int i = 0; i < lacunas.Count && i < coresOriginais.Count; i++)
        {
            lacunas[i].image.color = coresOriginais[i];
        }
    }

    IEnumerator FadeOutAndDestroy(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Destroy(obj);
            yield break;
        }

        float t = 0f;
        float startAlpha = cg.alpha;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }

        Destroy(obj);
    }

    public void SubirPainel()
    {
        AtualizarProgresso();
        AtualizarEstrelasUI(); 
        StartCoroutine(AnimarPainel());
    }

    private IEnumerator AnimarPainel()
    {
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracaoAnimacao)
        {
            tempoDecorrido += Time.deltaTime;
            float t = Mathf.Clamp01(tempoDecorrido / duracaoAnimacao);
            painelFinal.anchoredPosition = Vector2.Lerp(posicaoInicial, posicaoFinal, t);
            yield return null;
        }

        painelFinal.anchoredPosition = posicaoFinal;
    }

    public void AtualizarEstrelasUI()
    {
        int estrelas = scrGerenciaFase.instance.estrelasDaFase;

        // Desativa tudo primeiro
        imgEstrelaAcesa1.SetActive(false);
        imgEstrelaAcesa2.SetActive(false);
        imgEstrelaAcesa3.SetActive(false);

        // Ativa conforme a quantidade de estrelas
        if (estrelas >= 1)
            imgEstrelaAcesa1.SetActive(true);
        if (estrelas >= 2)
            imgEstrelaAcesa2.SetActive(true);
        if (estrelas >= 3)
            imgEstrelaAcesa3.SetActive(true);
    }
    public void AtualizarProgresso()
    {
        int ultimaFase = scrGerenciaFase.instance.ultimaFaseConquistada;

        string nomeFase = scrGerenciaFase.instance.nomeFaseAtual;
        int numeroFaseAtual = int.Parse(System.Text.RegularExpressions.Regex.Match(nomeFase, @"\d+").Value);

        // Verifica se a fase atual é diferente da última fase conquistada
        if (numeroFaseAtual == scrGerenciaFase.instance.ultimaFaseConquistada)
        {
            scrGerenciaFase.instance.ultimaFaseConquistada = numeroFaseAtual + 1;
            Debug.Log("Ultima fase conquistada atualizada para: " + scrGerenciaFase.instance.ultimaFaseConquistada);
        }
        else
        {
            Debug.Log("Fase já conquistada. Nenhuma atualização.");
        }


        float progressoAntes = scrGerenciaFase.instance.progresso;

        scrGerenciaFase.instance.AdicionarProgresso();



        int userId = autenticador.usuarioId;
        string token = autenticador.bearerToken;

        float progressoGanho = scrGerenciaFase.instance.progressoAdicionado;
        float progressoAtual = scrGerenciaFase.instance.progresso;
        int nivel = scrGerenciaFase.instance.nivelJogador;

        txtProgressoAdicionado.text = $"+{progressoGanho} EXP";
        sliderProgresso.value = progressoAtual;

        // Se o progresso anterior somado ao ganho ultrapassar 100, sobe de nível
        if (progressoAntes + progressoGanho >= 100)
        {
            txtNivel.text = "Nível " + scrGerenciaFase.instance.nivelJogador;
            txtNovoNivel.SetActive(true);
        }
        else
        {
            txtNivel.text = "Nível " + scrGerenciaFase.instance.nivelJogador;
            txtNovoNivel.SetActive(false);
        }

        ProgressoData dadosParaAtualizar = new ProgressoData
        {
            nivel = nivel,
            progresso = progressoAtual
        };

        JornadaData dados = new JornadaData
        {
            jornUltimaFase = ultimaFase
        };

        StartCoroutine(apiConexao.UpdateNivelEProgresso(userId, dadosParaAtualizar, token,
       onSuccess: (res) => {
           Debug.Log("Progresso sincronizado com servidor com sucesso.");
       },
       onError: (code, err) => {
           Debug.LogError($"Falha ao sincronizar progresso: {err} (Código {code})");
       }));

        StartCoroutine(apiConexao.AtualizarJornada(userId, dados, token,
            onSuccess: (res) =>
            {
                Debug.Log("Jornada atualizada com sucesso: " + res);
            },
            onError: (code, err) =>
            {
                Debug.LogError($"Erro ao atualizar jornada: {err} (Código {code})");
            }
        ));

    }
}
