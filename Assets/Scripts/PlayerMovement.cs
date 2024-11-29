using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public Rigidbody rb;

    public float horizontal;
    public float vertical;
    public Vector3 direction;

    public float moveSpeed;
    public float turnSmoothTime;
    public float turnSmoothVelocity;

    public LayerMask groundLayer;
    public float groundCheckDistance;
    public float jumpForce;
    public bool canJump;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.velocity = Vector3.zero;

        horizontal = 0f;
        vertical = 0f;
        direction = Vector3.zero;
        moveSpeed = 5f;
        turnSmoothTime = 0.1f;

        groundCheckDistance = 0.5f;
        jumpForce = 5f;
        canJump = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && isGrounded())
        {
            Debug.DrawRay(transform.position, Vector3.down * (GetComponent<SphereCollider>().radius + groundCheckDistance), Color.red);
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && isGrounded())
        {
            PlayerMove();
        }

        else if (direction.magnitude == 0f)
        {
            Vector3 velocity = rb.velocity;
            rb.velocity = new Vector3(velocity.x * 0.9f, velocity.y, velocity.z * 0.9f);
        }
    }

    private void PlayerMove()
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        moveDirection.y = 0f;

        Vector3 moveVelocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    private void PlayerJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    private bool isGrounded()
    {
        float sphereRadius = GetComponent<SphereCollider>().radius;
        Vector3 sphereCenter = transform.position;

        Vector3[] raycastOffsets = {
            Vector3.zero,                           
            Vector3.forward * 0.5f,                
            Vector3.back * 0.5f,                     
            Vector3.left * 0.5f,                     
            Vector3.right * 0.5f                      
        };

        // Cast rays
        foreach (var offset in raycastOffsets)
        {
            Vector3 origin = sphereCenter + offset;
            if (Physics.SphereCast(sphereCenter, sphereRadius * 0.9f, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) < 5f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
