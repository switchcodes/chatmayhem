using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleLimit : MonoBehaviour
{
    public float minAngle = 0;

    public float maxAngle = 90;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = transform.rotation;
        rotation.z = Mathf.Clamp(rotation.z, minAngle, maxAngle);
        transform.rotation = rotation;
    }
}