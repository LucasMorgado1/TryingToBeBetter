using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Weak, Medium, Strong, Hard};
[CreateAssetMenu(fileName = "EnemyType", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemyScriptableObject : ScriptableObject
{
    public int _hp = default;
    public int _normalAttack = default;
    public int _strongAttack = default;
    public EnemyType _enemyType = default;
    public int _healAmount = default;
}
