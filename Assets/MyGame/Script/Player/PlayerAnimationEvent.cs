using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerAttack playerattack;

    private void Start()
    {
        playerattack = GetComponent<PlayerAttack>();
    }

    private void BlockedAttack()
    {
        
    }

    private void PlayerAttacKHandleft()
    {
        
            playerattack.AttackPointLeftHand();
    }
    private void PlayerAttackHandRight()
    {
        playerattack.AttackPointRightHand();
    }
private void PlayerAttackLeg()
    {
        playerattack.AttackPointLeg();
    }
    
} // Thêm event trong animation để hoạt động 
