using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovementManager: MonoBehaviour
{
    public static MouseMovementManager Instance;

    public float mouseSentivity = 500f;
    public float topClamp = 70f;
    public float bottomClamp = -50f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
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
