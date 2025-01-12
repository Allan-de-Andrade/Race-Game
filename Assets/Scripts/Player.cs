using TMPro;
using UnityEngine;

/// <summary>
/// Class that controls the player's behavior.
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField] WheelCollider[] wheelColliders; 
    [SerializeField] Transform[] wheelTransforms; 
    [SerializeField] TrailRenderer[] driftMarks; 
    [SerializeField] float brakeForce, steerAngle; 
    [SerializeField] Vector2 center_of_mass; 
    [SerializeField] float[] gearForces;
    [SerializeField] float[] maxSpeeds;
    [SerializeField] TMP_Text gearText;
    [SerializeField] TMP_Text velocimenter;
    bool beBraking;
    int gearIndex = 0;
    float currentVelocity;
    Rigidbody carBody;

    private void Start()
    {
        carBody = GetComponent<Rigidbody>();
        carBody.centerOfMass = center_of_mass;
    }

    private void FixedUpdate()
    {
        Movement();
        Brake();
        RotateAllWheels();
        HandleGears();

        velocimenter.text = "Km/H:" + (int)(carBody.linearVelocity.magnitude * 3.6); // Update the speedometer with the car's speed
    }

    /// <summary>
    /// Controls the car's movement based on user input.
    /// </summary>
    public void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal"); // User's horizontal input (steering)
        float vertical = Input.GetAxis("Vertical"); // User's vertical input (acceleration)

        // Set the motor torque for each wheel based on the vertical input and the motor force of the current gear
        for (int wheel = 0; wheel < wheelColliders.Length; wheel++)
             wheelColliders[wheel].motorTorque = vertical * gearForces[gearIndex];
        

        // Set the steering angle for the front wheels based on the horizontal input
        for (int wheel = 0; wheel < wheelColliders.Length - 2; wheel++)
            wheelColliders[wheel].steerAngle = horizontal * steerAngle;
    }

    /// <summary>
    /// Controls the car's braking based on user input.
    /// </summary>
    public void Brake()
    {
        float vertical = Input.GetAxis("Vertical"); // User's vertical input (acceleration)
        beBraking = Input.GetKey(KeyCode.Space);

        // Activate brake and drift marks if the space key is pressed
        if (beBraking)
        {
            wheelColliders[0].motorTorque = 0;
            wheelColliders[1].motorTorque = 0;

            wheelColliders[2].brakeTorque = brakeForce;
            wheelColliders[3].brakeTorque = brakeForce;

            if (currentVelocity > 80)
            {
                foreach (var item in driftMarks)
                    item.emitting = true;
            }
        }

        // Deactivate brake and drift marks if the space key is released
        else if (!beBraking)
        {
            wheelColliders[2].brakeTorque = 0;
            wheelColliders[3].brakeTorque = 0;

            foreach (var item in driftMarks)
                item.emitting = false;
        }
    }
   

    public void HandleGears()
    {
        gearText.text= "Gear:" + (gearIndex + 1);
        if (Input.GetKeyDown(KeyCode.E) && gearIndex + 1 <= gearForces.Length - 1)
            gearIndex += 1;

        else if (Input.GetKeyDown(KeyCode.Q) && gearIndex - 1 >= 0)
            gearIndex -= 1;
        LimitSpeed();
    }
    public void LimitSpeed()
    {
        currentVelocity = carBody.linearVelocity.magnitude * 3.6f;

        if (currentVelocity > maxSpeeds[gearIndex])
        {
            carBody.linearVelocity = carBody.linearVelocity.normalized * (maxSpeeds[gearIndex] / 3.6f);
            print(currentVelocity);
            print(carBody.linearVelocity);
        }
    }

    public void RotateAllWheels()
    {
        RotateASingleWheel(wheelColliders[0], wheelTransforms[0]);
        RotateASingleWheel(wheelColliders[1], wheelTransforms[1]);
        RotateASingleWheel(wheelColliders[2], wheelTransforms[2]);
        RotateASingleWheel(wheelColliders[3], wheelTransforms[3]);
    }

    public void RotateASingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

   
}
