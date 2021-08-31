using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    public bool steer;
    public bool invertSteer;
    public bool power;

    public float SteerAngle { get; set; }
    public float Torque { get; set; }

    private WheelCollider _wheelCollider;
    private Transform _wheelTransform;

    // Start is called before the first frame update
    void Start()
    {
        _wheelCollider = GetComponentInChildren<WheelCollider>();
        _wheelTransform = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        _wheelTransform.position = position;
        _wheelTransform.rotation = rotation;
    }

    void FixedUpdate()
    {
        if(steer)
        {
            _wheelCollider.steerAngle = SteerAngle * (invertSteer ? -1 : 1);
        }

        if(power)
        {
            _wheelCollider.motorTorque = Torque;
        }
    }
}
