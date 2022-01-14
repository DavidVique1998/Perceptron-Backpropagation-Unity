using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBrain : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform t_brain;
    public float angleOfRotation;
    void Start()
    {
        t_brain = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        t_brain.transform.Rotate(angleOfRotation * Vector3.up, Space.World);
    }
}
