using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    public Transform target;
    public float smoothTime;
    public float maxSpeed;
    Vector3 currentVelocity;

    private Vector3 shakeOffset;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 2f;
    public float dampingSpeed = 1f;

    private Coroutine shakeRoutine;

    private void LateUpdate()
    {

        /*
        if (target != null)
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), ref currentVelocity, smoothTime, maxSpeed);
        */
        


        Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos + shakeOffset, ref currentVelocity, smoothTime, maxSpeed);
    }

    public void TriggerShake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;

            float magnitude = shakeMagnitude * Mathf.Exp(-dampingSpeed * elapsedTime);

            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0f
            );

            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }
}
