using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float crouchSpeed = 3f;

    public float jumpForce = 10f;
    public float boostForce = 12f;

    public GameObject spear;
    public GameObject droppedSpear;

    private Rigidbody2D rb;
    private float moveInput;

    private bool isGrounded = true;
    private bool boostAvailable = true;
    private bool isBoosting = false;

    private bool spearEquipped = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spear != null)
            spear.SetActive(false);

        if (droppedSpear != null)
            droppedSpear.SetActive(false);
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Equip / Unequip Spear
        if (Input.GetKeyDown(KeyCode.E))
        {
            spearEquipped = !spearEquipped;

            if (spear != null)
                spear.SetActive(spearEquipped);
        }

        // Jump
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

        // Remove spear from hand
        spearEquipped = false;

        if (spear != null)
            spear.SetActive(false);

        // Spawn planted spear
        if (droppedSpear != null)
        {
            droppedSpear.transform.position =
                new Vector3(
                    transform.position.x,
                    transform.position.y - 1.5f,
                    0f
                );

            droppedSpear.SetActive(true);
        }

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.35f);

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
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