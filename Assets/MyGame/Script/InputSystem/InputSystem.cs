using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    // values input.
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool IsAttack;

    // movement settings
    public bool analogMovement;

    // mouse cursor settings
    public bool lockCursor = true;
    public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

#endif
    // cập nhật value vào 1 hàm để hứng value.
    public void MoveInput(Vector2 NewMoveDirection)
    {
        move = NewMoveDirection;
    }

    public void LookInput(Vector2 NewLookDirection)
    {
        look = NewLookDirection;
    }

    public void JumpInput(bool NewJumpState)
    {
        jump = NewJumpState;
    }

    public void SprintInput(bool NewSprintState)
    {
        sprint = NewSprintState;
    }


    // khi vào game khóa trỏ không cho hiện và khi vào menu vẫn sử dụng dc.
    private void SetcursorState(bool NewCursorState)
    {
        Cursor.lockState = NewCursorState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    //Khi game lấy lại focus (ví dụ: alt-tab rồi quay lại), tự động khóa lại con trỏ.
    private void OnApplicationFocus(bool focus)
    {
        SetcursorState(lockCursor);
    }
}
