using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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

    void Start()
    {
        foreach (var input in lacunas)
        {
            coresOriginais.Add(input.image.color);
            TMP_InputField captured = input;
            captured.onSelect.AddListener(delegate { LimparFeedback(captured); });

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
            SubirPainel();
        }


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
}
