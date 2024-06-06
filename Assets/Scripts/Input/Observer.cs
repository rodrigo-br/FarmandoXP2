using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Observer : InputActions.IGamePlayActions
{
    private readonly InputActions inputActions;
    public event Action<Vector2> OnSelectAction;
    Vector2 targetPosition;

    public Observer()
    {
        inputActions = new InputActions();
        inputActions.GamePlay.SetCallbacks(this);
        EnableGamePlayMap();
    }

    public void EnableGamePlayMap(bool disableAll = true)
    {
        if (disableAll)
        {
            DisableAllGameMaps();
        }
        inputActions.GamePlay.Enable();
    }

    public void DisableAllGameMaps()
    {
        inputActions.GamePlay.Disable();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSelectAction?.Invoke(targetPosition);
        }
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        targetPosition = context.ReadValue<Vector2>();
    }
}
