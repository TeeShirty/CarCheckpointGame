using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private Transform _target;
    [SerializeField] private float _translateSpeed;
    [SerializeField] private float _rotationSpeed;

    private void FixedUpdate()
    {
        HandleTranslation();
        HandleRotation();
    }


    private void HandleTranslation()
    {
        var targetPosition = _target.TransformPoint(_cameraOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, _translateSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        var direction = _target.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
    }
}
