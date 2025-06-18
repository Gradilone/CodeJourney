using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrGerenciaMapa : MonoBehaviour
{
    public scrGerenciaFase gerenciaFase;
    public string jornada;

    private void Start()
    {

        gerenciaFase = GameObject.Find("GameManager").GetComponent<scrGerenciaFase>();

    }

}
