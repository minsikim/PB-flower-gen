using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static void TurnOn() { Instance.gameObject.SetActive(true); }
    public static void TurnOff() { Instance.gameObject.SetActive(false); }

    public static Vector2 InitialPosition { get; private set; }
    public static Vector2 CurrentPosition { get; private set; }

    public static float MouseWheelValue { get; private set; }

    public static bool InputOn { get; private set; }
    public static bool InputOff { get; private set; }    
    public static bool InputRunning { get; private set; }
    public static bool HitUI { get; private set; }

    public static bool Pinching { get; private set; }

    private void Input_PC()
    {
        InputOn = Input.GetMouseButtonDown(0);
        InputRunning = Input.GetMouseButton(0);
        InputOff = Input.GetMouseButtonUp(0);
        HitUI = EventSystem.current.IsPointerOverGameObject(-1);
        MouseWheelValue = Input.mouseScrollDelta.y;

        CurrentPosition = Input.mousePosition;

        if (InputOn) InitialPosition = Input.mousePosition;
    }

    private void Input_MobileDevice()
    {
        if (Input.touchCount > 0)
        {
            Pinching = Input.touchCount == 2;
        }
    }

    private void Awake() { Instance = this; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        Input_PC();
#elif UNITY_ANDROID
        Input_MobileDevice();
#endif
    }

    //public InputData CollectInput()
    //{
    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);

    //        _inputData.PrevPosition = _inputData.CurrentPosition;
    //        _inputData.CurrentPosition = touch.position;

    //        _inputData.WasDown = _inputData.IsDown;
    //        _inputData.IsDown = (touch.phase == TouchPhase.Moved) || (touch.phase == TouchPhase.Began) || (touch.phase == TouchPhase.Stationary);



    //        if (touch.phase == TouchPhase.Ended) _inputData.PressEndTime = Time.time;


    //        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
    //        {
    //            _inputData.StartPosition = touch.position;
    //            _inputData.PressStartTime = Time.time;
    //        }
    //    }
    //    else if (Input.touchCount == 0)
    //    {
    //        _inputData.PrevPosition = _inputData.CurrentPosition;

    //        _inputData.IsDown = false;
    //        _inputData.WasDown = false;
    //    }

    //    return _inputData;
    //}
}
