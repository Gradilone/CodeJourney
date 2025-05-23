using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scrExibirDadosUsuario : MonoBehaviour
{
    [Header("Campos de texto")]
    public TextMeshProUGUI txtNomeCompleto;
    public TextMeshProUGUI txtIdade;
    public TextMeshProUGUI txtDataCadastro;
    public TextMeshProUGUI txtUsuario;
    public TextMeshProUGUI txtUserName;

    void Start()
    {
        ExibirDadosUsuario();
    }

    void ExibirDadosUsuario()
    {
        string nomeCompleto = scrAutenticador.Instance.nome;
        string dataNascimentoStr = scrAutenticador.Instance.dataNascimento;
        string dataCadastroStr = scrAutenticador.Instance.dataCadastro;
        string userName = scrAutenticador.Instance.userName;
        int userId = scrAutenticador.Instance.usuarioId;

        // Calcula a idade
        int idade = CalcularIdade(dataNascimentoStr);

        // Formata a data de cadastro
        string dataCadastroFormatada = FormatarData(dataCadastroStr);

        // Exibe nos campos

        if (txtUsuario != null)
            txtUsuario.text = userName;

        if (txtUserName != null)
            txtUserName.text = $"#{userId}";

        if (txtNomeCompleto != null)
            txtNomeCompleto.text = nomeCompleto;

        if (txtIdade != null)
            txtIdade.text = $"{idade} anos";

        if (txtDataCadastro != null)
            txtDataCadastro.text = $"Conta criada em: {dataCadastroFormatada}";

        Debug.Log($"Dados exibidos: {nomeCompleto}, {idade} anos, criado em {dataCadastroFormatada}");
    }

    int CalcularIdade(string dataNascimentoStr)
    {
        if (DateTime.TryParse(dataNascimentoStr, out DateTime dataNascimento))
        {
            DateTime hoje = DateTime.Today;
            int idade = hoje.Year - dataNascimento.Year;

            if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;

            return idade;
        }
        else
        {
            Debug.LogWarning("Data de nascimento inválida: " + dataNascimentoStr);
            return 0;
        }
    }

    string FormatarData(string dataStr)
    {
        if (DateTime.TryParse(dataStr, out DateTime data))
        {
            return data.ToString("dd/MM/yyyy");
        }
        else
        {
            Debug.LogWarning("Data de cadastro inválida: " + dataStr);
            return "Data inválida";
        }
    }
}
