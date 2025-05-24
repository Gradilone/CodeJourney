using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class scrConexaoAPI : MonoBehaviour
{
    [System.Serializable]
    public class CadastroData
    {
        public string nome;
        public string userName;
        public string email;
        public string dataNascimento; 
        public string senha;
    }

    [System.Serializable]
    public class LoginData
    {
        public string userName;
        public string senha;
    }

    [Header("Configura��o da API")]
    public string apiUrlCadastro = "https://localhost:44386/api/usuarios/cadastro";
    public string apiUrlLogin = "https://localhost:44386/api/usuarios/login";
    public string apiUrlGetUsuario = "https://localhost:44386/api/usuarios/";
    public string apiUrlAtualizarUsuario = "https://localhost:44386/api/usuarios/atualizar/";

    private IEnumerator EnviarPostRequest(string url, string jsonData, Action<string> onSuccess, Action<long, string> onError)
    {
        Debug.Log("Preparando requisi��o para: " + url);
        Debug.Log("Dados enviados: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Enviando requisi��o para: " + url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Resposta recebida: " + request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Erro na requisi��o: " + request.error + " | C�digo: " + request.responseCode);
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }

    // Ajuste o EnviarLogin tamb�m:
    public IEnumerator EnviarLogin(LoginData dados, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        string jsonData = JsonUtility.ToJson(dados);
        yield return EnviarPostRequest(apiUrlLogin, jsonData, onSuccess, onError);
    }

    public IEnumerator EnviarCadastro(CadastroData dados, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        string jsonData = JsonUtility.ToJson(dados);
        yield return EnviarPostRequest(apiUrlCadastro, jsonData, onSuccess, onError);
    }

    public IEnumerator BuscarDadosCompletosUsuario(int id, string token, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        string url = apiUrlGetUsuario + id;

        Debug.Log("Preparando requisi��o GET para: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Dados completos recebidos: " + request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Erro ao buscar dados completos: " + request.error + " | C�digo: " + request.responseCode);
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }

    public IEnumerator AtualizarUsuario(int id, CadastroData dadosAtualizados, string token, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        string url = apiUrlAtualizarUsuario + id;
        string jsonData = JsonUtility.ToJson(dadosAtualizados);

        Debug.Log("Preparando requisi��o PUT para: " + url);
        Debug.Log("Dados enviados: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            Debug.Log("Enviando requisi��o para: " + url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Resposta recebida: " + request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Erro na requisi��o: " + request.error + " | C�digo: " + request.responseCode);
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }

    public IEnumerator DeletarUsuario(int id, string token, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        string url = apiUrlGetUsuario + id;  // ou apiUrlDeletarUsuario se tiver um endpoint espec�fico para DELETE.

        Debug.Log("Preparando requisi��o DELETE para: " + url);

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.Log("Usu�rio deletado com sucesso: " + request.downloadHandler.text);
                    onSuccess?.Invoke(request.downloadHandler.text);
                }
                else
                {
                    Debug.Log("Usu�rio deletado com sucesso, sem resposta de conte�do.");
                    onSuccess?.Invoke("Usu�rio deletado com sucesso.");
                }
            }
            else
            {
                Debug.LogError("Erro na requisi��o: " + request.error + " | C�digo: " + request.responseCode);
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }
}
