using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereControl : MonoBehaviour, IControllable
{
    Vector3 drag_position;
    public void moveTo(Vector3 destination)
    {
        drag_position = destination;
    }

    public void scaleTo(Vector3 initialScale, float scale)
    {

        transform.localScale = initialScale * scale;
    }

    public void youve_been_tapped()
    {
        transform.position += Vector3.up;
    }

    // Start is called before the first frame update
    void Start()
    {
        drag_position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, drag_position, 0.1f);
    }
}
