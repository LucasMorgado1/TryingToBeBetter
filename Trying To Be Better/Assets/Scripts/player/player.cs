using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;


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

    [Header("Original Movement Values")]
    
    private float originalMoveSpeed = 7;
    private float originalAcceleration = 12;
    private float originalDecceleration = 16;
    private float originalVelPower = 0.85f;
    private float originalFrictionAmout = 0.55f;

    #endregion

    #region Jump Variables

    [Header("Jump")]
    
    [SerializeField] private float jumpForce;  // Força inicial do pulo
    [SerializeField] private int maxJumps = 2;  // Número máximo de pulos
    [SerializeField] private float originalJumpForce = 5f;
    [SerializeField] private GroundChecker groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public float fallMultiplier = 2.5f;    // Multiplicador de queda para controlar a velocidade de queda
    public float lowJumpMultiplier = 2f;   // Multiplicador de pulo baixo para controlar o arco do pulo
    private float playerYpositionBeforeJump;
   
    private int currentJumps = 0;     // Pulos atuais

    [HideInInspector]
    public bool isJumping = false;   // Está pulando?  2
    private bool isGrounded = false;  // Está no chão?
    private bool jumpPressed;
    
    private Rigidbody2D _rb;

    #endregion

    #region Interaction Variables

    [Header("Interactable")]
    private int interactableLayerMask = 6;
    private bool _interacted = false;

    #endregion

    #region Dash Variables
    [Header("Dash Variables")]
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashForce;
    private float dashTime = 0.2f;
    private bool _pressedDash = default;
    private bool _canDash = true;
    #endregion

    #region Animator Variables
    private Animator animator;
    #endregion

    #region Combat Variables

    [Header("Combat Variables")]
    [SerializeField] private int _maxHP = 10;
    private int _attackDamage = 1;
    private int _currentHp;

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
    public int GetLife { get => _currentHp; }
    public int SetLife { set => _currentHp = value; }
    public int GetTotalLife {  get => _maxHP; }
    public GroundChecker OnGround { get => groundCheck; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        var action = new InputAction();
        _rb = GetComponent<Rigidbody2D>();

        _playerControls.Player.Move.performed += ctx => moveDirection = ctx.ReadValue<float>();
        _currentHp = _maxHP;
        jumpForce = originalJumpForce;
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
            playerYpositionBeforeJump = this.transform.position.y;

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

        //_jumpDebugText.text = _jumpState.ToString();

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
        
        if (_pressedDash)
        {
            Debug.Log("deu dash");
            if (_canDash)
            {
                _canDash = false;
                //PerformDash();
                StartCoroutine(Dash());
            }
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
        _interacted = ctx.performed;
    }

    public void DashHandler (InputAction.CallbackContext ctx)
    {
        _pressedDash = ctx.performed;
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

    public void DisablePlayerMovement()
    {
        SetMoveSpeed = 0;
        FrictionAmount = 5;
        jumpForce = 0;
        _canDash = false;
    }

    public void EnablePlayerMovement()
    {
        SetMoveSpeed = originalMoveSpeed;
        frictionAmout = originalFrictionAmout;
        jumpForce = originalJumpForce;
        _canDash = true;
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

    #region Dash

    IEnumerator Dash ()
    {
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        
        float dashSpeed = (dashForce + moveDirection) * direction;
        Vector2 dashDirection = new Vector2(this.transform.position.x + dashSpeed, this.transform.position.y);
        
        rb.AddForce(dashDirection);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;

        yield return new WaitForSeconds(dashCooldown);

        _canDash = true;

    }

    #endregion

    #region Collisions

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se a colisão ocorreu com o objeto "Ground" e se a parte de baixo do jogador está tocando o chão
        if (collision.gameObject.CompareTag(StringUtils.Tags.Ground) && groundCheck.IsGrounded())
        {
            currentJumps = 0;
            isJumping = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_interacted)
        {
            if (collision.gameObject.CompareTag(StringUtils.Tags.NPC) && collision.gameObject.layer == interactableLayerMask)
            {
                Debug.Log("Interacted with a NPC");
            }

            if (collision.gameObject.CompareTag(StringUtils.Tags.Teleport) && collision.gameObject.layer == interactableLayerMask)
            {
                ChangeScene.Instance.LoadScene(StringUtils.SceneName.CombatWorld);
            }
        }
    }

    #endregion

    #region Damage/Heal
    public bool TakeDamage (int damage)
    {
        _currentHp -= damage;

        if (_currentHp <= 0)
            return true;
        else
            return false;
    }

    public void HealHP (int amount)
    {
        _currentHp += amount;

        if (_currentHp > _maxHP)
        {
            _currentHp = _maxHP;
        }
    }

    public void RestoreHP ()
    {
        _currentHp = _maxHP;
    }

    #endregion

}
