using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour
{

    public float sensitivityX = 2f;
    public float sensitivityY = 2f;

    public float minX = -360;
    public float maxX = 360;
    public float minY = -60;
    public float maxY = 60;

    private Quaternion originalRot;
    public float rotX = 0;
    public float rotY = 0;

    private void Start()
    {
        originalRot = transform.localRotation;
    }

    private void Update()
    {
        rotX += Input.GetAxis("Mouse X") * sensitivityX;
        rotY += Input.GetAxis("Mouse Y") * sensitivityY;

        rotX = rotX % 360;
        rotY = rotY % 360;

        rotX = Mathf.Clamp(rotX, minX, maxX);
        rotY = Mathf.Clamp(rotY, minY, maxY);

        Quaternion xQuaternion = Quaternion.AngleAxis(rotX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotY, Vector3.left);

        transform.localRotation = originalRot * xQuaternion * yQuaternion;
    }
}