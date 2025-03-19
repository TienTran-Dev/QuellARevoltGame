using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerAttack playerattack;

    private void Start()
    {
        playerattack = GetComponent<PlayerAttack>();
    }

    private void PlayerAttacK()
    {
        if (playerattack != null)
        {
            playerattack.AttackPoint();
        }
    }
} // Thêm event trong animation để hoạt động 
