using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static scrConexaoAPI;


public class scrValidaConfig : MonoBehaviour
{
    public TMP_InputField inpNome;
    public TMP_InputField inpUsuario;
    public TMP_InputField inpEmail;
    public TMP_InputField inpDia;
    public TMP_InputField inpMes;
    public TMP_InputField inpAno;
    public TMP_InputField inpSenha;

    public Button bntNome;
    public Button bntUsuario;
    public Button bntEmail;
    public Button bntData;
    public Button bntSenha;

    public GameObject tooltipConfirmacao;

    private scrAutenticador autenticador;
    public scrConexaoAPI conexaoAPI;

    void Start()
    {
        bntNome.GetComponent<Button>().onClick.AddListener(ValidarNome);
        bntUsuario.GetComponent<Button>().onClick.AddListener(ValidarUsuario);
        bntEmail.GetComponent<Button>().onClick.AddListener(ValidarEmail);
        bntData.GetComponent<Button>().onClick.AddListener(ValidarData);
        bntSenha.GetComponent<Button>().onClick.AddListener(ValidarSenha);

        GameObject objAutenticador = GameObject.Find("Autenticador");
        autenticador = objAutenticador.GetComponent<scrAutenticador>();

        OcultarTooltip();
    }

    void ValidarNome()
    {
        string novoNome = inpNome.text.Trim();
        if (string.IsNullOrWhiteSpace(inpNome.text))
        {
            MostrarTooltip("Preencha o nome.", Color.red);
            return;
        }


        CadastroData data = new CadastroData
        {
            nome = novoNome,
            userName = null,
            email = null,
            dataNascimento = "0001-01-01T00:00:00",
            senha = null
        };

        StartCoroutine(AtualizarCampo(data));
    }

    void ValidarUsuario()
    {
        string novoUsuario = inpUsuario.text.Trim();
        if (string.IsNullOrWhiteSpace(inpUsuario.text))
        {
            MostrarTooltip("Preencha o usuário.", Color.red);
            return;
        }
        if (!System.Text.RegularExpressions.Regex.IsMatch(inpUsuario.text, @"^[a-zA-Z0-9\s]+$"))
        {
            MostrarTooltip("Usuário não pode conter caracteres especiais.", Color.red);
            return;
        }

        CadastroData data = new CadastroData
        {
            nome = null,
            userName = novoUsuario,
            email = null,
            dataNascimento = "0001-01-01T00:00:00",
            senha = null
        };

        StartCoroutine(AtualizarCampo(data));
    }

    void ValidarEmail()
    {
        string novoEmail = inpEmail.text.Trim();
        if (string.IsNullOrWhiteSpace(inpEmail.text))
        {
            MostrarTooltip("Preencha o email.", Color.red);
            return;
        }
        if (!System.Text.RegularExpressions.Regex.IsMatch(inpEmail.text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MostrarTooltip("Email inválido.", Color.red);
            return;
        }

        CadastroData data = new CadastroData
        {
            nome = null,
            userName = null,
            email = novoEmail,
            dataNascimento = "0001-01-01T00:00:00",
            senha = null
        };

        StartCoroutine(AtualizarCampo(data));
    }

    void ValidarData()
    {

        int dia, mes, ano;

        if (!int.TryParse(inpDia.text, out dia) || dia < 1 || dia > 31)
        {
            MostrarTooltip("Dia inválido.", Color.red);
            return;
        }
        if (!int.TryParse(inpMes.text, out mes) || mes < 1 || mes > 12)
        {
            MostrarTooltip("Mês inválido.", Color.red);
            return;
        }
        if (!int.TryParse(inpAno.text, out ano) || ano < 1900 || ano > System.DateTime.Now.Year)
        {
            MostrarTooltip("Ano inválido.", Color.red);
            return;
        }
        int maxDia = DateTime.DaysInMonth(ano, mes);
        if (dia > maxDia)
        {
            MostrarTooltip($"Dia inválido para o mês {mes}.", Color.red);
            return;
        }

        DateTime dataNascimento = new DateTime(ano, mes, dia);
        string novaData = dataNascimento.ToString("yyyy-MM-ddTHH:mm:ss");

        CadastroData data = new CadastroData
        {
            nome = null,
            userName = null,
            email = null,
            dataNascimento = novaData,
            senha = null
        };

        StartCoroutine(AtualizarCampo(data));
    }

    void ValidarSenha()
    {
        string novaSenha = inpSenha.text.Trim();
        if (string.IsNullOrWhiteSpace(inpSenha.text) || inpSenha.text.Length < 5)
        {
            MostrarTooltip("A senha deve ter no mínimo 5 caracteres.", Color.red);
            return;
        }

        CadastroData data = new CadastroData
        {
            nome = null,
            userName = null,
            email = null,
            dataNascimento = "0001-01-01T00:00:00",
            senha = novaSenha
        };

        StartCoroutine(AtualizarCampo(data));
    }

    void MostrarTooltip(string mensagem, Color cor)
    {
        tooltipConfirmacao.SetActive(true);
        var texto = tooltipConfirmacao.GetComponentInChildren<TextMeshProUGUI>();
        texto.text = mensagem;
        texto.color = cor;
    }

    void OcultarTooltip()
    {
        tooltipConfirmacao.SetActive(false);
    }

    IEnumerator AtualizarCampo(scrConexaoAPI.CadastroData dadosAtualizados)
    {
        if (autenticador == null)
        {
            MostrarTooltip("Erro: autenticador não encontrado.", Color.red);
            yield break;
        }

        string token = autenticador.bearerToken;
        int userId = autenticador.usuarioId;

        MostrarTooltip("Enviando ", Color.yellow);

        yield return StartCoroutine(conexaoAPI.AtualizarUsuario(userId, dadosAtualizados, token,
            onSuccess: (resposta) =>
            {
                MostrarTooltip("Atualizado com sucesso!", Color.green);
                Debug.Log("Resposta API: " + resposta);

                if (!string.IsNullOrEmpty(dadosAtualizados.nome))
                    autenticador.nome = dadosAtualizados.nome;

                if (!string.IsNullOrEmpty(dadosAtualizados.userName))
                    autenticador.userName = dadosAtualizados.userName;

                if (!string.IsNullOrEmpty(dadosAtualizados.email))
                    autenticador.email = dadosAtualizados.email;

                if (!string.IsNullOrEmpty(dadosAtualizados.dataNascimento) && dadosAtualizados.dataNascimento != "0001-01-01T00:00:00")
                    autenticador.dataNascimento = dadosAtualizados.dataNascimento;
            },
            onError: (codigo, erro) =>
            {
                if (codigo == 409)
                {
                    MostrarTooltip("Já existe um usuário com esse dado.", Color.red);
                }
                else
                {
                    MostrarTooltip($"Erro {codigo}: {erro}", Color.red);
                }
                Debug.LogError($"Erro na atualização: {codigo} - {erro}");
            }));
    }

    public void DeletarConta()
    {
        if (autenticador == null)
        {
            MostrarTooltip("Erro: autenticador não encontrado.", Color.red);
            return;
        }

        int userId = autenticador.usuarioId;
        string token = autenticador.bearerToken;

        MostrarTooltip("Deletando conta...", Color.yellow);

        StartCoroutine(DeletarContaCoroutine(userId, token));
    }

    private IEnumerator DeletarContaCoroutine(int userId, string token)
    {
        yield return StartCoroutine(conexaoAPI.DeletarUsuario(userId, token,
            onSuccess: (resposta) =>
            {
                MostrarTooltip("Conta deletada com sucesso!", Color.green);
                Debug.Log("Conta deletada: " + resposta);
                StartCoroutine(VoltarParaIndex());
            },
            onError: (codigo, erro) =>
            {
                MostrarTooltip($"Erro {codigo}: {erro}", Color.red);
                Debug.LogError($"Erro ao deletar conta: {codigo} - {erro}");
            }));
    }

    private IEnumerator VoltarParaIndex()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(1); // Carrega a cena de índice 1
    }

}
