using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform target = null;
    [SerializeField] float smoothTime = 0.3f;
    [SerializeField] float zOffset = 15f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 goalPos = target.position;
        goalPos.y = transform.position.y;
        goalPos.z -= zOffset;
        transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
    }
}