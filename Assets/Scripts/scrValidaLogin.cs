using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scrValidaLogin : MonoBehaviour
{
   
    public TMP_InputField inpUsuario;
    public TMP_InputField inpSenha;
    public Button btnLogin;

    private List<Selectable> tabOrder;

    public GameObject tooltipUsuario;
    public GameObject tooltipSenha;
    public GameObject tooltipConfirmacao;

    public int sceneIndex;

    public scrConexaoAPI conexaoAPI;

    void Start()
    {
        conexaoAPI = FindObjectOfType<scrConexaoAPI>();

        btnLogin.onClick.AddListener(ValidarFormulario);

        tabOrder = new List<Selectable> {
            inpUsuario,
            inpSenha
        };


        OcultarTodosOsTooltips();
    }

    void Update()
    {
        NavegarComTab();
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

    void ValidarFormulario()
    {
        OcultarTodosOsTooltips();

        if (!ValidarCampos())
            return;

        MostrarTooltip(tooltipConfirmacao, "Carregando...", Color.green);

        ExecutarLogin();
    }

    bool ValidarCampos()
    {
        bool valido = true;

        if (string.IsNullOrWhiteSpace(inpUsuario.text))
        {
            MostrarTooltip(tooltipUsuario, "Preencha o usuário.");
            valido = false;
        }
        else if (!Regex.IsMatch(inpUsuario.text, @"^[a-zA-Z0-9\s]+$"))
        {
            MostrarTooltip(tooltipUsuario, "Usuário não pode conter caracteres especiais.");
            valido = false;
        }

        if (string.IsNullOrWhiteSpace(inpSenha.text))
        {
            MostrarTooltip(tooltipSenha, "Digite uma senha.");
            valido = false;
        }
        else if (inpSenha.text.Length < 5)
        {
            MostrarTooltip(tooltipSenha, "A senha deve ter no mínimo 5 caracteres.");
            valido = false;
        }

        return valido;
    }

    void ExecutarLogin()
    {
        scrConexaoAPI.LoginData loginData = new scrConexaoAPI.LoginData
        {
            userName = inpUsuario.text,
            senha = inpSenha.text
        };

        StartCoroutine(conexaoAPI.EnviarLogin(loginData,
            onSuccess: (resposta) =>
            {
                RespostaLogin tokenResponse = JsonUtility.FromJson<RespostaLogin>(resposta);
                SalvarTokenEId(tokenResponse);
                BuscarDadosCompletos();
            },
            onError: (statusCode, erro) =>
            {
                if (statusCode == 401)
                {
                    MostrarTooltip(tooltipConfirmacao, "Usuário ou senha inválidos.");
                }
                else
                {
                    MostrarTooltip(tooltipConfirmacao, "Erro no login: " + erro);
                }
            }
        ));
    }

    void SalvarTokenEId(RespostaLogin tokenResponse)
    {
        scrAutenticador.Instance.bearerToken = tokenResponse.tokenName;
        scrAutenticador.Instance.usuarioId = tokenResponse.id;

        Debug.Log("Token salvo: " + scrAutenticador.Instance.bearerToken);
        Debug.Log("ID salvo: " + scrAutenticador.Instance.usuarioId);
    }

    void BuscarDadosCompletos()
    {
        StartCoroutine(conexaoAPI.BuscarDadosCompletosUsuario(
            scrAutenticador.Instance.usuarioId,
            scrAutenticador.Instance.bearerToken,
            (respostaDados) =>
            {
                DadosUsuarioCompleto dados = JsonUtility.FromJson<DadosUsuarioCompleto>(respostaDados);
                SalvarDadosCompletos(dados);
                TrocarCenaComSucesso();
            },
            (statusCode, erro) =>
            {
                MostrarTooltip(tooltipConfirmacao, "Erro ao buscar dados do usuário: " + erro);
            }
        ));
    }

    void SalvarDadosCompletos(DadosUsuarioCompleto dados)
    {
        scrAutenticador.Instance.nome = dados.nome;
        scrAutenticador.Instance.userName = dados.userName;
        scrAutenticador.Instance.email = dados.email;
        scrAutenticador.Instance.dataNascimento = dados.dataNascimento;
        scrAutenticador.Instance.dataCadastro = dados.dataCadastro;

        Debug.Log("Dados completos salvos.");
    }

    void TrocarCenaComSucesso()
    {
        MostrarTooltip(tooltipConfirmacao, "Login efetuado!", Color.green);
        StartCoroutine(TrocarCenaAposDelay(1.5f, sceneIndex));
    }

    void MostrarTooltip(GameObject tooltip, string mensagem, Color? cor = null)
    {
        tooltip.SetActive(true);
        var texto = tooltip.GetComponentInChildren<TextMeshProUGUI>();

        texto.text = mensagem;

        if (cor.HasValue)
        {
            texto.color = cor.Value;
        }
        else
        {
            Color corPadrao;
            ColorUtility.TryParseHtmlString("#FF7E0A", out corPadrao);
            texto.color = corPadrao;
        }
    }

    void OcultarTodosOsTooltips()
    {
        
        tooltipUsuario.SetActive(false);
        tooltipSenha.SetActive(false);
    }

    IEnumerator TrocarCenaAposDelay(float delay, int sceneIndex)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }
}
