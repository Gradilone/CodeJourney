using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrTransFase : MonoBehaviour
{
    public int sceneIndex;
    public void AoClicarNoBotaoFase(GameObject botaoFase)
    {
        string nomeFase = botaoFase.name; // Nome do bot�o � o nome da fase (ex: "Fase1", "Fase2", etc.)
        scrGerenciaFase.instance.DefinirFase(nomeFase);
        // Carrega a primeira cena da fase (modelo)
        LoadSceneByIndex();
    }

    public void LoadSceneByIndex()
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError("�ndice de cena inv�lido! Verifique o Build Settings.");
        }
    }
}
