using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scrDestacarObjeto : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject objetoSelecionado;
    private GameObject ultimoSelecionado;

    public scrRemoveObjeto remove;

    // Lista pública de todos os objetos que já foram selecionados
    public List<GameObject> historicoSelecionados = new List<GameObject>();

    public LineRenderer lineRenderer;

    public GameObject inicio;

    public int contadorLer = 0;
    public int contadorDeclarar = 0;
    public int contadorAtribuir = 0;
    public int contadorExibir = 0;
    public int Fim = 0;

    public string[] sequenciaEsperada;

    public TextMeshProUGUI textoVerifica;

    public TextMeshProUGUI textoVitoria;


    public scrPainelDeslisante painelDeslizante;

    public scrPainelDeslisante painelVitoria;

    public scrPainelDeslisante painelTransicao;

    public int proximaCenaIndex;


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
        bool sequenciaCorreta = true;

        for (int i = 0; i < sequenciaEsperada.Length; i++)
        {
            if (i >= historicoSelecionados.Count)
            {
                Debug.Log($"Esperado: {sequenciaEsperada[i]} | Obtido: NULO - Faltando objeto na posição {i}");
                sequenciaCorreta = false;
                continue;
            }

            GameObject obj = historicoSelecionados[i];
            string nomeObjeto = obj.name;

            if (nomeObjeto == "I") // Se for um identificador ignorado
                continue;

            if (nomeObjeto == sequenciaEsperada[i])
            {
                Debug.Log($"CORRETO - Esperado: {sequenciaEsperada[i]} | Obtido: {nomeObjeto}");
                PintarOutline(obj, Color.green);
            }
            else
            {
                Debug.LogWarning($"INCORRETO - Esperado: {sequenciaEsperada[i]} | Obtido: {nomeObjeto}");
                sequenciaCorreta = false;
                PintarOutline(obj, Color.red);
            }
        }

        if (historicoSelecionados.Count > sequenciaEsperada.Length)
        {
            Debug.LogWarning("Existem objetos extras na sequência selecionada:");
            for (int i = sequenciaEsperada.Length; i < historicoSelecionados.Count; i++)
            {
                GameObject obj = historicoSelecionados[i];
                Debug.LogWarning($"Extra: {obj.name}");
            }
        }

        if (sequenciaCorreta)
        {
            painelVitoria.aberto = true;
            if (textoVitoria != null)
            {
               textoVitoria.text = "Parabéns! Você conseguiu!";
                StartCoroutine(ExecutarDepoisDeDoisSegundos());






            }
        }
        else
        {
            painelDeslizante.aberto = true;
            if (textoVerifica != null)
            {
                textoVerifica.text = "Corrija as peças nas posições erradas indicadas pelo contorno vermelho. Aperte o botão de voltar peça para fazer isso";
            }
        }


    }

    IEnumerator ExecutarDepoisDeDoisSegundos()
    {
        yield return new WaitForSeconds(2f);
        Invoke("CarregarProximaCena", 1.5f);

        painelTransicao.aberto = true;
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

    public void AtribuirIdentificadores()
    {
        contadorLer = 0;
        contadorDeclarar = 0;
        contadorAtribuir = 0;
        contadorExibir = 0;
        foreach (GameObject obj in historicoSelecionados)
        {
            AtribuirIdentificador(obj);
        }

        VerificarSequencia();
    }

    public void AtribuirIdentificador(GameObject objeto)
    {
        string identificador = "";


        switch (objeto.tag)
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
            case "Fim":
                identificador = "F" + Fim;
                break;
            default:
                identificador = "I";
                break;
        }

        objeto.name = identificador;
    }

    void CarregarProximaCena()
    {
        SceneManager.LoadScene(proximaCenaIndex);
        
    }
}
