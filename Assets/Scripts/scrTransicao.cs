using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scrTransicao : MonoBehaviour
{
    public int sceneIndex;
    public float deelay = 0f;

    public void LoadSceneByIndex()
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadSceneWithDelay());
        }
        else
        {
            Debug.LogError("Índice de cena inválido! Verifique o Build Settings.");
        }
    }

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(deelay); // Espera o tempo definido no inspector
        SceneManager.LoadScene(sceneIndex);
    }
}
