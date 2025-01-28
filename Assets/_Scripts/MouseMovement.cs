using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement
{
    public float mouseSentivity = 500f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = 50f;
    public float bottomClamp = -30f;

    public MouseMovement()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    public float GetRotationX()
    {
        //Getting the mouse inputs
        float mouseY = Input.GetAxis("Mouse Y") * mouseSentivity * Time.deltaTime;
        //Rotation around the x axis (Look up and down)
        xRotation -= mouseY;
        //Clamp the rotation
        xRotation = Mathf.Clamp(xRotation, bottomClamp, topClamp);
        return xRotation;
    }

    public float GetRotationY()
    {
        //Getting the mouse inputs
        float mouseX = Input.GetAxis("Mouse X") * mouseSentivity * Time.deltaTime;
        //Rotation around the Y axis (Look left and right)
        yRotation += mouseX;
        return yRotation;
    }
}
