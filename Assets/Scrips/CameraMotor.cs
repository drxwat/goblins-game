using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public float Speed = 1f;

    Coroutine lastRoutine = null;

    // Update is called once per frame
    void Update()
    {
        float horizontalSpeed = Input.GetAxis("Horizontal") * Speed;
        float verticalSpeed = Input.GetAxis("Vertical") * Speed;

        if (verticalSpeed != 0)
        {
            InterruptCameraMove();
            transform.position = transform.position + new Vector3(transform.forward.x * verticalSpeed, 0, transform.forward.z * verticalSpeed);
        }

        if (horizontalSpeed != 0)
        {
            InterruptCameraMove();
            transform.position = transform.position + new Vector3(transform.right.x * horizontalSpeed, 0, transform.right.z * horizontalSpeed);
        }
    }

    public void MoveTo(Vector3 point)
    {
        Vector3 newCameraPosition = new Vector3(point.x, transform.position.y, point.z - 5);
        InterruptCameraMove();
        lastRoutine = StartCoroutine(MoveCamera(newCameraPosition));
    }

    IEnumerator MoveCamera(Vector3 point)
    {
        while(transform.position != point)
        {
            transform.position = Vector3.MoveTowards(transform.position, point, 50 * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    void InterruptCameraMove()
    {
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
            lastRoutine = null;
        }
    }
}
