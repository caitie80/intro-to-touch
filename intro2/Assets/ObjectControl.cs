using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControl : MonoBehaviour, IControllable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
              
    }

    public void youve_been_tapped()
    {
        transform.position += Vector3.down;
    }

    public void moveTo(Vector3 destination)
    {
        transform.position = destination;
    }
    public void scaleTo(Vector3 initialScale, float scale)
    {
        transform.localScale = initialScale * scale;
    }
}
