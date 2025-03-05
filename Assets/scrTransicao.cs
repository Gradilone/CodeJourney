using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scrTransicao : MonoBehaviour
{
    public int sceneIndex; // �ndice da cena a ser carregada

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
