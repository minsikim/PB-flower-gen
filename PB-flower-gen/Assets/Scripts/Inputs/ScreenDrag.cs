using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenDrag : MonoBehaviour
{
    [SerializeField] private float dragScale;

    private Vector2 previousPosition;
    private bool dragging;

    private void Update()
    {
        if (InputManager.InputOn)
        {
            dragging = !InputManager.HitUI;
            previousPosition = InputManager.InitialPosition;
        }

        if (InputManager.InputOff) dragging = false;

        DragScreen();
    }

    private void DragScreen()
    {
        if (dragging == false) return;

        Vector2 deltaPosition = previousPosition - InputManager.CurrentPosition;
        deltaPosition = new Vector2(deltaPosition.x * dragScale, deltaPosition.y * dragScale);

        Camera.main.transform.position += new Vector3(deltaPosition.x, 0f, deltaPosition.y);

        previousPosition = InputManager.CurrentPosition;
    }
}
