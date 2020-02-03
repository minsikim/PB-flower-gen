using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom_Perspective : MonoBehaviour
{
    [SerializeField] private float zoomScale;

    [Header("Minimum Zoom")]
    [SerializeField] private float maximumCameraHeight;

    [Header("Maximum Zoom")]
    [SerializeField] private float minimumCameraHeight;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ZoomIn();
        ZoomOut();
    }

    private void ZoomIn_PC()
    {
        if (InputManager.MouseWheelValue <= 0) return;

        if (transform.position.y < minimumCameraHeight) return;

        transform.position += transform.forward * zoomScale;
    }

    private void ZoomOut_PC()
    {
        if (InputManager.MouseWheelValue >= 0) return;

        //if(transform.position.y>minimumZoom)

        transform.position += -transform.forward * zoomScale;
    }

    private void ZoomIn_MobileDevice()
    {
        /*if (Input.touchCount == 2)
        {
            Vector2 touch0 = Input.GetTouch(0).position;
            Vector2 touch1 = Input.GetTouch(1).position;

            Vector2 lastTouch0 = touch0 - Input.GetTouch(0).deltaPosition;
            Vector2 lastTouch1 = touch1 - Input.GetTouch(1).deltaPosition;

            float pinch = (lastTouch0 - lastTouch1).magnitude - (touch0 - touch1).magnitude;

            ZoomIn(currentZoom, pinch);
            ZoomOut(currentZoom, pinch);
        }*/

        /*if (Input.touchCount == 2)
        {
            Vector2 touch0 = Input.GetTouch(0).position;
            Vector2 touch1 = Input.GetTouch(1).position;

            Vector2 lastTouch0 = touch0 - Input.GetTouch(0).deltaPosition;
            Vector2 lastTouch1 = touch1 - Input.GetTouch(1).deltaPosition;

            float pinch = (lastTouch0 - lastTouch1).magnitude - (touch0 - touch1).magnitude;

            ZoomIn(currentZoom, pinch);
            ZoomOut(currentZoom, pinch);
        }*/
    }

    private void ZoomOut_MobileDevice()
    {
    }

    private void ZoomIn()
    {
#if UNITY_EDITOR
        ZoomIn_PC();
#elif UNITY_ANDROID
        ZoomIn_MobileDevice();
#endif
    }

    private void ZoomOut()
    {
#if UNITY_EDITOR
        ZoomOut_PC();
#elif UNITY_ANDROID
        ZoomOut_MobileDevice();
#endif
    }
}
