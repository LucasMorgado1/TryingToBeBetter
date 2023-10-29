using System.Collections;
using UnityEngine;
using TMPro;


public enum BattleState { Start, PreparingPlayer, PlayerTurn, EnemyTurn, Won, Lost}

public class turnbasedScript : MonoBehaviour
{
    [Header("References")]
    private Camera _camera;
    [SerializeField] private EnemiesManager _enemiesManager;
    [SerializeField] private Checkpoint _checkpoint;

    [Header("Prefabs")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _enemyPrefab;

    [Header("BattlePosition")]
    [SerializeField] private Transform _playerBattlePosition;
    [SerializeField] private Transform _enemyBattlePosition;

    [Header("Player")]
    [SerializeField] private player _player;
    private bool _itsCritical = false;

    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _tbCanvas;
    private TextMeshProUGUI tmpro; //excluir dps
    private GameObject _attackUI;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _playerLife;
    [SerializeField] private TextMeshProUGUI _enemyLife;
    private GameObject _lifeUI;

    [Header("Enemy")]
    [SerializeField] private int enemyHeal = 4;
    private Enemy _enemy;
    private bool  _getRandomRoll = false;

    private BattleState _state = BattleState.PlayerTurn;
    private Vector3 _velocity = Vector3.one;

    #region Getter & Setters
    public Enemy SetEnemyScript { set => _enemy = value; }
    //public GameObject SetEnemySelectedByThePlayer { set => _enemySelectedByThePlayer = value; }
    #endregion

    private void Awake()
    {
        tmpro = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _state = BattleState.Start;
        _camera = Camera.main;
        _attackUI = this.gameObject.transform.GetChild(2).transform.GetChild(0).gameObject;
        _lifeUI = this.gameObject.transform.GetChild(2).transform.GetChild(1).gameObject;
        _lifeUI.SetActive(false);
    }

    IEnumerator SetupBattle ()
    {
        GameObject playerGO = Instantiate(_playerPrefab, _playerBattlePosition);

        GameObject enemyGO =  Instantiate(_enemyPrefab, _enemyBattlePosition);
        _enemy = GetComponent<Enemy>();

        _dialogueText.text = "The Battle starts";

        //set hud here

        yield return new WaitForSeconds(2f);

        _state = BattleState.PlayerTurn;
        _lifeUI.SetActive(true);
        UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
        UpdateLifeUI(_enemyLife, _enemy.GetLife, _enemy.GetTotalLife);
        PlayerTurn();

    }

    private void PlayerTurn ()
    {
        _dialogueText.text = "Choose an action...";
        _attackUI.SetActive(true);
    }

    public void OnAttackButton ()
    {
        if (_state != BattleState.PlayerTurn)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton ()
    {
        if (_state != BattleState.PlayerTurn)
            return;

        StartCoroutine(HealPlayer());
    }

    IEnumerator PlayerAttack ()
    {
        bool isDead;

        if (RollDice() <= 8)
        {
            //damage the enemy
            isDead = _enemy.TakeDamage(_player.GetAttackDamge);
            UpdateLifeUI(_enemyLife, _enemy.GetLife, _enemy.GetTotalLife);
            _dialogueText.text = "The attack is succesful!!";
        }
        else
        {
            _dialogueText.text = "The attack has missed the enemy!!";
            isDead = false;
        }

        yield return new WaitForSeconds(2.5f);

        //check if the enemy is dead
        if (isDead)
        {
            _state = BattleState.Won;
            StartCoroutine(EndBattle());
        } 
        else
        {
            _state = BattleState.EnemyTurn;
            _attackUI.SetActive(false);
            StartCoroutine(EnemyAttack());
        }
    }

    IEnumerator HealPlayer ()
    {
        _player.HealHP(3);
        UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
        _dialogueText.text = "After driking some refreshing juice, you feel new again!!";

        yield return new WaitForSeconds(2f);

        _state = BattleState.EnemyTurn;
        _attackUI.SetActive(false);
        StartCoroutine(EnemyAttack());
    }

    IEnumerator EnemyAttack ()
    {
        bool isDead;
        _dialogueText.text = _enemy.name + " attacks...";

        yield return new WaitForSeconds(2.5f);


        if (RollDice() >= 1 && RollDice() <= 7)
        {
            isDead = _player.TakeDamage(_enemy.GetNormalDamage);
            UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = _enemy.name + " attacks, causing " + _enemy.GetNormalDamage + " of damage";
        }
        else if (RollDice() >= 8 && RollDice() <= 9)
        {
            isDead = _player.TakeDamage(_enemy.GetStrongDamage);
            UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = _enemy.name + " attacks, causing " + _enemy.GetStrongDamage + " of damage";
        }
        else
        {
            isDead = false;
            UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = "Enemy missed his attack!!";
        }

        yield return new WaitForSeconds(2.5f);

        if (isDead)
        {
            _state = BattleState.Lost;
            StartCoroutine(EndBattle());
        } 
        else
        {
            _state = BattleState.PlayerTurn;
            PlayerTurn();
            //StartCoroutine(PlayerAttack());
        }

    }

    IEnumerator EndBattle ()
    {
        if (_state == BattleState.Won)
        {
            _dialogueText.text = "Congrats, you won the battle!!";
            _lifeUI.SetActive(false);
        } 
        else
        {
            _dialogueText.text = "You were defeated..."; //this scares me
            _lifeUI.SetActive(false);

            yield return new WaitForSeconds(2.5f);

            _player.EnablePlayerMovement();
            _checkpoint.ReturnToCheckpoint();
            _enemiesManager.EnableAllEnemies();
            _camera.orthographicSize = 5;
        }
        yield return null;
    }

    private void UpdateLifeUI (TextMeshProUGUI lifeTxt, int currentLife, int totalLife)
    {
        lifeTxt.text = currentLife + "/" + totalLife;
    }

    private int RollDice ()
    {
        return Random.Range(1, 11);
    }

    public void ActivateTurnBased()
    {
        StartCoroutine(SetupBattle());
        transform.position = _player.gameObject.transform.position;
        _camera.orthographicSize = 3;
    }
}
