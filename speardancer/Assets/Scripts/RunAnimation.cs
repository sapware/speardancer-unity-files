
using UnityEngine;

public class RunAnimation : MonoBehaviour
{
    [Header("Character Sprite")]
    public SpriteRenderer characterSprite;

    [Header("Running Animation - 8 Frames")]
    public Sprite[] runningFrames = new Sprite[8];
    [Min(1f)] public float runningFPS = 10f;
    [Min(0.01f)] public float runningScale = 1f;
    public float runningHeight = 0f;

    [Header("Idle Animation - 6 Frames")]
    public Sprite[] idleFrames = new Sprite[6];
    [Min(1f)] public float idleFPS = 6f;
    [Min(0.01f)] public float idleScale = 1f;
    public float idleHeight = 0f;

    [Header("Jump Animation - 6 Frames")]
    public Sprite[] jumpFrames = new Sprite[6];
    [Min(1f)] public float jumpFPS = 10f;
    [Min(0.01f)] public float jumpScale = 1f;
    public float jumpHeight = 0f;

    [Header("Landing Impact")]
    public Sprite impactFrame;
    [Min(0.01f)] public float impactDuration = 0.15f;
    [Min(0.01f)] public float impactScale = 1f;
    public float impactHeight = 0f;

    [Header("Jump Settings")]
    public Rigidbody2D playerRigidbody;

    [Header("Ground Detection")]
    public GroundCheck groundChecker;

    [Header("Debug")]
    public bool logFrames = true;

    private Transform spriteTransform;
    private Vector3 originalPosition;
    private Vector3 originalScale;

    private int currentFrame;
    private float timer;
    private float impactTimer;
    private int facingDirection = 1;

    private bool wasGrounded;
    private bool jumpStarted;

    private enum AnimationState
    {
        Idle,
        Running,
        Jumping,
        Impact
    }

    private AnimationState currentState = AnimationState.Idle;

    void Awake()
    {
        if (characterSprite == null)
            characterSprite = GetComponentInChildren<SpriteRenderer>();

        if (characterSprite == null)
        {
            Debug.LogError(
                "RunAnimation: No SpriteRenderer found!", this
            );

            enabled = false;
            return;
        }

        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody2D>();

        if (playerRigidbody == null)
            playerRigidbody = GetComponentInParent<Rigidbody2D>();

        if (groundChecker == null)
            groundChecker = GetComponent<GroundCheck>();

        spriteTransform = characterSprite.transform;
        originalPosition = spriteTransform.localPosition;
        originalScale = spriteTransform.localScale;

        wasGrounded = IsGrounded();

        ChangeAnimation(AnimationState.Idle);
    }

    void Update()
    {
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        if (left && !right)
            facingDirection = -1;
        else if (right && !left)
            facingDirection = 1;

        bool grounded = IsGrounded();

        // Detect the start of a jump.
        if (currentState != AnimationState.Jumping &&
            currentState != AnimationState.Impact &&
            playerRigidbody != null &&
            playerRigidbody.linearVelocity.y > 0.1f)
        {
            jumpStarted = true;
            ChangeAnimation(AnimationState.Jumping);
            wasGrounded = grounded;
            return;
        }

        // Handle the airborne animation and landing.
        if (currentState == AnimationState.Jumping)
        {
            if (grounded && jumpStarted &&
                (playerRigidbody == null ||
                 playerRigidbody.linearVelocity.y <= 0.05f))
            {
                jumpStarted = false;
                StartImpactAnimation();
            }
            else
            {
                AdvanceJumpAnimation();
            }

            wasGrounded = grounded;
            return;
        }

        // Play the impact frame for a short duration.
        if (currentState == AnimationState.Impact)
        {
            impactTimer += Time.deltaTime;

            if (impactTimer >= impactDuration)
            {
                ReturnToGroundAnimation();
            }

            wasGrounded = grounded;
            return;
        }

        // Catch a landing if the character becomes grounded
        // before the jump animation state was entered.
        if (!wasGrounded && grounded && jumpStarted)
        {
            jumpStarted = false;
            StartImpactAnimation();
            wasGrounded = grounded;
            return;
        }

        // Select idle or running while on the ground.
        AnimationState desiredState = left != right
            ? AnimationState.Running
            : AnimationState.Idle;

        if (desiredState != currentState)
            ChangeAnimation(desiredState);

        if (currentState == AnimationState.Running)
            AdvanceLoopingAnimation(runningFrames, runningFPS);
        else
            AdvanceLoopingAnimation(idleFrames, idleFPS);

        wasGrounded = grounded;
    }

    bool IsGrounded()
    {
        return groundChecker != null &&
               groundChecker.isGrounded;
    }

    void StartImpactAnimation()
    {
        impactTimer = 0f;
        ChangeAnimation(AnimationState.Impact);

        // If no impact sprite is assigned, skip straight to
        // the normal grounded animation.
        if (impactFrame == null)
            ReturnToGroundAnimation();
    }

    void ChangeAnimation(AnimationState newState)
    {
        currentState = newState;
        currentFrame = 0;
        timer = 0f;

        ShowCurrentFrame();
        ApplyClipSettings();
    }

    void AdvanceLoopingAnimation(Sprite[] frames, float fps)
    {
        if (frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(fps, 1f);

        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame = (currentFrame + 1) % frames.Length;
            ShowCurrentFrame();
        }
    }

    void AdvanceJumpAnimation()
    {
        if (jumpFrames == null || jumpFrames.Length == 0)
            return;

        // Hold the last jump frame until landing.
        if (currentFrame >= jumpFrames.Length - 1)
            return;

        timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(jumpFPS, 1f);

        while (timer >= frameDuration &&
               currentFrame < jumpFrames.Length - 1)
        {
            timer -= frameDuration;
            currentFrame++;
            ShowCurrentFrame();
        }
    }

    void ReturnToGroundAnimation()
    {
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        AnimationState nextState = left != right
            ? AnimationState.Running
            : AnimationState.Idle;

        ChangeAnimation(nextState);
    }

    void ShowCurrentFrame()
    {
        if (characterSprite == null)
            return;

        if (currentState == AnimationState.Impact)
        {
            if (impactFrame != null)
            {
                characterSprite.sprite = impactFrame;
                characterSprite.enabled = true;
            }

            return;
        }

        Sprite[] frames = GetActiveFrames();

        if (frames == null || frames.Length == 0)
            return;

        currentFrame = Mathf.Clamp(
            currentFrame, 0, frames.Length - 1
        );

        Sprite sprite = frames[currentFrame];

        if (sprite == null)
        {
            if (logFrames)
            {
                Debug.LogWarning(
                    "RunAnimation: Missing sprite in " +
                    currentState + " frame " + currentFrame,
                    this
                );
            }

            return;
        }

        characterSprite.sprite = sprite;
        characterSprite.enabled = true;
    }

    Sprite[] GetActiveFrames()
    {
        switch (currentState)
        {
            case AnimationState.Running:
                return runningFrames;

            case AnimationState.Jumping:
                return jumpFrames;

            default:
                return idleFrames;
        }
    }

    void ApplyClipSettings()
    {
        if (spriteTransform == null)
            return;

        float scale;
        float height;

        switch (currentState)
        {
            case AnimationState.Running:
                scale = runningScale;
                height = runningHeight;
                break;

            case AnimationState.Jumping:
                scale = jumpScale;
                height = jumpHeight;
                break;

            case AnimationState.Impact:
                scale = impactScale;
                height = impactHeight;
                break;

            default:
                scale = idleScale;
                height = idleHeight;
                break;
        }

        spriteTransform.localScale = new Vector3(
            Mathf.Abs(originalScale.x) * scale * facingDirection,
            originalScale.y * scale,
            originalScale.z
        );

        Vector3 position = originalPosition;
        position.y += height;
        spriteTransform.localPosition = position;
    }

    void LateUpdate()
    {
        ApplyClipSettings();
    }
}
