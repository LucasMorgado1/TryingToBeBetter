using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;


public class player : MonoBehaviour
{
    #region Enum
    public enum MovementState { Idle, Walking, Stop };
    #endregion

    #region References Variables
    private PlayerControls _playerControls;
    private Rigidbody2D rb;
    private MovementState mState;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Movement Variables

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float velPower;
    [SerializeField] private float frictionAmout;
    [HideInInspector] public float direction = default;
    public float moveDirection = default;
    private bool isFacingRight = true;
    private float saveMoveSpeed = 7;
    private float saveFrictionAmount = 0.55f;

    #endregion

    #region Jump Variables

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;  // Força inicial do pulo
    public float fallMultiplier = 2.5f;    // Multiplicador de queda para controlar a velocidade de queda
    public float lowJumpMultiplier = 2f;   // Multiplicador de pulo baixo para controlar o arco do pulo
    [SerializeField] private int maxJumps = 2;  // Número máximo de pulos
    private int currentJumps = 0;     // Pulos atuais
    private bool isJumping = false;   // Está pulando?  2
    private bool isGrounded = false;  // Está no chão?
    [SerializeField] private GroundChecker groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool jumpPressed;

    #endregion

    #region Interaction Variables

    [Header("Interactable")]
    private int npcLayerMask = 6;
    private bool interacted = false;

    #endregion

    #region Animator Variables
    private Animator animator;
    #endregion

    #region Attacks && Health
    [Header("Attacks Damage")]
    private int _attackDamage = 1;
    private int _life;
    private int _totalLife = 10;
    #endregion

    #region Getters/Setters
    public bool GetIdle => mState == MovementState.Idle;
    public bool GetWalk => mState == MovementState.Walking;
    public bool GetFacinRight => isFacingRight == true;
    public float SetMoveSpeed { set => moveSpeed = value; }
    public float OriginalMoveSpeed { get => saveMoveSpeed; private set => saveMoveSpeed = value; }
    public float FrictionAmount { get => frictionAmout; set => frictionAmout = value; }
    public float OriginalFrictionAmount { get => saveFrictionAmount; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public MovementState SetIdle() => mState = MovementState.Idle;
    public MovementState SetWalk() => mState = MovementState.Walking;
    public MovementState SetStop() => mState = MovementState.Stop;
    public int GetAttackDamge { get => _attackDamage; }
    public Rigidbody2D GetRigidbody => rb;
    public int GetLife { get => _life; }
    public int SetLife { set => _life = value; }
    public int GetTotalLife {  get => _totalLife; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        var action = new InputAction();
        _playerControls.Player.Move.performed += ctx => moveDirection = ctx.ReadValue<float>();
        _life = _totalLife;
    }

    private void Start()
    {
        SetIdle();
        groundCheck = transform.GetChild(0).GetComponent<GroundChecker>();
    }

    private void Update()
    {
        if (!isFacingRight && moveDirection > 0f)
            FlipSprite();
        else if (isFacingRight && moveDirection < 0f)
            FlipSprite();

        if (Input.GetKeyDown(KeyCode.Space) && currentJumps < maxJumps)
        {
            if (!isJumping)
            {
                Jump();
                isJumping = true;
            }
            else if (isJumping && currentJumps <= maxJumps - 1)
            {
                Jump();
            }
        }

        // Aplica o multiplicador de queda para controlar a velocidade de queda
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Aplica o multiplicador de pulo baixo para controlar o arco do pulo
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (mState != MovementState.Stop)
        {
            if (moveDirection == 0)
                SetIdle();
            else
            {
                Walk();
                SetDirection();
            }
            Friction();
        }
    }
    #endregion

    #region Input System Methods

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    public void InteractHandler(InputAction.CallbackContext ctx)
    {
        interacted = ctx.performed;
    }
    #endregion

    #region Movement

    private void Walk()
    {
        //calcula a direcao que queremos nos mover na velocidade desejada
        float targetSpeed = moveDirection * moveSpeed;

        //calcula a diferente entre a velocidade atual e a velocidade desejada
        float speedDif = targetSpeed - rb.velocity.x;

        //muda a taxa de acelera��o dependnedo da situa��o
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01) ? acceleration : decceleration;

        //aplica a acelera��o na diferen�a de velocidade, a acelera��o aumenta com velocidades maiores
        //multiplica com o Sign para reaplicar a dire��o
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        //aplica a for�a ao rb, multiplicando pelo Vector2.right, afetando apenas em X
        rb.AddForce(movement * Vector2.right);

        SetWalk();
    }

    public void SetDirection()
    {
        if (moveDirection > 0)
        {
            direction = 1;
        }
        else if (moveDirection < 0)
        {
            direction = -1;
        }
    }

    private void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
    #endregion

    #region Friction
    private void Friction()
    {
        if (Mathf.Abs(moveDirection) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmout));

            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }
    #endregion

    #region Jump
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        currentJumps++;
    }

    #endregion

    #region Collisions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão ocorreu com o objeto "Ground" e se a parte de baixo do jogador está tocando o chão
        if (collision.gameObject.CompareTag("Ground") && groundCheck.IsGrounded())
        {
            currentJumps = 0;
            isJumping = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (interacted)
        {
            if (collision.gameObject.CompareTag("NPC") && collision.gameObject.layer == npcLayerMask)
            {
                Debug.Log("Interacted with a NPC");
            }
        }
    }

    #endregion

    #region Damage
    public bool TakeDamage (int damage)
    {
        _life -= damage;

        if (_life <= 0)
            return true;
        else
            return false;
    }
    #endregion 
}
