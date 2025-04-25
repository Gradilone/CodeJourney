using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrInatanciarFluxo : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject prefabParaInstanciar;
    public LineRenderer linhaRaycast;

    public float distanciaParaInstanciar = 1f;

    private GameObject objetoSelecionado;
    private GameObject prefabInstanciado;
    private GameObject contorno;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clique do mouse
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);  

            if (hit.collider != null)
            {
                objetoSelecionado = hit.collider.gameObject;
                Debug.Log("Selecionado (2D): " + objetoSelecionado.name);
                DestacarSelecionado(objetoSelecionado);
            }



        }
    }

    public void InstanciarPrefab()
    {
        if (objetoSelecionado == null)
        {
            Debug.LogWarning("Nenhum objeto selecionado!");
            return;
        }

        // Calcula posição mais distante abaixo do objeto (ex: 5 metros)
        Vector3 posicaoInstanciada = objetoSelecionado.transform.position + Vector3.down * distanciaParaInstanciar;

       // Instancia o prefab
       prefabInstanciado = Instantiate(prefabParaInstanciar, posicaoInstanciada, Quaternion.identity);

        objetoSelecionado = prefabInstanciado;

        DestacarSelecionado(prefabInstanciado);
    }

    // Desenha a linha no Editor com Gizmos
    void OnDrawGizmos()
    {
        if (objetoSelecionado != null && prefabInstanciado != null)
        {
            Gizmos.color = Color.red;  // Cor da linha
            Gizmos.DrawLine(objetoSelecionado.transform.position, prefabInstanciado.transform.position);  // Linha entre os objetos
        }
    }

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
        srContorno.color = Color.yellow; // Define a cor do contorno como vermelha
    }



}
