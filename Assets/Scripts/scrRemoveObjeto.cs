using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrRemoveObjeto : MonoBehaviour
{
    public GameObject inicio;
    public scrDestacarObjeto destacador;                // Refer�ncia ao script que guarda o objeto selecionado
    public scrManager manager;

    public scrInatanciarFluxo ler;
    public scrInatanciarFluxo exibir;
    public scrInatanciarFluxo atribuir;
    public scrInatanciarFluxo declarar;


    public void RemoverObjetoSelecionado()
    {
        if (destacador == null || destacador.objetoSelecionado == null || destacador.objetoSelecionado == inicio)
        {
            Debug.LogWarning("Nenhum objeto selecionado para remover.");
            return;
        }

        GameObject objetoRemovido = destacador.objetoSelecionado;

        srcIdentificadorInstanciador marcador = objetoRemovido.GetComponent<srcIdentificadorInstanciador>();
        Destroy(marcador.textoUI);

        manager.instanciados--;

        if (destacador.contadorLer > 0) destacador.contadorLer--;
        if (destacador.contadorExibir > 0) destacador.contadorExibir--;
        if (destacador.contadorDeclarar > 0) destacador.contadorDeclarar--;
        if (destacador.contadorAtribuir > 0) destacador.contadorAtribuir--;
        if (destacador.Fim > 0) destacador.Fim--;


        // Remove o objeto da cena
        Destroy(objetoRemovido);

        // Verifica se ainda h� hist�rico de objetos selecionados
        if (destacador.historicoSelecionados != null && destacador.historicoSelecionados.Count > 0)
        {
            // Remove o �ltimo item (que foi destru�do)
            destacador.historicoSelecionados.Remove(objetoRemovido);

            srcIdentificadorInstanciador identificador = objetoRemovido.GetComponent<srcIdentificadorInstanciador>();
            if (identificador != null && identificador.instanciador != null)
            {
                identificador.instanciador.vezesUsado = Mathf.Max(0, identificador.instanciador.vezesUsado - 1);
            }

            // Define o novo objetoSelecionado como o �ltimo da lista
            if (destacador.historicoSelecionados.Count > 0)
            {
                GameObject ultimo = destacador.historicoSelecionados[destacador.historicoSelecionados.Count - 1];
                destacador.SelecionarObjeto(ultimo);
            }
            else
            {
                destacador.objetoSelecionado = null;
            }
        }
        else
        {
            destacador.objetoSelecionado = null;
        }
    }

    }
