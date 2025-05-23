using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RespostaLogin
{
    public string tokenName;
    public int id;
}

[System.Serializable]
public class DadosUsuarioCompleto
{
    public string nome;
    public string userName;
    public string email;
    public string dataNascimento;
    public string dataCadastro;
}
public class scrAutenticador : MonoBehaviour
{
    public static scrAutenticador Instance;

    public string bearerToken;
    public int usuarioId;

    public string nome;
    public string userName;
    public string email;
    public string dataNascimento;
    public string dataCadastro;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
