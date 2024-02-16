using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float jumpForce = 1f;
    public float SprintSpeed = 10f;
    public bool CanMove;
    private bool CanJump;
    private bool CanDash;
    private bool CanSprint;
    private Rigidbody2D rb;
    public Vector3 initialposition;
    private Animator animator;
    private bool isDashing;
    private float horizontalInput;
    private float jumpPressTime = 1f;
    public float maxChargeTime = 1f;
    public float chargeMultiplier = 1f;
    public float maxJumpForce = 1f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    private float animationDuration = 1f;

    public float life = 1;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialposition = transform.position;
        CanMove = true;
        isDashing = false;
        CanDash = false;
        CanSprint = true;

    }
    public void Update()
    {
        PlayerMovement();
        HandleJump();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && CanMove == false && CanDash)
        {
            Dash();

            float horizontalInput = Input.GetAxis("Horizontal");
            float move = isDashing ? dashSpeed : MoveSpeed;
            rb.velocity = new Vector2(horizontalInput * move, rb.velocity.y);

            Debug.Log("DASH!");
        }

        if (Input.GetKey(KeyCode.R))
        {
            Respawn();
        }
    }


    public void PlayerMovement()
    {
        if (CanMove)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && CanSprint)
            {
                MoveSpeed = 10f;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && CanSprint)
            {
                MoveSpeed = 5f;
            }
            if (Input.GetKey(KeyCode.A) && !isDashing)
            {
                transform.Translate(Vector2.right * MoveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                animator.SetBool("PlayerWalk", true);
            }
            if (Input.GetKey(KeyCode.D) && !isDashing)
            {
                transform.Translate(Vector2.right * MoveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                animator.SetBool("PlayerWalk", true);
            }
            else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                animator.SetBool("PlayerWalk", false);
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint2D contact = collision.contacts[0];
            if (Vector2.Dot(contact.normal, Vector2.up) > 0.5)
            {
                CanJump = true;
                CanMove = true;
                animator.SetBool("PlayerJump", false);

                if (collision.gameObject.CompareTag("Enemy"))
                {
                    Debug.Log("HIT");
                    TakeDamage();
                }

                if (collision.gameObject.CompareTag("Ground"))
                {
                    animator.SetBool("PlayerDash", false);
                    CanDash = false;
                    CanSprint = true;
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }
    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CanJump)
        {
            animator.SetBool("PlayerWalk", false);
            StartChargingJump();
            animator.SetBool("PlayerCrouch", true);
        }

        if (Input.GetKey(KeyCode.Space) && CanJump)
        {
            ContinueChargingJump();
        }

        if (Input.GetKeyUp(KeyCode.Space) && CanJump)
        {
            ReleaseJump();
            animator.SetBool("PlayerCrouch", false);
            animator.SetBool("PlayerJump", true);
        }
    }

    void StartChargingJump()
    {
        jumpPressTime = 0f;
        CanMove = false;
    }

    void ContinueChargingJump()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        jumpPressTime += Time.deltaTime;

        float chargePercentage = Mathf.Clamp01(jumpPressTime / maxChargeTime);
    }

    void ReleaseJump()
    {
        float adjustedJumpForce = jumpForce + (jumpForce * jumpPressTime * chargeMultiplier);

        adjustedJumpForce = Mathf.Clamp(adjustedJumpForce, 0f, maxJumpForce);

        Vector2 jumpForceVector = new Vector2(horizontalInput * MoveSpeed, adjustedJumpForce);

        rb.AddForce(jumpForceVector, ForceMode2D.Impulse);

        CanMove = false;
        CanJump = false;
        CanDash = true;
    }

    void Dash()
    {
        float dashDirection = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
        isDashing = true;
        animator.SetBool("PlayerDash", true);
        StartCoroutine(DashDUR());
    }

    IEnumerator DashDUR()
    {
        yield return new WaitForSeconds(dashDuration);
        StopDashing();
    }

    void StopDashing()
    {
        // Reset the velocity and dashing flag
        rb.velocity = new Vector2(0f, rb.velocity.y);
        isDashing = false;
        animator.SetBool("PlayerDash", false);
    }

    void Respawn()
    {
        animator.SetBool("PlayerDead", false);
        transform.position = initialposition;
        CanMove = true;
    }

    void TakeDamage()
    {
        life--;

        if (life <= 0)
        {
            CanMove = false;
            Debug.Log("YOU ARE SO BAD!");
            animator.SetBool("PlayerWalk", false);
            animator.SetBool("PlayerDash", false);
            CanMove = false;
            rb.velocity = Vector2.zero;
            animator.SetBool("PlayerDead", true);
            StartCoroutine(Respawning());
        }
    }
    IEnumerator Respawning()
    {
        yield return new WaitForSeconds(animationDuration);
        Respawn();
    }
}
