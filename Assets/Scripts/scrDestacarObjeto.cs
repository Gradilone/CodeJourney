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

    public LineRenderer lineRenderer;

    public GameObject inicio;

    public int contadorLer = 0;
    public int contadorDeclarar = 0;
    public int contadorAtribuir = 0;
    public int contadorExibir = 0;
    public int Fim = 0;

    // Dicionário opcional se quiser rastrear identificadores
    public Dictionary<GameObject, string> identificadores = new Dictionary<GameObject, string>();

    public string[] sequenciaEsperada;




    void Start()
    {
        GameObject objetoInicial = GameObject.Find("Inicio");
        SelecionarObjeto(objetoInicial);
        ultimoSelecionado = null;

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.sortingOrder = 10;
        lineRenderer.enabled = false;
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

                    string identificador = "";

                    switch (objetoSelecionado.tag)
                    {
                        case "Ler":
                            contadorLer++;
                            identificador = "L" + contadorLer;
                            break;
                        case "Declarar":
                            contadorDeclarar++;
                            identificador = "D" + contadorDeclarar;
                            break;
                        case "Atribuir":
                            contadorAtribuir++;
                            identificador = "A" + contadorAtribuir;
                            break;
                        case "Exibir":
                            contadorExibir++;
                            identificador = "E" + contadorExibir;
                            break;
                        case "Fim":;
                            identificador = "F" + Fim;
                            break;
                        default:
                            identificador = "I";
                            break;
                    }

                    identificadores[objetoSelecionado] = identificador;

                    // Opcional: você pode exibir no nome ou em um texto do objeto
                    objetoSelecionado.name = identificador; // ou use TMP para mostrar na UI

                }

                if (inicio != null && objetoSelecionado != null)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.SetPosition(0, inicio.transform.position);
                    lineRenderer.SetPosition(1, objetoSelecionado.transform.position);
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
    public void VerificarSequencia()
    {

        for (int i = 0; i < sequenciaEsperada.Length; i++)
        {
            if (i >= historicoSelecionados.Count)
            {
                Debug.Log($"Esperado: {sequenciaEsperada[i]} | Obtido: NULO - Faltando objeto na posição {i}");
                continue;
            }

            GameObject obj = historicoSelecionados[i];

            if (identificadores.ContainsKey(obj))
            {
                string identificador = identificadores[obj];

                if (identificador == "I")
                    continue;

                if (identificador == sequenciaEsperada[i])
                {
                    Debug.Log($"CORRETO - Esperado: {sequenciaEsperada[i]} | Obtido: {identificador}");
                    PintarOutline(obj, Color.green);
                }
                else
                {
                    Debug.LogWarning($"INCORRETO - Esperado: {sequenciaEsperada[i]} | Obtido: {identificador}");
                    PintarOutline(obj, Color.red);
                }
            }
            else
            {
                Debug.LogError($"Objeto na posição {i} não tem identificador registrado.");
            }
        }

        if (historicoSelecionados.Count > sequenciaEsperada.Length)
        {
            Debug.LogWarning("Existem objetos extras na sequência selecionada:");
            for (int i = sequenciaEsperada.Length; i < historicoSelecionados.Count; i++)
            {
                if (identificadores.ContainsKey(historicoSelecionados[i]))
                {
                    Debug.LogWarning($"Extra: {identificadores[historicoSelecionados[i]]}");
                }
                else
                {
                    Debug.LogWarning($"Extra sem identificador: {historicoSelecionados[i].name}");
                }
            }
        }
    }

    void PintarOutline(GameObject obj, Color cor)
    {
        if (obj.transform.childCount > 0)
        {
            Transform filho = obj.transform.GetChild(0);
            SpriteRenderer sr = filho.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.enabled = true;
                sr.color = cor;
            }
        }
    }
}
