using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControllable
{
    void youve_been_tapped();
    void moveTo(Vector3 destination);

    void scaleTo(Vector3 initialScale, float scale);
    GameObject gameObject { get; }
}
