using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTurnBased : MonoBehaviour
{
    private player _player;
    private int playerLife = 10;

    private GameObject _turnbasedCanvas;
    private turnbasedScript _turnbasedScript;

    #region Getter & Setters
    public player GetPlayerScript { get => _player; }
    public int GetPlayerLife { get => playerLife; }
    public int SetPlayerLife { set => playerLife = value; }

    #endregion

    private void Awake()
    {
        _player = GetComponent<player>();
    }

    private void Start()
    {
        _turnbasedCanvas = GameObject.Find("Turnbased Canvas");
        _turnbasedScript = _turnbasedCanvas.GetComponent<turnbasedScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _turnbasedScript.ActivateTurnBased();
            _player.DisablePlayerMovement();
            _turnbasedScript.SetEnemyScript = collision.GetComponent<Enemy>();
        }
    }

    public void OnExitBattle ()
    {
        _player.SetMoveSpeed = _player.OriginalMoveSpeed;
        _player.FrictionAmount = _player.OriginalFrictionAmount;
    }
}
