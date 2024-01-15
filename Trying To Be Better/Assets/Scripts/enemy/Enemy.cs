using System;
using System.Collections;
using System.Linq;
using UnityEngine;



public class Enemy : MonoBehaviour
{
    enum Side { left, right, idle}
    enum Action { attacking, walking, idle }
    enum Name { Slime, Gnome, Cyclope, Golem, None }

    [Header("Enemy Variables")]
    [SerializeField] private turnbasedScript _turnbasedScript;
    [SerializeField] private EnemyScriptableObject _enemyScriptableObject;
    private Rigidbody2D _rb;
    private Animator _animator;

    [Header("Combat Variables")]
    [SerializeField] private int _hp;
    private int _maxHP;
    private int _normalDamage;
    private int _strongDamage;
    private SpriteRenderer _spriteRenderer;
    private int _healAmount;
    private EnemyType _enemyType;
    private int _enemyIndex;

    [Header("State")]
    private Side _side = Side.right;
    private Action _action = Action.walking;
    [SerializeField] private Name _name;

    [Header("Slime Variables")]
    private float _slimeVelocity = 2f;
    private float _originalSlimeVelocity;


    [Header("Enemies Variables")]
    private float _velocity = 10f;
    private float _distance = 2f;
    private Vector2 _leftPoint;
    private Vector2 _rightPoint;
    private Vector2 _startPosition;
    private bool _isFacingRight;
    private Side _lastWalkingSide;

    #region Getter/Setter
    public int GetLife { get => _hp; }
    public int SetLife { set => _hp = value; }
    public int GetTotalLife { get => _maxHP; }
    public int GetNormalDamage { get => _normalDamage; }
    public int GetStrongDamage { get => _strongDamage; }
    public int GetIndex { get => _enemyIndex; }
    public int GetHealAmount => _healAmount;
    public Sprite GetSprite { get => _spriteRenderer.sprite; }
    public EnemyType GetEnemyType { get => _enemyType; }
    #endregion

    private void Awake()
    {
        _enemyIndex = GetComponent<Index>().GetIndex;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();  
        _animator = GetComponent<Animator>();

        _maxHP = _enemyScriptableObject._hp;
        _hp = _maxHP;

        _normalDamage = _enemyScriptableObject._normalAttack;
        _strongDamage = _enemyScriptableObject._strongAttack;
        _enemyType = _enemyScriptableObject._enemyType;
        _healAmount = _enemyScriptableObject._healAmount;
        _spriteRenderer.sprite = _enemyScriptableObject._sprite;
        _healAmount = _enemyScriptableObject._healAmount;
    }

    private void Start()
    {
        _startPosition = this.transform.position;
        _originalSlimeVelocity = _slimeVelocity;
        _leftPoint = new Vector2(_startPosition.x - _distance, _startPosition.y);
        _rightPoint = new Vector2(_startPosition.x + _distance, _startPosition.y);
        _side = Side.right;
        _action = Action.walking;

        Debug.Log(_name + " _leftPoint.x: " + _leftPoint);
        Debug.Log(_name + " _righPoint.x: " + _rightPoint);
    }

    private void Update()
    {
        switch (_name)
        {
            case Name.Slime:
                if (_side == Side.right || _side == Side.left && _action == Action.walking)
                {
                    Walking();
                }
                break;
            case Name.Gnome:
                break;
            case Name.Cyclope:
                break;
            case Name.Golem:
                break;
            case Name.None:
                break;
            default:
                break;
        }

        if (!_isFacingRight && _side == Side.left)
            FlipSprite();
        else if (_isFacingRight && _side == Side.right)
            FlipSprite();
    }

    private void Walk(float velocity)
    {
        _rb.velocity = new Vector3(velocity, 0, 0);
    }
    
    private void Walking ()
    {
        _slimeVelocity = _originalSlimeVelocity;

        if (_side == Side.right && _action == Action.walking)
        {
            if (_rightPoint.x > transform.position.x)
            {
                _isFacingRight = true;
                _slimeVelocity = Mathf.Abs(_slimeVelocity);
                Walk(_slimeVelocity);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                if (Vector2.Distance(_rightPoint, transform.position) < 0.9f)
                {
                    StartCoroutine(Idle());
                    return;
                }
            }
        }

        if (_side == Side.left && _action == Action.walking)
        {
            if (_leftPoint.x < transform.position.x)
            {
                _lastWalkingSide = Side.right;
                _isFacingRight = false;
                _slimeVelocity = Mathf.Abs(_slimeVelocity) * -1;
                Walk(_slimeVelocity);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);

                if (Vector2.Distance(_leftPoint, transform.position) < 0.9f)
                {
                    StartCoroutine(Idle());
                    return;
                }
            }
        }
    }

    IEnumerator  Idle ()
    {
        _side = Side.idle;
        _action = Action.idle;

        if (_side == Side.idle && _action == Action.idle)
        {
            _slimeVelocity = 0f;
            Walk(_slimeVelocity);
        }

        yield return new WaitForSeconds(2f);

        _side = (_isFacingRight == true) ? Side.left : Side.right;
        _action = Action.walking;
        Walking();
    }

    private void FlipSprite()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    #region Enemy Take Damage
    public bool TakeDamage(int damage)
    { 
        _hp -= damage;

        if (_hp <= 0)
            return true;
        else
            return false;
    }

    public void HealEnemy (int healAmount)
    {
        _hp += healAmount;
    }

    public void RestoreFullHP ()
    {
        _hp = _maxHP;
    }
    #endregion
}
