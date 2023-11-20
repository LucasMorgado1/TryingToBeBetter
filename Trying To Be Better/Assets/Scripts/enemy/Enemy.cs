using System;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Variables")]
    [SerializeField] private turnbasedScript _turnbasedScript;
    [SerializeField] private EnemyScriptableObject _enemyScriptableObject;

    [Header("Combat Variables")]
    [SerializeField]
    private int _hp;
    private int _maxHP;
    private int _normalDamage;
    private int _strongDamage;
    private SpriteRenderer _spriteRenderer;
    private int _healAmount;
    private EnemyType _enemyType;
    private int _enemyIndex;

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

        _maxHP = _enemyScriptableObject._hp;
        _hp = _maxHP;

        _normalDamage = _enemyScriptableObject._normalAttack;
        _strongDamage = _enemyScriptableObject._strongAttack;
        _enemyType = _enemyScriptableObject._enemyType;
        _healAmount = _enemyScriptableObject._healAmount;
        _spriteRenderer.sprite = _enemyScriptableObject._sprite;
        _healAmount = _enemyScriptableObject._healAmount;
    }

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
}
