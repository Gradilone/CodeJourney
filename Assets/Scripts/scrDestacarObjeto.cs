using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrDestacarObjeto : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject objetoSelecionado;
    private GameObject ultimoSelecionado;

    // Lista pública de todos os objetos que já foram selecionados
    public List<GameObject> historicoSelecionados = new List<GameObject>();



    void Start()
    {
        GameObject objetoInicial = GameObject.Find("Inicio");
        SelecionarObjeto(objetoInicial);
        ultimoSelecionado = null;
    }

    void Update()
    {
        if (objetoSelecionado != ultimoSelecionado)
        {
            // Remover destaque do antigo
            if (ultimoSelecionado != null)
            {
                RemoverDestaque(ultimoSelecionado);
            }

            // Adicionar destaque ao novo
            if (objetoSelecionado != null)
            {
                DestacarSelecionado(objetoSelecionado);
                if (!historicoSelecionados.Contains(objetoSelecionado))
                {
                    historicoSelecionados.Add(objetoSelecionado);
                }
            }

            // Atualizar referência
            ultimoSelecionado = objetoSelecionado;
        }

    }

  
    public void SelecionarObjeto(GameObject objeto)
    {
        if (objetoSelecionado != null)
        {
            // Remover contorno do objeto anterior
            RemoverDestaque(objetoSelecionado);
        }

        // Atualizar a referência do objeto selecionado
        objetoSelecionado = objeto;

        // Destacar o novo objeto
        DestacarSelecionado(objetoSelecionado);
    }

    // Destacar o objeto selecionado com um contorno
    void DestacarSelecionado(GameObject alvo)
    {
        if (alvo.transform.childCount > 0)
        {
            Transform filho = alvo.transform.GetChild(0);
            SpriteRenderer srFilho = filho.GetComponent<SpriteRenderer>();

            if (srFilho != null)
            {
                srFilho.enabled = true;
            }
        }
    }

    void RemoverDestaque(GameObject alvo)
    {
        if (alvo.transform.childCount > 0)
        {
            Transform filho = alvo.transform.GetChild(0);
            SpriteRenderer srFilho = filho.GetComponent<SpriteRenderer>();

            if (srFilho != null)
            {
                srFilho.enabled = false;
            }
        }
    }
    public GameObject GetObjetoSelecionado()
    {
        return objetoSelecionado;
    }
}
