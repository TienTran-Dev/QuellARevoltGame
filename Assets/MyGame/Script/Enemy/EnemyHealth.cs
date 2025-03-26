using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    public int shied;
    public int dame;
    private Animator _Animator;
    private int _IDHit = Animator.StringToHash("Hit");

    private void Start()
    {
        _Animator = GetComponent<Animator>();
    }
    public void TakeDamage()
    {
            health -= dame;
        _Animator.SetTrigger(_IDHit);
        Debug.Log($"{health}");
        if (health <= 0)
        {
            if(EnemyController.Instance != null)
            {
                EnemyController.Instance.CountEnemyDead();
            }
          
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
