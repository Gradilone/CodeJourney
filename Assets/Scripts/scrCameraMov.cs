using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrCameraMov : MonoBehaviour
{
    public Camera mainCamera;
    public Transform background; // GameObject que define os limites (com SpriteRenderer)

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private Bounds backgroundBounds;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (background != null)
        {
            SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                backgroundBounds = sr.bounds;
            }
            else
            {
                Debug.LogWarning("O GameObject de fundo precisa ter um SpriteRenderer.");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 difference = mainCamera.ScreenToWorldPoint(dragOrigin) - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mainCamera.transform.position += difference;
            dragOrigin = Input.mousePosition;

            ClampCameraToBackground();
        }
    }

    void ClampCameraToBackground()
    {
        if (background == null) return;

        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * mainCamera.aspect;

        float minX = backgroundBounds.min.x + horzExtent;
        float maxX = backgroundBounds.max.x - horzExtent;
        float minY = backgroundBounds.min.y + vertExtent;
        float maxY = backgroundBounds.max.y - vertExtent;

        Vector3 clampedPosition = mainCamera.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        mainCamera.transform.position = clampedPosition;
    }
}
