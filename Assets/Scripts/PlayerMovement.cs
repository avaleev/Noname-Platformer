using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator    animator;
    private Rigidbody2D rigidbody;
    private Vector3     refVelocity;

    [Header("Movement")]
    [Space]

    [SerializeField] private float speed = 50f;
    [SerializeField] private float jumpForce = 900f;
    [Range(0, 1)] [SerializeField] private float crouchDeceleration = 0f;
    [Range(0, 0.3f)] [SerializeField] private float movementSmoothing = 0.05f;

    private float horizontalMovement = 0f;

    [Header("Movement Control")]
    [Space]

    [SerializeField] private LayerMask groundLayerSelect;
    [SerializeField] private Transform groundPoint;
                     const float groundCheckRadius = 0.2f;
    [SerializeField] private Transform ceilingPoint;
                     const float ceilingCheckRadius = 0.2f;
    [SerializeField] private Collider2D onCrouchDisableCollider;

    private bool isGrounded;
    private bool isCrouching;
    private bool isFacingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator  = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal") * speed;
        isGrounded = Physics2D.OverlapCircle(groundPoint.position, groundCheckRadius, groundLayerSelect);

        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = true;
            horizontalMovement *= crouchDeceleration;
            onCrouchDisableCollider.enabled = false;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
            onCrouchDisableCollider.enabled = true;
        }

        Vector3 targetVelocity = new Vector2(horizontalMovement * 10f, rigidbody.velocity.y);
        rigidbody.velocity = Vector3.SmoothDamp(rigidbody.velocity, targetVelocity, ref refVelocity, movementSmoothing);
        animator.SetFloat("Speed", Mathf.Abs(horizontalMovement * 10f));

        if (isGrounded && !isCrouching && Input.GetButtonDown("Jump"))
        {
            rigidbody.AddForce(new Vector2(0f, jumpForce));
        }

        Vector2 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        animator.SetFloat("VerticalMovement", localVelocity.y);
        animator.SetBool("isGrounded", isGrounded);

        if (horizontalMovement > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalMovement < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
