using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    public Transform target;
    public float smoothTime;
    public float maxSpeed;
    Vector3 currentVelocity;

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), ref currentVelocity, smoothTime,maxSpeed);
    }
}
