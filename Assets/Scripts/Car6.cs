using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car6 : MonoBehaviour
{

    private const string _HORIZONTAL = "Horizontal";
    private const string _VERTICAL = "Vertical";


    private float _horizontalInput;
    private float _verticalInput;
    private float _currentSteerAngle;
    private float _currentBrakeForce = 0;
    private bool _isBraking = false;

    //use getter and setter 
    public bool isBraking
    {
        get { return _isBraking; }
        set
        {
            if (_isBraking == value)
                return;

            _isBraking = value;
            _currentBrakeForce = _isBraking ? _brakeForce : 0f;
            ApplyBrakes();
        }
    }

    [SerializeField] private Transform _centerOfMass;

    [SerializeField] private float _motorForce;
    [SerializeField] private float _brakeForce;
    [SerializeField] private float _maxSteeringAngle;

    [SerializeField] private WheelCollider _wheelColliderFrontLeft;
    [SerializeField] private WheelCollider _wheelColliderFrontRight;
    [SerializeField] private WheelCollider _wheelColliderBackLeft;
    [SerializeField] private WheelCollider _wheelColliderBackRight;

    [SerializeField] private Transform _wheelFrontLeft;
    [SerializeField] private Transform _wheelFrontRight;
    [SerializeField] private Transform _wheelBackLeft;
    [SerializeField] private Transform _wheelBackRight;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
    }


    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();

        Debug.Log("_wheelColliderBackLeft.motorTorque " + _wheelColliderBackLeft.motorTorque);
        Debug.Log("_wheelColliderBackRight.brakeTorque " + _wheelColliderBackRight.brakeTorque);
        Debug.Log(_isBraking);
    }

  
    private void GetInput()
    {
        _horizontalInput = Input.GetAxis(_HORIZONTAL);
        _verticalInput = Input.GetAxis(_VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
    }


    private void HandleMotor()
    {
        _wheelColliderBackLeft.motorTorque = _verticalInput * _motorForce;
        _wheelColliderBackRight.motorTorque = _verticalInput * _motorForce;
        
        //if (_isBraking)
        //{
        //    ApplyBrakes();
        //}
    }


    private void ApplyBrakes()
    {
        _wheelColliderFrontLeft.brakeTorque = _currentBrakeForce;
        _wheelColliderFrontRight.brakeTorque = _currentBrakeForce;
        _wheelColliderBackLeft.brakeTorque = _currentBrakeForce;
        _wheelColliderBackRight.brakeTorque = _currentBrakeForce;
    }


    private void HandleSteering()
    {
        _currentSteerAngle = _maxSteeringAngle * _horizontalInput;
        _wheelColliderFrontLeft.steerAngle = _currentSteerAngle;
        _wheelColliderFrontRight.steerAngle = _currentSteerAngle;
    }



    private void UpdateWheels()
    {
        var position = Vector3.zero;
        var rotation = Quaternion.identity;

        _wheelColliderFrontLeft.GetWorldPose(out position, out rotation);
        _wheelFrontLeft.position = position;
        _wheelFrontLeft.rotation = rotation;

        _wheelColliderFrontRight.GetWorldPose(out position, out rotation);
        _wheelFrontRight.position = position;
        _wheelFrontRight.rotation = rotation * Quaternion.Euler(0, 180, 0);

        _wheelColliderBackLeft.GetWorldPose(out position, out rotation);
        _wheelBackLeft.position = position;
        _wheelBackLeft.rotation = rotation;

        _wheelColliderBackRight.GetWorldPose(out position, out rotation);
        _wheelBackRight.position = position;
        _wheelBackRight.rotation = rotation * Quaternion.Euler(0, 180, 0);
    }
}
