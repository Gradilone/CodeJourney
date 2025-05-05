using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrManager : MonoBehaviour
{
    public int instanciados;
    public int qtdTotal;
    public int qtdTotalParaFim;

    public Button botaoFim;
    public Button botaoConfirmar;

    private void Start()
    {
        botaoFim.interactable = false;
        botaoConfirmar.interactable = false;
    }
    private void Update()
    {
        if (instanciados == qtdTotalParaFim)
        {
            botaoFim.interactable = true;
        }

        if (instanciados == qtdTotal)
        {
            botaoConfirmar.interactable = true;
        }

    }

}
