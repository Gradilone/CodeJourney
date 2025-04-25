using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class scrMovCamera : MonoBehaviour
{
    public RectTransform targetToMove; // Painel que será arrastado
    public RectTransform limitArea;    // Área visível (painel pai)

    private Vector3 lastMousePosition;
    private bool isDragging = false;

    void Update()
    {
        // Inicia o arrasto ao clicar com o botão esquerdo sobre a UI
        if (Input.GetMouseButtonDown(0) && IsPointerOverUIElement())
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        // Termina o arrasto ao soltar o botão
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Move o painel enquanto estiver arrastando
        if (isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 difference = currentMousePosition - lastMousePosition;

            targetToMove.anchoredPosition += new Vector2(difference.x, difference.y);
            lastMousePosition = currentMousePosition;

            ClampToBounds();
        }
    }

    void ClampToBounds()
    {
        if (limitArea == null) return;

        Vector3[] areaCorners = new Vector3[4];
        Vector3[] contentCorners = new Vector3[4];

        limitArea.GetWorldCorners(areaCorners);
        targetToMove.GetWorldCorners(contentCorners);

        Vector2 offset = Vector2.zero;

        if (contentCorners[0].x > areaCorners[0].x)
            offset.x = areaCorners[0].x - contentCorners[0].x;
        else if (contentCorners[2].x < areaCorners[2].x)
            offset.x = areaCorners[2].x - contentCorners[2].x;

        if (contentCorners[0].y > areaCorners[0].y)
            offset.y = areaCorners[0].y - contentCorners[0].y;
        else if (contentCorners[2].y < areaCorners[2].y)
            offset.y = areaCorners[2].y - contentCorners[2].y;

        targetToMove.anchoredPosition += offset;
    }

    bool IsPointerOverUIElement()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
