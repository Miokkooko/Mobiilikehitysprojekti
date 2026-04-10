using System.Collections;
using UnityEngine;
public class PopUpDamage : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float duration = 0.5f;

    void Update()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        renderer.sortingLayerName = "UI";

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(DestroyAfterDelay(duration));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolManager.Instance.DisableOther(OtherPoolType.DmgPopUp, gameObject);
    }
}
