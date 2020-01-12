using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public float Speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalSpeed = Input.GetAxis("Horizontal") * Speed;
        float verticalSpeed = Input.GetAxis("Vertical") * Speed;

        if (verticalSpeed != 0)
        {
            transform.position = transform.position + new Vector3(transform.forward.x * verticalSpeed, 0, transform.forward.z * verticalSpeed);
        }

        if (horizontalSpeed != 0)
        {
            transform.position = transform.position + new Vector3(transform.right.x * horizontalSpeed, 0, transform.right.z * horizontalSpeed);
        }
    }
}
