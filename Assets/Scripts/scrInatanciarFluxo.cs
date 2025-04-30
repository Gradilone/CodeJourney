using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scrInatanciarFluxo : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject prefabParaInstanciar;
    public LineRenderer linhaRaycast;

    public Canvas canvasWorldSpace;         // <- Referência ao Canvas em World Space
    public GameObject textoPrefabUI;


    public float distanciaParaInstanciar = 1f;

    private GameObject prefabInstanciado;

    public scrDestacarObjeto destacador;

    public int maxUso = 2; // Quantidade máxima de vezes que o botão pode ser usado
    private int vezesUsado = 0; // Contador de vezes que o botão foi usado

    public Button meuBotao; // Referência ao botão

    [Header("Textos para cada instância")]
    public List<string> textosInstancia;

    public void InstanciarPrefab()
    {
        if (destacador == null || destacador.objetoSelecionado == null)
        {
            Debug.LogWarning("Nenhum objeto selecionado!");
            return;
        }

        Vector3 posicaoInstanciada = destacador.objetoSelecionado.transform.position + Vector3.down * distanciaParaInstanciar;

        prefabInstanciado = Instantiate(prefabParaInstanciar, posicaoInstanciada, Quaternion.identity);

        string texto = textosInstancia[vezesUsado];
        float escalaPorCaractere = 0.09f; // Ajuste esse valor conforme o modelo
        float larguraMinima = 1f;
        float novaLargura = Mathf.Max(larguraMinima, texto.Length * escalaPorCaractere);

        // Supondo que o prefab instanciado tenha um transform padrão (sem RectTransform)
        Vector3 novaEscala = prefabInstanciado.transform.localScale;
        novaEscala.x = novaLargura;
        prefabInstanciado.transform.localScale = novaEscala;

        // Instancia o texto no Canvas World Space
        GameObject textoInstanciado = Instantiate(textoPrefabUI, canvasWorldSpace.transform);
        textoInstanciado.transform.position = posicaoInstanciada;
        textoInstanciado.transform.rotation = Quaternion.identity;

        TextMeshProUGUI tmp = textoInstanciado.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = texto;
        }

        destacador.objetoSelecionado = prefabInstanciado;

        vezesUsado++;

        if (vezesUsado >= maxUso)
        {
            meuBotao.interactable = false; 
        }

    }

    void OnDrawGizmos()
    {
        if (destacador.objetoSelecionado != null && prefabInstanciado != null)
        {
            Gizmos.color = Color.red;  // Cor da linha
            Gizmos.DrawLine(destacador.objetoSelecionado.transform.position, prefabInstanciado.transform.position);  // Linha entre os objetos
        }
    }





}
