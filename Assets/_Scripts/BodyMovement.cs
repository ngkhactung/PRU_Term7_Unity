using UnityEngine;

public class BodyMovement : MonoBehaviour
{
    private MouseMovement mouseMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mouseMovement = new MouseMovement();
    }

    // Update is called once per frame
    void Update()
    {
        //Getting the inputs
        float xRotation = mouseMovement.GetRotationX();

        //Rotate around the X axis (Look up and down)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
