using UnityEngine;

public class FloatingText : MonoBehaviour
{


    Transform unit;
    Transform worldSpaceCanvas;

    public Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = transform.parent;

        transform.SetParent(worldSpaceCanvas);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = unit.position + offset;
    }
}
