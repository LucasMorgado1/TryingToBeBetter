using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Net.Http.Headers;

public enum Combat { playerTurn, enemyTurn, secondEnemyTurn, thirdEnemyTurn}

public class turnbasedScript : MonoBehaviour
{
    [Header("Player")]
    private playerTurnBased _playerTurnBased;
    [SerializeField] private player _player;

    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI _turnbasedText;
    private TextMeshProUGUI tmpro;


    private Camera _camera;
    private Combat _combat = Combat.playerTurn;
    private Enemy _enemyScript;
    private Vector3 _velocity = Vector3.one;

    private GameObject _enemySelectedByThePlayer;

    #region Getter & Setters
    public Enemy SetEnemyScript { set => _enemyScript = value; }
    public GameObject SetEnemySelectedByThePlayer { set => _enemySelectedByThePlayer = value; }
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
                this.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case Combat.secondEnemyTurn:
                break;
            case Combat.thirdEnemyTurn:
                break;
            default:
                break;
        }
    }

    private void SelectEnemyShowText()
    {
        _turnbasedText.text = "Please select the target enemy!!";
    }

    private bool EnemySelected ()
    {
        if (_enemySelectedByThePlayer != null)
        {
            return true;
        }
        else
            return false;
    }

    public void ChoosedAttack (int i)
    {
        switch (i)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    public void FirstPlayerAttack()
    {
        if (EnemySelected())
        {
            if (RandomCriticalDamage() >= 1 && RandomCriticalDamage() <= 5)
            {
                Debug.Log("Critical");
                _enemySelectedByThePlayer.GetComponent<Enemy>().TakeDamage(_player.GetFirstAttackDamage * 2);
                _turnbasedText.text = "Player used Sword Slash, causing a CRITICAL DAMAGE of " + (_player.GetFirstAttackDamage * 2);
            }
            else
            {
                _enemySelectedByThePlayer.GetComponent<Enemy>().TakeDamage(_player.GetFirstAttackDamage);
                _turnbasedText.text = "Player used Sword Slash, causing" + _player.GetFirstAttackDamage + " of damage!";
            }
        }

    }

    private void StoreAttack ()
    {

    }

    public void SecondPlayerAttack()
    {
        _turnbasedText.text = "Player used Super Fist, causing" + _player.GetSecondAttackDamage + " of damage!";
    }

    public void ThirdPlayerAttack()
    {
        _turnbasedText.text = "Player used Sword Slash, causing" + _player.GetThirdAttackDamage + " of damage!";
    }

    public void EnemyTakeDamage()
    {

    }

    private int RandomCriticalDamage()
    {
        return Random.Range(1, 21);
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
