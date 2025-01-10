using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] WheelCollider wheelFR, wheelFL, wheelRR, wheelRL;
    [SerializeField] float brakeForce, motorForce, steerForce;
    [SerializeField] Vector2 center_of_mass;
    Rigidbody carBody;
    float nitroBoost;
    InputAction nitroAction;
    public void Movement(InputAction.CallbackContext value)
    {
        Vector2 inputValue = value.ReadValue<Vector2>();

        wheelRR.motorTorque = inputValue.y * motorForce;
        wheelRL.motorTorque = inputValue.y * motorForce;

        wheelFR.steerAngle = inputValue.x * steerForce;
        wheelFL.steerAngle = inputValue.x * steerForce;

        if(Input.GetKey(KeyCode.J) || inputValue == Vector2.zero)
        {
            wheelRR.brakeTorque = brakeForce;
            wheelRL.brakeTorque = brakeForce;
        }

        else if(Input.GetKeyUp(KeyCode.J) || inputValue != Vector2.zero)
        {
            wheelRR.brakeTorque = 0;
            wheelRL.brakeTorque = 0;
        }
    }

    public void Start()
    {
        carBody = GetComponent<Rigidbody>();
        carBody.centerOfMass = center_of_mass;
        nitroBoost = motorForce * 2;
        nitroAction = InputSystem.actions.FindAction("Nitro");
    }

    public void Update()
    {
        print(carBody.linearVelocity);
        if (nitroAction.IsPressed())
            motorForce = nitroBoost;
        else
            motorForce = nitroBoost / 2;
    }
}
