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

    [Header("Damage")]
    [SerializeField] private int _normalDamage = 2;
    [SerializeField] private int _strongDamage = 5;

    #region Getter/Setter
    public int GetEnemyQuantity { get => _enemyQuantity; }
    public bool SetPlayerIsAttacking { set => playerIsAttacking = value; }
    public int GetLife { get => _life; }
    public int SetLife { set => _life = value; }
    public int GetNormalDamage { get => _normalDamage; }
    public int GetStrongDamage { get => _strongDamage; }
    #endregion

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (collision.CompareTag("Player") && !_enteredOnce) //maybe change to Vector3.Distance <= 5.5
        {
            _collider.offset = Vector3.zero;
            _collider.size = Vector3.one;
            _enteredOnce = true;
            this.gameObject.layer = 8;

            switch (_enemyQuantity)
            {
                case 1:
                    break;
                case 2:
                    _enemyInstantiated1 = Instantiate(this.gameObject, new Vector3(this.transform.position.x, this.transform.position.y + 1.30f, 0), Quaternion.identity);
                    _numberofEnemiesSpawned = 1;
                    break;
                case 3:
                    _enemyInstantiated1 = Instantiate(this.gameObject, new Vector3(this.transform.position.x, this.transform.position.y + 1.30f, 0), Quaternion.identity);
                    _enemyInstantiated2 = Instantiate(this.gameObject, new Vector3(this.transform.position.x + 1.30f, this.transform.position.y + 0.8f, 0), Quaternion.identity);
                    _numberofEnemiesSpawned = 2;
                    break;
                default:
                    break;
            }
        }*/
    }

    public void DestroySpawnedEnemy ()
    {
        Destroy(_enemyInstantiated1);
        Destroy(this.gameObject);
        _enteredOnce = false;

    }

    public void TakeDamage(int damage)
    {
        Debug.Log("current life: " + _life + " and damage to be taken: " + damage);
        _life -= damage;

        if (_life <= 0)
            Destroy(this.gameObject);
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
