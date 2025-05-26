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
    public class ProgressoData
    {
        public int nivel;
        public float progresso;
    }

    [System.Serializable]
    public class JornadaData
    {
        public int usuarioId;
        public string jornNome;
        public string jornFase;
        public int jornEstrelas;
        public int jornUltimaFase;
    }

    [System.Serializable]
    public class LoginData
    {
        public string userName;
        public string senha;
    }

    [Header("Configura��o da API")]
    public string apiUrl = "https://localhost:44386/";
    public string apiUrlCadastro => $"{apiUrl}api/usuarios/cadastro";
    public string apiUrlLogin => $"{apiUrl}api/usuarios/login";
    public string apiUrlGetUsuario => $"{apiUrl}api/usuarios/";
    public string apiUrlAtualizarUsuario => $"{apiUrl}api/usuarios/atualizar/";
    public string apiNivelProgresso => $"{apiUrl}api/usuarios/";
    public string apiInserirFase => $"{apiUrl}api/jornada/inserir";
    public string apiGetJornada => $"{apiUrl}api/jornada/pegar";
    public string apiAtualizarJornada => $"{apiUrl}api/jornada/atualizar/";

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
        string url = apiUrlGetUsuario + id;  

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

    public IEnumerator GetNivelEProgresso(int userId, string token, Action<ProgressoData> onSuccess, Action<long, string> onError)
    {
        string url = $"{apiNivelProgresso}{userId}/nivel-progresso";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var json = request.downloadHandler.text;
                    ProgressoData progresso = JsonUtility.FromJson<ProgressoData>(json);
                    onSuccess?.Invoke(progresso);
                }
                catch (Exception e)
                {
                    Debug.LogError("Erro ao desserializar ProgressoData: " + e.Message);
                    onError?.Invoke(request.responseCode, e.Message);
                }
            }
            else
            {
                Debug.LogError($"Erro ao buscar n�vel e progresso: {request.error} | C�digo: {request.responseCode}");
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }

    // M�todo para atualizar n�vel e progresso do usu�rio por ID
    public IEnumerator UpdateNivelEProgresso(int userId, ProgressoData progressoData, string token, Action<string> onSuccess, Action<long, string> onError)
    {
        string url = $"{apiNivelProgresso}{userId}/nivel-progresso";
        string jsonData = JsonUtility.ToJson(progressoData);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("N�vel e progresso atualizados com sucesso: " + request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Erro ao atualizar n�vel e progresso: {request.error} | C�digo: {request.responseCode}");
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }

    public IEnumerator InserirJornada(JornadaData dados, string token, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        string jsonData = JsonUtility.ToJson(dados);
        using (UnityWebRequest request = new UnityWebRequest(apiInserirFase, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            Debug.Log("Enviando requisi��o de Inser��o de Jornada para: " + apiInserirFase);
            Debug.Log("Dados enviados: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Jornada inserida com sucesso: " + request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Erro ao inserir jornada: " + request.error + " | C�digo: " + request.responseCode);
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }

    public IEnumerator BuscarJornada(int usuarioId, string jornNome, string token, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        string url = apiGetJornada + "?usuarioId=" + UnityWebRequest.EscapeURL(usuarioId.ToString()) + "&jornNome=" + UnityWebRequest.EscapeURL(jornNome);

        Debug.Log("Preparando requisi��o GET para: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Jornada recebida: " + request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Erro ao buscar jornada: " + request.error + " | C�digo: " + request.responseCode);
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }

    // M�todo para atualizar jornada
    public IEnumerator AtualizarJornada(int usuarioId, JornadaData dadosAtualizados, string token, Action<string> onSuccess = null, Action<long, string> onError = null)
    {
        string url = apiAtualizarJornada + usuarioId;
        string jsonData = JsonUtility.ToJson(dadosAtualizados);

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            Debug.Log("Enviando requisi��o de Atualiza��o de Jornada para: " + url);
            Debug.Log("Dados enviados: " + jsonData);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Jornada atualizada com sucesso: " + request.downloadHandler.text);
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Erro ao atualizar jornada: " + request.error + " | C�digo: " + request.responseCode);
                onError?.Invoke(request.responseCode, request.error);
            }
        }
    }
}
