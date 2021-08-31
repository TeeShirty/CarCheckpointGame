using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car4 : MonoBehaviour
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

    public int acceleration;

    private Rigidbody _rigidbody;

    //Skidding variables
    float slipSidewayFriction;
    float slipForwardFriction;

    WheelFrictionCurve t1;
    WheelFrictionCurve t2;
    WheelFrictionCurve t3;
    WheelFrictionCurve t4;

    //max speed
    public int maxSpeed = 100;

    //the MAGIC VALUE
    float magicValue = 0.05f;//controls the time it takes for car to recover from drift:low->longer, high->faster

    private void Start()
    {

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.localPosition;

        slipForwardFriction = 0.05f;
        slipSidewayFriction = 0.018f;

        t1 = wheelColliderRightBack.forwardFriction;
        t1.stiffness = 0.01f;
        t2 = wheelColliderLeftBack.forwardFriction;
        t2.stiffness = 0;
        t3 = wheelColliderRightBack.sidewaysFriction;
        t3.stiffness = 0;
        t4 = wheelColliderLeftBack.sidewaysFriction;
        t4.stiffness = 0;
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

        Debug.Log("MotorTorque " + wheelColliderLeftBack.motorTorque + " and BrakeTorque " + wheelColliderLeftFront.brakeTorque);
        Debug.Log("MotorTorque " + wheelColliderRightBack.motorTorque + " and BrakeTorque " + wheelColliderRightFront.brakeTorque);


        Handbrake();

        Debug.Log("wheelColliderRightBack.forwardFriction " + t1.stiffness);
        Debug.Log("wheelColliderLeftBack.forwardFriction " + t2.stiffness);
        Debug.Log("wheelColliderRightBack.sidewaysFriction " + t3.stiffness);
        Debug.Log("wheelColliderLeftBack.sidewaysFriction " + t4.stiffness);
    }

    void Handbrake()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            braked = true;
        }
        
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            braked = false;
        }

        if (braked)
        {
            Debug.Log("Brake Released");

            Invoke("CalcStiffness", 1.0f);

            wheelColliderLeftFront.brakeTorque = MaxBrakeTorque;
            wheelColliderRightFront.brakeTorque = MaxBrakeTorque;
            wheelColliderLeftBack.motorTorque = 0f;
            wheelColliderRightBack.motorTorque = 0f;
        }
        else if (!braked)
        {
            Debug.Log("Brake Released");

            CancelInvoke("CalcStiffness");
            t1.stiffness = 0;
            t2.stiffness = 0;
            t3.stiffness = 0;
            t4.stiffness = 0;
            wheelColliderLeftFront.brakeTorque = 0f;
            wheelColliderRightFront.brakeTorque = 0f;


            //slipForwardFriction = 1f;
            //slipSidewayFriction = 0.018f;
        }
    }

    void Accelerate()
    {
        wheelColliderLeftBack.motorTorque = Input.GetAxis("Vertical") * motorTorque;
        wheelColliderRightBack.motorTorque = Input.GetAxis("Vertical") * motorTorque;
    }

    void Steer()
    {
        wheelColliderLeftFront.steerAngle = Input.GetAxis("Horizontal") * maxSteer;
        wheelColliderRightFront.steerAngle = Input.GetAxis("Horizontal") * maxSteer;
    }

    void MakeSlip(float forwardFriction, float sidewayFriction)
    {
        //WheelFrictionCurve t1 = wheelColliderRightBack.forwardFriction;
        t1.stiffness = forwardFriction;

        //WheelFrictionCurve t2 = wheelColliderLeftBack.forwardFriction;
        t2.stiffness = forwardFriction;

        //WheelFrictionCurve t3 = wheelColliderRightBack.sidewaysFriction;
        t3.stiffness = sidewayFriction;

        //WheelFrictionCurve t4 = wheelColliderLeftBack.sidewaysFriction;
        t4.stiffness = sidewayFriction;

        wheelColliderRightBack.forwardFriction = t1;
        wheelColliderLeftBack.forwardFriction = t2;
        wheelColliderRightBack.sidewaysFriction = t3;
        wheelColliderLeftBack.sidewaysFriction = t4;


        Debug.Log("wheelColliderRightBack.forwardFriction " + t1.stiffness);
        Debug.Log("wheelColliderLeftBack.forwardFriction " + t2.stiffness);
        Debug.Log("wheelColliderRightBack.sidewaysFriction " + t3.stiffness);
        Debug.Log("wheelColliderLeftBack.sidewaysFriction " + t4.stiffness);

    }

    //simple function for organization that determines the slip value for drfiting
    void CalcStiffness()
    {
        int carSpeed = (int)GetComponent<Rigidbody>().velocity.magnitude;
        
        if (carSpeed <= 70f && carSpeed >= 15f)
        {
            slipSidewayFriction = 1f;
            MakeSlip(slipForwardFriction, slipSidewayFriction);
        }
        else if (carSpeed > 70f && carSpeed <= maxSpeed)
        {
            slipSidewayFriction = 2f;
            MakeSlip(slipForwardFriction, slipSidewayFriction);
        }
        else if (carSpeed < 15f)
        {
            slipSidewayFriction = .5f;
            MakeSlip(slipForwardFriction, slipSidewayFriction);
            //MakeSlip(Mathf.Lerp(wheelColliderLeftBack.forwardFriction.stiffness, 1f, Time.deltaTime * magicValue), Mathf.Lerp(wheelColliderLeftBack.sidewaysFriction.stiffness, 1f, Time.deltaTime * magicValue));
        }
        Debug.Log(carSpeed);
        //Debug.Log("SIDEWAYS FRIC: "+ wheelColliderLeftBack.sidewaysFriction.stiffness+" FORWARD FRIC: "+ wheelColliderLeftBack.forwardFriction.stiffness);
    }
}
