using System.Collections;
using UnityEngine;
using TMPro;
using Febucci.UI;
using UnityEngine.UI;

public enum BattleState { Start, PreparingPlayer, PlayerTurn, EnemyTurn, Won, Lost}

public class turnbasedScript : MonoBehaviour
{
    [Header("References")]
    private Camera _camera;
    [SerializeField] private EnemiesManager _enemiesManager;
    [SerializeField] private Checkpoint _checkpoint;

    [Header("Prefabs")]
    [SerializeField] private Sprite _playerImages;
    [SerializeField] private Sprite[] _enemiesImage;

    [Header("BattlePosition")]
    [SerializeField] private Image _playerBattlePosition;
    [SerializeField] private Image _enemyBattlePosition;

    [Header("Player")]
    [SerializeField] private player _player;
    private bool _itsCritical = false;

    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _tbCanvas;
    private GameObject _attackCanvas;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _playerLife;
    [SerializeField] private TextMeshProUGUI _enemyLife;
    private GameObject _lifeUI;

    [Header("Enemy")]
    [SerializeField] private int enemyHeal = 4;
    private Enemy _enemy;
    private bool  _getRandomRoll = false;
    private int chanceOfMiss;

    private BattleState _state = BattleState.PlayerTurn;
    private Vector3 _velocity = Vector3.one;

    private TypewriterByWord _textAnimator;

    #region Getter & Setters
    public Enemy SetEnemyScript { set => _enemy = value; }
    //public GameObject SetEnemySelectedByThePlayer { set => _enemySelectedByThePlayer = value; }
    #endregion

    private void Start()
    {
        _state = BattleState.Start;
        _camera = Camera.main;
       
        _attackCanvas = this.gameObject.transform.GetChild(2).transform.GetChild(0).gameObject;
        _lifeUI = this.gameObject.transform.GetChild(2).transform.GetChild(1).gameObject;

        _textAnimator = this.gameObject.transform.GetChild(1).GetComponent<TypewriterByWord>();

        _lifeUI.SetActive(false);
        _attackCanvas.SetActive(false);
    }

    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.L))
            _textAnimator.ShowText("eu na verdade <rainb>estou testando para ver se o text animator</rainb> vai funcionar dessa vez.");
    }

    #region Setup Battle
    IEnumerator SetupBattle ()
    {
        _enemy = GetComponent<Enemy>();

        _textAnimator.ShowText("The Battle starts");

        yield return new WaitForSeconds(3f);

        _state = BattleState.PlayerTurn;

        _lifeUI.SetActive(true);

        UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
        UpdateLifeUI(_enemyLife, _enemy.GetLife, _enemy.GetTotalLife);

        _playerLife.transform.parent.gameObject.GetComponent<TextMeshProUGUI>().text = _player.name.ToUpper(); //change the text according to the player's name
        _enemyLife.transform.parent.gameObject.GetComponent<TextMeshProUGUI>().text = _enemy.name.ToUpper(); //change the text according to the enemy name

        _playerBattlePosition.sprite = _playerImages;
        _enemyBattlePosition.sprite = _enemiesImage[_enemy.GetIndex];

        PlayerTurn();

    }

    public void ActivateTurnBased()
    {
        _tbCanvas.SetActive(true);
        StartCoroutine(SetupBattle());
        transform.position = _player.gameObject.transform.position;
        _camera.orthographicSize = 3;
    }
    #endregion

    #region Player Turn
    private void PlayerTurn ()
    {
        _textAnimator.ShowText("Choose an action...");
        _attackCanvas.SetActive(true);
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

    public void OnRunButton ()
    {
        if (_state != BattleState.PlayerTurn)
            return;

        StartCoroutine(Run());
    }

    IEnumerator PlayerAttack ()
    {
        bool isDead;

        if (RollDice() <= 9)
        {
            //damage the enemy
            isDead = _enemy.TakeDamage(_player.GetAttackDamge);
            UpdateLifeUI(_enemyLife, _enemy.GetLife, _enemy.GetTotalLife);
            //_dialogueText.text = "The attack is succesful!!";
            _textAnimator.ShowText("The attack is <color=yellow><wave>succesful</color></wave>!!");
        }
        else
        {
            //_dialogueText.text = "The attack has missed the enemy!!";
            _textAnimator.ShowText("The attack has <color=red>missed</color> the enemy!!");
            isDead = false;
        }

        yield return new WaitForSeconds(4f);

        //check if the enemy is dead
        if (isDead)
        {
            _state = BattleState.Won;
            StartCoroutine(EndBattle());
        } 
        else
        {
            _state = BattleState.EnemyTurn;
            _attackCanvas.SetActive(false);
            StartCoroutine(EnemyAttack());
        }
    }

    IEnumerator HealPlayer ()
    {
        _player.HealHP(3);
        UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
        //_dialogueText.text = "After driking some refreshing juice, you feel new again!!";
        _textAnimator.ShowText("You feel <wave><color=green>healed</color></wave> after drinking some juice");

        yield return new WaitForSeconds(4f);

        _state = BattleState.EnemyTurn;
        _attackCanvas.SetActive(false);
        StartCoroutine(EnemyAttack());
    }
    #endregion

    #region Enemy Turn
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

        yield return new WaitForSeconds(4f);

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

    private void RandomEnemyAttack (int normalAttack, int strongAttack, EnemyType enemyType)
    {
        bool isDead;

        Something(enemyType);

        if (RollDice() >= 1 && RollDice() <= 7)
        {
            isDead = _player.TakeDamage(normalAttack);
            UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = _enemy.name + " attacks, causing " + normalAttack + " of damage";
        }
        else if (RollDice() >= 8 && RollDice() <= 9)
        {
            isDead = _player.TakeDamage(strongAttack);
            UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = _enemy.name + " attacks, causing " + strongAttack + " of damage";
        }
        else
        {
            isDead = false;
            UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);
            _dialogueText.text = "Enemy has missed his attack!!";
        }
    }

    private void Something(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Weak:
                break;
            case EnemyType.Medium:
                break;
            case EnemyType.Strong:
                break;
            case EnemyType.Hard:
                break;
            default:
                break;
        }
    }

    #endregion

    #region End Battle
    IEnumerator EndBattle ()
    {
        if (_state == BattleState.Won)
        {
            _dialogueText.text = "CONGRATS!! You has everything that a Hero needs...";
            _lifeUI.SetActive(false);
            _attackCanvas.SetActive(false);

            yield return new WaitForSeconds(2.5f);

            _tbCanvas.SetActive(false);
            _camera.orthographicSize = 5;

            _enemiesManager.DisableEnemy(_enemy.GetIndex);
            _player.RestoreHP();
            _player.EnablePlayerMovement();

        } 
        else
        {
            _dialogueText.text = "DEFEATED!! Best luck in your next time.";
            _lifeUI.SetActive(false);

            yield return new WaitForSeconds(2.5f);

            _player.EnablePlayerMovement();
            _checkpoint.ReturnToCheckpoint();
            _enemiesManager.EnableAllEnemies(); //later Ill have to change this to enable all the enemies that wasnt defeated

            _player.RestoreHP();
            _enemy.RestoreFullHP();

            _tbCanvas.SetActive(false); //change this later to leantween animation
            _camera.orthographicSize = 5;
        }
        yield return null;
    }
    #endregion

    IEnumerator Run ()
    {
        _dialogueText.text = "You decided to RUN...";

        yield return new WaitForSeconds(2f);
        
        if (RollDice() > 7)
        {
            _dialogueText.text = "But you failed...";

            yield return new WaitForSeconds(3f);

            _attackCanvas.SetActive(false);
            _state = BattleState.EnemyTurn;
            StartCoroutine(EnemyAttack());
        }
        else
        {
            _dialogueText.text = "After fearing your enemy, you RAN!!";

            yield return new WaitForSeconds(3f);
            
            _tbCanvas.SetActive(false);
            _camera.orthographicSize = 5;
            //the player does not heal himself when running from a battle
            _enemiesManager.DisableEnemy(_enemy.GetIndex);
            _player.EnablePlayerMovement();
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
}
