using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform[] _teleportPoints = new Transform[5];
    private Queue fila = new Queue();
    private bool playerInteracted = false;
    private GameObject _playerCharacter;
    private float _speed = 0.1f;
    Vector3 _pointA;
    Vector3 _pointB;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _teleportPoints.Length; i++)
        {
            PutOnQueue(_teleportPoints[i]);
        }
        _pointA = _teleportPoints[0].position;
        _pointB = _teleportPoints[1].position;
    }

    private void FixedUpdate()
    {
        if (playerInteracted) { 
            MovingPlayerThroughPoints();
        }
    }

    private void MovingPlayerThroughPoints ()
    {

        _playerCharacter.transform.position = Vector3.MoveTowards(_pointA, _pointB, _speed * Time.deltaTime);
    }

    private void PutOnQueue(object obj)
    {
        fila.Enqueue(obj);
    }

    public bool SetPlayerInteraction(GameObject obj) 
    {
        obj.transform.parent = this.transform;
        _playerCharacter = obj;
        return playerInteracted = true;
    }


}
