using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;
    public GameObject EnemyPrefab;
    public List<Transform> ListEnemyTranForm;
    private int count;
   



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        CreateEnemy();

    }


    public void CreateEnemy()
    {

        for (int i = 0; i < ListEnemyTranForm.Count; i++)
        {
            
            
                Instantiate(EnemyPrefab, ListEnemyTranForm[i].position, Quaternion.identity);
            
           
        }              

    }

    public void CountEnemyDead()
    {
        count++;
        if (count >= 10)
        {
            Debug.Log("Win");
        }
    }
}
