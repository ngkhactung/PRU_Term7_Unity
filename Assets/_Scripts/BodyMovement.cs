using UnityEngine;

public class BodyMovement : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //Getting the inputs
        float xRotation = GlobalReferences.Instance.mouseMovement.GetRotationX();

        //Rotate around the X axis (Look up and down)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
