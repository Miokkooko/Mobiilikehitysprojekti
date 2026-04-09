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

    void Start()
    {
        Destroy(gameObject, duration);
    }
}
