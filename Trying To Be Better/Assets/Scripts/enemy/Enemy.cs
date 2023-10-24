using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MouseHandler
{
    [Header("Enemy Variables")]
    [SerializeField] private int _life;
    [SerializeField] [Range(1, 3)] private int _enemyQuantity;
    [SerializeField] private turnbasedScript _turnbasedScript;
    private Animator _animator;
    private BoxCollider2D _collider;

    private GameObject _enemyInstantiated1;
    private GameObject _enemyInstantiated2;

    private bool playerIsAttacking = false;
    private bool _enteredOnce = false;
    
    private int _numberofEnemiesSpawned = default;
    private int _totalLife = 10;

    [Header("Damage")]
    [SerializeField] private int _normalDamage = 2;
    [SerializeField] private int _strongDamage = 5;

    #region Getter/Setter
    public int GetEnemyQuantity { get => _enemyQuantity; }
    public bool SetPlayerIsAttacking { set => playerIsAttacking = value; }
    public int GetLife { get => _life; }
    public int SetLife { set => _life = value; }
    public int GetTotalLife { get => _totalLife; }
    public int GetNormalDamage { get => _normalDamage; }
    public int GetStrongDamage { get => _strongDamage; }
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _life = _totalLife;
    }

    public void DestroySpawnedEnemy ()
    {
        Destroy(_enemyInstantiated1);
        Destroy(this.gameObject);
        _enteredOnce = false;

    }

    public bool TakeDamage(int damage)
    { 
        _life -= damage;

        if (_life <= 0)
            return true;
        else
            return false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //if (this.gameObject.layer == 8)
        //    _turnbasedScript.SetEnemySelectedByThePlayer = this.gameObject;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (this.gameObject.layer == 8)
            _animator.SetBool("PlayerIsAttacking", true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (this.gameObject.layer == 8)
            _animator.SetBool("PlayerIsAttacking", false);
    }
}
