using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrRemoveObjeto : MonoBehaviour
{
    public scrDestacarObjeto destacador;                // Refer�ncia ao script que guarda o objeto selecionado

    public void RemoverObjetoSelecionado()
    {
        if (destacador == null || destacador.objetoSelecionado == null)
        {
            Debug.LogWarning("Nenhum objeto selecionado para remover.");
            return;
        }

        GameObject objetoRemovido = destacador.objetoSelecionado;

        // Remove o objeto da cena
        Destroy(objetoRemovido);

        // Verifica se ainda h� hist�rico de objetos selecionados
        if (destacador.historicoSelecionados != null && destacador.historicoSelecionados.Count > 0)
        {
            // Remove o �ltimo item (que foi destru�do)
            destacador.historicoSelecionados.Remove(objetoRemovido);

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
