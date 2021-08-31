using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car5 : MonoBehaviour
{

	private WheelCollider[] wheels;

	public float maxAngle = 30;
	public float maxTorque = 300;
	public GameObject wheelShape;

	private Rigidbody _rigidbody;
	public Transform centerOfMass;

	// here we find all the WheelColliders down in the hierarchy
	public void Start()
	{
		wheels = GetComponentsInChildren<WheelCollider>();

		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.centerOfMass = centerOfMass.localPosition;

		for (int i = 0; i < wheels.Length; ++i)
		{
			var wheel = wheels[i];

			// create wheel shapes only when needed
			if (wheelShape != null)
			{
				var ws = GameObject.Instantiate(wheelShape);
				ws.transform.parent = wheel.transform;

				//if (wheel.transform.localPosition.z < 0f)
				//{
				//	ws.transform.localScale = new Vector3(ws.transform.localScale.z * -1f, ws.transform.localScale.y, ws.transform.localScale.x * -1f);
					
				//}
			}
		}
	}

	// this is a really simple approach to updating wheels
	// here we simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero
	// this helps us to figure our which wheels are front ones and which are rear
	public void Update()
	{
		float angle = maxAngle * Input.GetAxis("Horizontal");
		float torque = maxTorque * Input.GetAxis("Vertical");

		foreach (WheelCollider wheel in wheels)
		{
			// a simple car where front wheels steer while rear ones drive
			if (wheel.transform.localPosition.z > 0)
				wheel.steerAngle = angle;

			if (wheel.transform.localPosition.z < 0)
				wheel.motorTorque = torque;

			// update visual wheels if any
			if (wheelShape)
			{
				Quaternion q;
				Vector3 p;
				wheel.GetWorldPose(out p, out q);

				// assume that the only child of the wheelcollider is the wheel shape
				Transform shapeTransform = wheel.transform.GetChild(0);
				shapeTransform.position = p;
				shapeTransform.rotation = q;
			}

		}
	}
}
