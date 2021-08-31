using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform centerOfMass;

    public WheelCollider wheelColliderLeftFront;
    public WheelCollider wheelColliderRightFront;
    public WheelCollider wheelColliderLeftBack;
    public WheelCollider wheelColliderRightBack;

    public Transform wheelLeftFront;
    public Transform wheelRightFront;
    public Transform wheelLeftBack;
    public Transform wheelRightBack;

    public float motorTorque = 100f;
    public float maxSteer = 20f;
    public bool braked = false;
    public float MaxBrakeTorque = 100000f;

    private Rigidbody _rigidbody;

    //Skidding variables
    float slipSidewayFriction;
    float slipForwardFriction;

    //max speed
    public int maxSpeed = 100;

    //the MAGIC VALUE
    float magicValue = 0.05f;//controls the time it takes for car to recover from drift:low->longer, high->faster

    private void Start()
    {

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.localPosition;
    }
    private void FixedUpdate()
    {
        Accelerate();

        Steer();
    }

    private void Update()
    {
        var position = Vector3.zero;
        var rotation = Quaternion.identity;

        wheelColliderLeftFront.GetWorldPose(out position, out rotation);
        wheelLeftFront.position = position;
        wheelLeftFront.rotation = rotation;

        wheelColliderRightFront.GetWorldPose(out position, out rotation);
        wheelRightFront.position = position;
        wheelRightFront.rotation = rotation * Quaternion.Euler(0, 180, 0);

        wheelColliderLeftBack.GetWorldPose(out position, out rotation);
        wheelLeftBack.position = position;
        wheelLeftBack.rotation = rotation;

        wheelColliderRightBack.GetWorldPose(out position, out rotation);
        wheelRightBack.position = position;
        wheelRightBack.rotation = rotation * Quaternion.Euler(0, 180, 0);

        Debug.Log("MotorTorque " + wheelColliderLeftFront.motorTorque + " and BrakeTorque " + wheelColliderLeftFront.brakeTorque);
        Debug.Log("MotorTorque " + wheelColliderRightFront.motorTorque + " and BrakeTorque " + wheelColliderRightFront.brakeTorque);

        Handbrake();
    }

    void Handbrake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            braked = true;
        }
        else
        {
            braked = false;
        }

        if (braked)
        {
            wheelColliderLeftFront.brakeTorque = MaxBrakeTorque * 2;
            wheelColliderRightFront.brakeTorque = MaxBrakeTorque * 2;
            wheelColliderLeftFront.motorTorque = 0f;
            wheelColliderRightFront.motorTorque = 0f;
            CalcStiffness();
        }
        else if (!braked)
        {
            wheelColliderLeftFront.brakeTorque = 0f;
            wheelColliderRightFront.brakeTorque = 0f;
            Accelerate();
        }
    }

    void Accelerate()
    {
        wheelColliderLeftFront.motorTorque = Input.GetAxis("Vertical") * motorTorque * 10;
        wheelColliderRightFront.motorTorque = Input.GetAxis("Vertical") * motorTorque * 10;
    }

    void Steer()
    {
        wheelColliderLeftFront.steerAngle = Input.GetAxis("Horizontal") * maxSteer * 2;
        wheelColliderRightFront.steerAngle = Input.GetAxis("Horizontal") * maxSteer * 2;
    }

    void MakeSlip(float forwardFriction, float sidewayFriction)
    {
        WheelFrictionCurve t1 = wheelColliderRightBack.forwardFriction;
        t1.stiffness = forwardFriction;

        WheelFrictionCurve t2 = wheelColliderLeftBack.forwardFriction;
        t2.stiffness = forwardFriction;

        WheelFrictionCurve t3 = wheelColliderRightBack.sidewaysFriction;
        t3.stiffness = sidewayFriction;

        WheelFrictionCurve t4 = wheelColliderLeftBack.sidewaysFriction;
        t4.stiffness = sidewayFriction;

        wheelColliderRightBack.forwardFriction = t1;
        wheelColliderLeftBack.forwardFriction = t2;
        wheelColliderRightBack.sidewaysFriction = t3;
        wheelColliderLeftBack.sidewaysFriction = t4;
    }

    //simple function for organization that determines the slip value for drfiting
    void CalcStiffness()
    {
        int carSpeed = (int)GetComponent<Rigidbody>().velocity.magnitude;
        if (carSpeed <= 70f && carSpeed >= 15f)
        {
            slipSidewayFriction = 0.019f;
            MakeSlip(slipForwardFriction, slipSidewayFriction);
        }
        else if (carSpeed > 70f && carSpeed <= maxSpeed)
        {
            slipSidewayFriction = 0.022f;
            MakeSlip(slipForwardFriction, slipSidewayFriction);
        }
        else if (carSpeed < 15f)
        {
            MakeSlip(Mathf.Lerp(wheelColliderLeftBack.forwardFriction.stiffness, 1f, Time.deltaTime * magicValue), Mathf.Lerp(wheelColliderLeftBack.sidewaysFriction.stiffness, 1f, Time.deltaTime * magicValue));
        }
        //print ("SIDEWAYS FRIC: "+wheelRL.sidewaysFriction.stiffness+" FORWARD FRIC: "+wheelRL.forwardFriction.stiffness);
    }
}
