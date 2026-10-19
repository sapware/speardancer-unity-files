using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float crouchSpeed = 3f;

    public float jumpForce = 10f;
    public float boostForce = 25f;

    private Rigidbody2D rb;
    private float moveInput;

    private bool isGrounded = true;
    private bool boostAvailable = true;
    private bool isBoosting = false;

    private bool spearEquipped = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Equip / Unequip Spear
        if (Input.GetKeyDown(KeyCode.E))
        {
            spearEquipped = !spearEquipped;

            if (spearEquipped)
            {
                Debug.Log("Spear Equipped");
            }
            else
            {
                Debug.Log("Spear Unequipped");
            }
        }

        // Normal Jump
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }

        // Spear Boost
        if (spearEquipped &&
            Input.GetKeyDown(KeyCode.Q) &&
            !isGrounded &&
            boostAvailable &&
            !isBoosting)
        {
            StartCoroutine(SpearBoost());
        }
    }

    IEnumerator SpearBoost()
    {
        isBoosting = true;
        boostAvailable = false;

        rb.linearVelocity = Vector2.zero;

        RigidbodyConstraints2D oldConstraints = rb.constraints;

        rb.constraints =
            RigidbodyConstraints2D.FreezePositionX |
            RigidbodyConstraints2D.FreezePositionY |
            RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(0.35f);

        rb.constraints = oldConstraints;

        rb.linearVelocity = new Vector2(
            0f,
            boostForce
        );

        isBoosting = false;
    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.C))
        {
            currentSpeed = crouchSpeed;
        }

        if (!isBoosting)
        {
            rb.linearVelocity = new Vector2(
                moveInput * currentSpeed,
                rb.linearVelocity.y
            );
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        boostAvailable = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}