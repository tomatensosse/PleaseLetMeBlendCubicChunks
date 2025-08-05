using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Title("Player References")]
    private Player player;
    private Rigidbody rb;

    [Title("Movement Base", "Base movement variables used globally")]
    public Transform orientation;
    public float movementSpeed = 5.0f;
    private float horizontalMovement;
    private float verticalMovement;

    [Title("Ground Detection & Jumping")]
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public float jumpForce = 3.0f;
    private bool isGrounded;

    [Title("Drag", "Drag is used to clamp player movement speed.")]
    public float groundDrag = 6.0f;
    public float airDrag = 4.0f;
    public float decelerateDrag = 30.0f;
    private float dragMultiplier = 0.2f;

    public void Initialize(Player player, Rigidbody rb)
    {
        this.player = player;
        this.rb = rb;
    }

    public void UpdateMovement()
    {
        MyInput();

        MovePlayer();
    }

    public void FixedUpdateMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        HandleDrag();
    }

    private void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void HandleDrag()
    {
        var v = rb.velocity;
        v = -v * v.magnitude;

        if (!IsMovingControlled() && isGrounded)
        {
            v.y = 0;
            rb.AddForce(v * decelerateDrag * dragMultiplier, ForceMode.Force);
        }
        else if (isGrounded)
        {
            v.y = 0;
            rb.AddForce(v * groundDrag * dragMultiplier, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(v * airDrag * dragMultiplier, ForceMode.Force);
        }
    }

    private void MovePlayer()
    {
        rb.AddForce(MoveDirection().normalized * movementSpeed, ForceMode.Acceleration);
    }

    private void Jump()
    {
        if (!isGrounded)
        {
            return;
        }

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        StartCoroutine(ResetJump());
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => isGrounded);
    }

    private bool IsMovingControlled()
    {
        return horizontalMovement != 0 || verticalMovement != 0;
    }

    private Vector3 MoveDirection()
    {
        return orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }
}