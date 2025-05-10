using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class srcInstrucoes : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // Arraste o componente TMP aqui
    [TextArea(3, 10)]
    public string[] mensagens; // Array de blocos de texto
    public float velocidadeDigitacao = 0.05f;

    private int indexMensagem = 0;
    private bool textoEmDigitacao = false;
    private bool textoCompleto = false;

    void Start()
    {
        MostrarMensagemAtual();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textoEmDigitacao)
            {
                // Se clicar enquanto digita, mostra tudo de uma vez
                StopAllCoroutines();
                dialogueText.text = mensagens[indexMensagem];
                textoEmDigitacao = false;
                textoCompleto = true;
            }
            else if (textoCompleto)
            {
                ProximaMensagem();
            }
        }
    }

    void MostrarMensagemAtual()
    {
        dialogueText.text = "";
        StartCoroutine(DigitarTexto(mensagens[indexMensagem]));
    }

    IEnumerator DigitarTexto(string texto)
    {
        textoEmDigitacao = true;
        textoCompleto = false;

        foreach (char letra in texto)
        {
            dialogueText.text += letra;
            yield return new WaitForSeconds(velocidadeDigitacao);
        }

        textoEmDigitacao = false;
        textoCompleto = true;
    }

    void ProximaMensagem()
    {
        indexMensagem++;
        if (indexMensagem < mensagens.Length)
        {
            MostrarMensagemAtual();
        }
    }
}
