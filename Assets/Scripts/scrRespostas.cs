using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrRespostas : MonoBehaviour
{
    public bool eCorreto;
    public scrQuiz quiz;
    public GameObject[] corretoImagem;
    public GameObject[] erroImagem;
    public float fadeDuration = 0.5f;
    public float displayTime = 1f;

    public void Answer()
    {
        if (eCorreto)
        {
            Debug.Log("Acertou!");
            quiz.Correto();
            StartCoroutine(ShowFeedback(corretoImagem));
        }
        else
        {
            Debug.Log("Errou!");
            StartCoroutine(ShowFeedback(erroImagem));
        }
    }

    private IEnumerator ShowFeedback(GameObject[] feedbackObjects)
    {
        foreach (GameObject obj in feedbackObjects)
        {
            obj.SetActive(true);
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = obj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        // Fade In
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            foreach (GameObject obj in feedbackObjects)
            {
                obj.GetComponent<CanvasGroup>().alpha = alpha;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (GameObject obj in feedbackObjects)
        {
            obj.GetComponent<CanvasGroup>().alpha = 1;
        }

        yield return new WaitForSeconds(displayTime);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            foreach (GameObject obj in feedbackObjects)
            {
                obj.GetComponent<CanvasGroup>().alpha = alpha;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (GameObject obj in feedbackObjects)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0;
            obj.SetActive(false);
        }
    }
}
