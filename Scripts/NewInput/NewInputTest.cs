using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class NewInputTest : MonoBehaviour
{


    public InputTest inputTest;

    private void Awake()
    {
        //inputTest = new InputTest();

        
        //Mouse.current.leftButton.isPressed


    }



    private void OnEnable()
    {
        inputTest.Enable();
    }

    private void OnDisable()
    {
        inputTest.Disable();
    }



    // Start is called before the first frame update
    void Start()
    {

        inputTest.MouseTest.Rotation.performed += Rotation_performed;


    }





    private void Rotation_performed(UnityEngine.Experimental.Input.InputAction.CallbackContext obj)
    {
        Debug.LogError("rotation:" + obj.ReadValue<Vector2>());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
