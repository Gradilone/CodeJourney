using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scrValidaFormulario : MonoBehaviour
{
    public TMP_InputField inpNome;
    public TMP_InputField inpUsuario;
    public TMP_InputField inpEmail;
    public TMP_InputField inpDia;
    public TMP_InputField inpMes;
    public TMP_InputField inpAno;
    public TMP_InputField inpSenha;
    public TMP_InputField inpConfirmaSenha;
    public Button btnLogin;

    private List<Selectable> tabOrder;

    public GameObject tooltipNome;
    public GameObject tooltipUsuario;
    public GameObject tooltipEmail;
    public GameObject tooltipData;
    public GameObject tooltipSenha;
    public GameObject tooltipConfirmaSenha;
    public GameObject tooltipConfirmacao;

    public int sceneIndex;


    void Start()
    {
        btnLogin.onClick.AddListener(ValidarFormulario);

        tabOrder = new List<Selectable> {
            inpNome,
            inpUsuario,
            inpEmail,
            inpDia,
            inpMes,
            inpAno,
            inpSenha,
            inpConfirmaSenha
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

        bool valido = true;

        // Nome
        if (string.IsNullOrWhiteSpace(inpNome.text))
        {
            MostrarTooltip(tooltipNome, "Preencha o nome.");
            valido = false;
        }
        else if (!Regex.IsMatch(inpNome.text, @"^[a-zA-Z0-9\s]+$"))
        {
            MostrarTooltip(tooltipNome, "Nome não pode conter caracteres especiais.");
            valido = false;
        }

        // Usuário
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


        // Email
        if (string.IsNullOrWhiteSpace(inpEmail.text))
        {
            MostrarTooltip(tooltipEmail, "Preencha o e-mail.");
            valido = false;
        }
        else
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(inpEmail.text, pattern))
            {
                MostrarTooltip(tooltipEmail, "Formato de e-mail inválido.");
                valido = false;
            }
        }

        // Data
        bool dataValida = true;
        if (!int.TryParse(inpDia.text, out int dia) || dia < 1 || dia > 31) dataValida = false;
        if (!int.TryParse(inpMes.text, out int mes) || mes < 1 || mes > 12) dataValida = false;
        if (!int.TryParse(inpAno.text, out int ano) || ano < 1900 || ano > DateTime.Now.Year) dataValida = false;

        if (dataValida)
        {
            try
            {
                DateTime dt = new DateTime(ano, mes, dia);
            }
            catch
            {
                dataValida = false;
            }
        }

        if (!dataValida)
        {
            MostrarTooltip(tooltipData, "Data inválida.");
            valido = false;
        }

        // Senha
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

        // Confirmação da senha
        if (inpSenha.text != inpConfirmaSenha.text)
        {
            MostrarTooltip(tooltipConfirmaSenha, "As senhas não coincidem.");
            valido = false;
        }

        if (valido)
        {
            Debug.Log("Formulário válido!");
            MostrarTooltip(tooltipConfirmacao, "Conta cadastrada com sucesso!");
            StartCoroutine(TrocarCenaAposDelay(1.5f, sceneIndex));

        }
    }

    void MostrarTooltip(GameObject tooltip, string mensagem)
    {
        tooltip.SetActive(true);
        tooltip.GetComponentInChildren<TextMeshProUGUI>().text = mensagem;
    }

    void OcultarTodosOsTooltips()
    {
        tooltipNome.SetActive(false);
        tooltipUsuario.SetActive(false);
        tooltipEmail.SetActive(false);
        tooltipData.SetActive(false);
        tooltipSenha.SetActive(false);
        tooltipConfirmaSenha.SetActive(false);
        tooltipConfirmacao.SetActive(false);
    }

    IEnumerator TrocarCenaAposDelay(float delay, int sceneIndex)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }

}
