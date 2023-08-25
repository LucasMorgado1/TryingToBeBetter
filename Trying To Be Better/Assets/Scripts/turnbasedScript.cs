using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Net.Http.Headers;
using Unity.VisualScripting.Dependencies.Sqlite;

public enum Combat { playerTurn, enemyTurn}

public class turnbasedScript : MonoBehaviour
{
    [Header("Player")]
    private playerTurnBased _playerTurnBased;
    [SerializeField] private player _player;
    private bool _itsCritical = false;

    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI _turnbasedText;
    private TextMeshProUGUI tmpro;
    [SerializeField] private GameObject _turnBasedCanvas;

    [Header("Enemy")]
    [SerializeField] private int enemyHeal = 4;
    private Enemy _enemyScript;
    private bool  _getRandomRoll = false;

    private Camera _camera;
    private Combat _combat = Combat.playerTurn;
    private Vector3 _velocity = Vector3.one;

    #region Getter & Setters
    public Enemy SetEnemyScript { set => _enemyScript = value; }
    //public GameObject SetEnemySelectedByThePlayer { set => _enemySelectedByThePlayer = value; }
    #endregion

    private void Awake()
    {
        tmpro = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _playerTurnBased = _player.GetComponent<playerTurnBased>();
        _camera = Camera.main;
    }

    private void Update()
    {
        switch (_combat)
        {
            case Combat.playerTurn:
                break;
            case Combat.enemyTurn:
                StartCoroutine(EnemyAttack());
                _turnBasedCanvas.SetActive(false);
                break;
            default:
                break;
        }

        Debug.Log("It's " + _combat + " turn.");
    }

    #region Player Turn
    public void GetPlayerAttack (int numberAttack)
    {
        int _attackDamage;
        switch (numberAttack)
        {
            case 1:
                _attackDamage = CriticalHit(_player.GetFirstAttackDamage);
                StartCoroutine(PlayerAttack(_attackDamage));
                _turnbasedText.text = "Player is using Sword Slash";
                break;
            case 2:
                _attackDamage = CriticalHit(_player.GetSecondAttackDamage);
                StartCoroutine(PlayerAttack(_attackDamage));
                _turnbasedText.text = "Player is using Magic Spell";
                break;
            case 3:
                _attackDamage = CriticalHit(_player.GetThirdAttackDamage);
                StartCoroutine(PlayerAttack(_attackDamage));
                _turnbasedText.text = "Player is using Quick Fist";
                break;
            default:
                break;
        }
    }

    private int CriticalHit (int damage)
    {
        int randomNumber = Random.Range(1, 21);

        if (randomNumber <= 5 && randomNumber >= 1)
        {
            _itsCritical = true;
            return damage * 2;
        }
        else
        {
            _itsCritical = false;
            return damage;
        }
    }

    IEnumerator PlayerAttack (int damage)
    {
        yield return new WaitForSeconds(2.5f);

        if (_itsCritical)
            _turnbasedText.text = "IT'S A CRITICAL!! Player gave " + damage + " of damage!!!";
        else
            _turnbasedText.text = "And gave " + damage + " of damage!";

        _enemyScript.TakeDamage(damage);

        ObserveEnemyLife();

        yield return new WaitForSeconds(3f);

        _turnbasedText.text = "Now it's Enemy's turn!";

        yield return new WaitForSeconds(3f);

        EndPlayerTurn();
    }

    private void EndPlayerTurn ()
    {
        _combat = Combat.enemyTurn;
    }
    #endregion

    #region Enemy Turn
    IEnumerator EnemyAttack ()
    {
        _turnbasedText.text = "The " + _enemyScript.gameObject.name + " will...";

        yield return new WaitForSeconds(2f);

        ChooseEnemyAttack();
        
        yield return new WaitForSeconds(3f);

        //_turnbasedText.text = "Now it's Player's turn!";
        EndEnemyTurn();
    }

    private void ChooseEnemyAttack()
    {
        int roll = RandomRoll(); 

        if (_enemyScript.GetLife <= _enemyScript.GetLife / 2.5f)
        {
            if (roll >= 1 && roll <= 7)
            {
                _enemyScript.SetLife = _enemyScript.GetLife + enemyHeal;
                _turnbasedText.text = "Heal in: " + enemyHeal + " of life";
                EndEnemyTurn();
                return;
            }
            else
                return;
        }
        else //colocar uma condicao da vida do player, se tiver baixa o inimigo vai atacar
        {
            if (roll >= 1  && roll <= 16) //normal attack
            {
                _turnbasedText.text = "do a Normal Attack!!";
                Debug.Log("normal attack");
                _player.TakeDamage(_enemyScript.GetNormalDamage);
                return;
            }
            else if (roll >= 17 && roll <= 20) //strong attack
            {
                _turnbasedText.text = "do a Strong Attack!!";
                Debug.Log("strong attack");
                _player.TakeDamage(_enemyScript.GetStrongDamage);
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
        _combat = Combat.playerTurn;
    }
    #endregion

    private void ObserveEnemyLife ()
    {
        if (_enemyScript.GetLife <= 0)
            DisableTurnbasedCanvas();
    }

    public void RunButton()
    {
        DisableTurnbasedCanvas();
        _enemyScript.DestroySpawnedEnemy();
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
        _playerTurnBased.OnExitBattle();
        _camera.orthographicSize = 5;
    }
}
