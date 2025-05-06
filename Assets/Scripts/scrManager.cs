using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrManager : MonoBehaviour
{
    public int instanciados;
    public int qtdTotal;

    public Button botaoConfirmar;

    private void Start()
    {
        botaoConfirmar.interactable = false;
    }
    private void Update()
    {

        if (instanciados == qtdTotal)
        {
            botaoConfirmar.interactable = true;
        }
        else
        {
            botaoConfirmar.interactable = false;
        }

    }

}
