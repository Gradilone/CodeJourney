using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrDestacarObjeto : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject objetoSelecionado;
    private GameObject contorno;


    void Start()
    {
        GameObject objetoInicial = GameObject.Find("Inicio");
        SelecionarObjeto(objetoInicial);
    }

    void Update()
    {
        if (objetoSelecionado != null)
        {
            DestacarSelecionado(objetoSelecionado);
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
        if (contorno != null)
            Destroy(contorno);

        // Obtém o SpriteRenderer do objeto alvo
        SpriteRenderer sr = alvo.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogWarning("Objeto não tem SpriteRenderer!");
            return;
        }

        // Cria um novo GameObject para o contorno
        contorno = new GameObject("Contorno");
        contorno.transform.SetParent(alvo.transform);
        contorno.transform.localPosition = Vector3.zero;
        contorno.transform.localScale = Vector3.one * 1.1f; // Ligeiramente maior que o objeto original

        // Adiciona um SpriteRenderer para o contorno
        SpriteRenderer srContorno = contorno.AddComponent<SpriteRenderer>();
        srContorno.sprite = sr.sprite;
        srContorno.sortingLayerID = sr.sortingLayerID;

        // Define um sortingOrder fixo para o contorno, por exemplo, 0
        srContorno.sortingOrder = 0; // Valor fixo, o contorno ficará atrás do objeto original
        srContorno.color = Color.red; // Define a cor do contorno como amarela
    }

    // Remover o destaque (contorno) do objeto
    void RemoverDestaque(GameObject alvo)
    {
        if (alvo != null && alvo.transform.Find("Contorno"))
        {
            Destroy(alvo.transform.Find("Contorno").gameObject);
        }
    }

    public GameObject GetObjetoSelecionado()
    {
        return objetoSelecionado;
    }
}
