using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public Enemy enemy;

    public void Start()
    {
        Destroy(enemy);
    }

    public void Update()
    {
        FunctionA();
        //FunctionB();
        FunctionC();
    }

    public void FunctionA()
    {
        enemy.health = 5;
    }

    private void FunctionB()
    {
        enemy.name = "...";
    }

    private void FunctionC()
    {
        this.NullableEnemyDebug("!ReferenceEquals()", !ReferenceEquals(enemy, null));
        this.NullableEnemyDebug("'is not' operator", enemy is not null);
        this.NullableEnemyDebug("implicit Component", enemy);
        this.NullableEnemyDebug("!Equals()", !enemy.Equals(null));
        this.NullableEnemyDebug("!= operator", enemy != null);
    }

    private void NullableEnemyDebug(string label, bool result)
    {
        if (result)
        {
            Debug.Log($"Enemy is NOT null ({label})");
        }
        else
        {
            Debug.LogWarning($"Enemy is null ({label})");
        }
    }
}
