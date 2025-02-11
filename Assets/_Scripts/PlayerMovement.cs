using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    float xRotation = 0f;
    float yRotation = 0f;

    public float speed = 12f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f * 2;
    
    public Transform feetTransform;
    public LayerMask groundLayer;
    public float groundDistance = 0.4f;

    bool isGrounded;
    bool isMoving;

    Vector3 velocity;
    Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Getting the inputs
        float yRotaiton = MouseMovementManager.Instance.GetRotationY();
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Rotate around the Y axis (Look left and right)
        transform.localRotation = Quaternion.Euler(0f, yRotaiton, 0f);

        //Creating the moving vector
        Vector3 move = transform.right * x + transform.forward * z;

        // Actuall moving the player
        controller.Move(move * speed * Time.deltaTime);

        //Ground check
        isGrounded = Physics.CheckSphere(feetTransform.position, groundDistance, groundLayer);
        //Resseting the default velocity
        if (isGrounded && velocity.y < 0)
        {
            //The velocity is directed downward with a magnitude of 2.
            velocity.y = -2f;
        }

        //Check if the player can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //The velocity is positive and directed upward
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //Jumping with the distance traveled upward or downward depends on the sign of the velocity
        controller.Move(velocity * Time.deltaTime);

        //Falling down
        velocity.y += gravity * Time.deltaTime;

        if (lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
            //for the later
        }
        else
        {
            isMoving = false;
            //for the later
        }

        lastPosition = gameObject.transform.position;
    }
}
