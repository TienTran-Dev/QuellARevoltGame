using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    
    public int health;
    public int shied;
    public int dame;

    public void TakeDamage()
    {
        health -= dame;
        if (health < 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void CurrentShied()
    {
        shied -= dame;
        Debug.Log($"{shied}");
        if (shied < 0)
        {
            health -= dame;
        }
    }
   
}
