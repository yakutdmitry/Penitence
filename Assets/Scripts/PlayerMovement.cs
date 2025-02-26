using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed;
    public float groundDrag;
    public Transform orientation;

    public float jumpForce;
    public float jumpCooldown;
    public float AirMultiplier;
    public bool ReadyToJump;

    public Rigidbody rb;
    Vector3 Movedirection;

    float HorizontalInput;
    float VerticalInput;
    public float PlayerHeight;
    public LayerMask IsGround;
    bool grounded;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();
        //checks for "GroundTag", basically recognises when it is and isn't touching the floor
        grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, IsGround);

        if (grounded)
            rb.drag = groundDrag;
        else rb.drag = 0;
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && ReadyToJump && grounded)
        {
            ReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //Math for movement direction
        Movedirection = orientation.forward * VerticalInput + orientation.right * HorizontalInput;
        if (grounded)
            rb.AddForce(Movedirection.normalized * MovementSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(Movedirection.normalized * 10f * AirMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 FlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (FlatVel.magnitude > MovementSpeed)
        {
            Vector3 limitedVel = FlatVel.normalized * MovementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        ReadyToJump = true;
    }
}