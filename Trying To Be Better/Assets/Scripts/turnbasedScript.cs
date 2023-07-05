using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Combat { playerTurn, enemyTurn, secondEnemyTurn, thirdEnemyTurn}

public class turnbasedScript : MonoBehaviour
{
    private playerTurnBased _playerTurnBased;
    private Camera _camera;
    private Combat _combat = Combat.playerTurn;
    private TextMeshProUGUI tmpro;

    private Enemy _enemyScript;
    private player _player;

    private Vector3 _velocity = Vector3.one;

    #region Getter & Setters
    public Enemy SetEnemyScript { set => _enemyScript = value; }
    #endregion

    private void Awake()
    {
        tmpro = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _playerTurnBased = GameObject.Find("player").GetComponent<playerTurnBased>();
        _player = _playerTurnBased.GetComponent<player>();
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

    public void FirstPlayerAttack ()
    {
        Debug.Log("Sword Slash");

    }

    public void SecondPlayerAttack ()
    {
        Debug.Log("Super Fist");
    }

    public void ThirdPlayerAttack()
    {
        Debug.Log("Magic Spell");
    }
    public void CardButton()
    {
        Debug.Log("The player will use a card power");
    }

    public void RunButton()
    {
        DisableTurnbasedCanvas();
        _enemyScript.DestroySpawnedEnemy();
    }

    public void ActivateTurnBased ()
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
