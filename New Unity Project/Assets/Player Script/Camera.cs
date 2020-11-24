using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(1, 10)]
    public float speed = 0.15f;

    private void FixedUpdate()
    {
        Follow();
    }

    void Follow()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, speed);
        transform.position = desiredPosition;
        transform.LookAt(target);
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, speed * Time.fixedDeltaTime);
        transform.position = desiredPosition;
    }

}
