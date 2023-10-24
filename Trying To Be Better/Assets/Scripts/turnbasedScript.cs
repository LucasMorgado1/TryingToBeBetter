using System.Collections;
using UnityEngine;
using TMPro;


public enum BattleState { Start, PreparingPlayer, PlayerTurn, EnemyTurn, Won, Lost}

public class turnbasedScript : MonoBehaviour
{
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

    private Camera _camera;
    private BattleState _state = BattleState.PlayerTurn;
    private Vector3 _velocity = Vector3.one;
    private Timer _timer;

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
        _timer = new Timer();
        _timer.duration = 3.0f;
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

    IEnumerator PlayerAttack ()
    {
        if (RollDice() <= 8)
        {
            //damage the enemy
            bool isDead = _enemy.TakeDamage(_player.GetAttackDamge);
            UpdateLifeUI(_enemyLife, _enemy.GetLife, _enemy.GetTotalLife);
            _dialogueText.text = "The attack is succesful!!";
        }
        else
        {
            _dialogueText.text = "The attack has missed the enemy!!";
        }



        yield return new WaitForSeconds(2.5f);

        //check if the enemy is dead
        if (isDead)
        {
            _state = BattleState.Won;
            EndBattle();
        } 
        else
        {
            _state = BattleState.EnemyTurn;
            _attackUI.SetActive(false);
            StartCoroutine(EnemyAttack());
        }
    }

    IEnumerator EnemyAttack ()
    {
        _dialogueText.text = _enemy.name + " attacks...";

        yield return new WaitForSeconds(2.5f);

        bool isDead = _player.TakeDamage(ChooseEnemyAttack());
        UpdateLifeUI(_playerLife, _player.GetLife, _player.GetTotalLife);

        if (isDead)
        {
            _state = BattleState.Lost;
            EndBattle();
        } 
        else
        {
            _state = BattleState.PlayerTurn;
            PlayerTurn();
            //StartCoroutine(PlayerAttack());
        }

    }

    private int ChooseEnemyAttack()
    {
        int roll = Random.Range(1, 21);

        if (roll >= 1 && roll <= 12) //normal attack
        {
            return 3;
        } 
        else if (roll >= 13 && roll <= 20)
        {
            return 6;
        }

        return 0;
    }


    private void EndBattle ()
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
        }
        return;
    }

    private void UpdateLifeUI (TextMeshProUGUI lifeTxt, int currentLife, int totalLife)
    {
        lifeTxt.text = currentLife + "/" + totalLife;
    }

    private int RollDice ()
    {
        return Random.Range(1, 11);
    }



    /*#region Player Turn
    public void GetPlayerAttack (int numberAttack)
    {
        switch (numberAttack)
        {
            case 1:
                Attack(_player.GetFirstAttackDamage, "Sword Slash");
                ObserveEnemyLife();
                EndPlayerTurn();
                break;
            case 2:
                Attack(_player.GetSecondAttackDamage, "Magic Spell");
                ObserveEnemyLife();
                EndPlayerTurn();
                break;
            case 3:
                Attack(_player.GetThirdAttackDamage, "Quick Fist");
                ObserveEnemyLife();
                EndPlayerTurn();
                break;
        }
    }

    public void StartPlayerAttack (int numberAttack)
    {
        StartCoroutine(PlayerAttack(numberAttack));
    }

    private IEnumerator PlayerAttack (int numberAttack)
    {
        switch (numberAttack)
        {
            case 1:
                //Attack(_player.GetFirstAttackDamage, "Sword Slash");
                _dialogueText.text = "Player used Sword Slash and gave " + _player.GetFirstAttackDamage + " of damage.";
                ObserveEnemyLife();
                yield return new WaitForSeconds(2f);
                EndPlayerTurn();
                break;
            case 2:
                //Attack(_player.GetSecondAttackDamage, "Magic Spell");
                _dialogueText.text = "Player used Magic Spell and gave " + _player.GetSecondAttackDamage + " of damage.";
                ObserveEnemyLife();
                yield return new WaitForSeconds(2f);
                EndPlayerTurn();
                break;
            case 3:
                //Attack(_player.GetThirdAttackDamage, "Quick Fist");
                _dialogueText.text = "Player used Sword Slash and gave " + _player.GetFirstAttackDamage + " of damage.";
                ObserveEnemyLife();
                yield return new WaitForSeconds(2f);
                EndPlayerTurn();
                break;
        }
    }

    private void Attack (int damage, string attackname)
    {
        int randomNumber = Random.Range(1, 21);

        _itsCritical = randomNumber <= 5 && randomNumber >= 1 ?  true: false;

        if (_itsCritical)
        {
            _enemy.TakeDamage(damage * 2);
            //_turnbasedText.text = "Player used " + attackname + " and gave " + damage * 2 + " of damage.";
        }
        else
        {
            _enemy.TakeDamage(damage);
            _dialogueText.text = "Player used " + attackname + " and gave " + damage + " of damage.";
        }
    }

    private void EndPlayerTurn ()
    {
        _state = BattleState.EnemyTurn;
        _dialogueText.text = "Now it's Enemy turn";
    }
    #endregion

    #region Enemy Turn
    IEnumerator EnemyAttack ()
    {
        int roll = RandomRoll();

        if (_enemy.GetLife <= _enemy.GetLife / 2.5f)
        {
            if (roll >= 1 && roll <= 7)
            {
                _enemy.SetLife = _enemy.GetLife + enemyHeal;
                _dialogueText.text = "Heal in: " + enemyHeal + " of life";
                EndEnemyTurn();
                yield return null;
            }
            else
                yield return null; ;
        }
        else if (_player.GetLife <= _enemy.GetNormalDamage)
        {
            _player.TakeDamage(_enemy.GetNormalDamage);
            yield return null; ;
        }
        else
        {
            if (roll >= 1 && roll <= 16) //normal attack
            {
                _dialogueText.text = "do a Normal Attack!!";
                Debug.Log("normal attack");
                _player.TakeDamage(_enemy.GetNormalDamage);
                yield return null; ;
            }
            else if (roll >= 17 && roll <= 20) //strong attack
            {
                _dialogueText.text = "do a Strong Attack!!";
                Debug.Log("strong attack");
                _player.TakeDamage(_enemy.GetStrongDamage);
                yield return null; ;
            }
        }

        yield return new WaitForSeconds(3f);

        EndEnemyTurn();
    }

    private void ChooseEnemyAttack()
    {
        int roll = RandomRoll();

        if (_enemy.GetLife <= _enemy.GetLife / 2.5f)
        {
            if (roll >= 1 && roll <= 7)
            {
                _enemy.SetLife = _enemy.GetLife + enemyHeal;
                _dialogueText.text = "Heal in: " + enemyHeal + " of life";
                EndEnemyTurn();
                return;
            }
            else
                return;
        }
        else if (_player.GetLife <= _enemy.GetNormalDamage)
        {
            _player.TakeDamage(_enemy.GetNormalDamage);
            return;
        }
        else
        {
            if (roll >= 1  && roll <= 16) //normal attack
            {
                _dialogueText.text = "do a Normal Attack!!";
                Debug.Log("normal attack");
                _player.TakeDamage(_enemy.GetNormalDamage);
                return;
            }
            else if (roll >= 17 && roll <= 20) //strong attack
            {
                _dialogueText.text = "do a Strong Attack!!";
                Debug.Log("strong attack");
                _player.TakeDamage(_enemy.GetStrongDamage);
                return;
            }
        }
    }

    private int RandomRoll ()
    {
        if (!_getRandomRoll)
        {
            int temp = Random.Range(1, 21);
            _getRandomRoll = true;
            return temp;
        }
        return 0;
    }

    private void EndEnemyTurn ()
    {
        _state = BattleState.PlayerTurn;
        _dialogueText.text = "Now it's Player Turn";
    }
    #endregion

    private void ObserveEnemyLife ()
    {
        if (_enemy.GetLife <= 0)
        {
            _dialogueText.text = "The Enemy has been defeated.";
            DisableTurnbasedCanvas();
        }
    }

    public void RunButton()
    {
        DisableTurnbasedCanvas();
        _enemy.DestroySpawnedEnemy();
    }

    public void ActivateTurnBased()
    {
        this.gameObject.SetActive(true);
        transform.position = _player.gameObject.transform.position;
        _camera.orthographicSize = 3;
    }

    public void DisableTurnbasedCanvas()
    {
        this.gameObject.SetActive(false);
        //essas medidas eventulamente irao para um game manager
        _camera.orthographicSize = 5;
    }*/
    public void ActivateTurnBased()
    {
        StartCoroutine(SetupBattle());
        transform.position = _player.gameObject.transform.position;
        _camera.orthographicSize = 3;
    }
}
