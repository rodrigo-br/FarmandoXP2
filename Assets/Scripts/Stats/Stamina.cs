using System;
using UnityEngine;

public class Stamina
{
    private int maxStamina;
    private int currentStamina;
    public static event Action OnStaminaChange;
    public static event Action OnMaxStaminaChange;

    public Stamina(int maxHealth)
    {
        this.maxStamina = maxHealth;
        this.currentStamina = maxHealth;
    }

    public int GetCurrentStamina()
    {
        return currentStamina;
    }

    public void ChangeStaminaByAmount(int amount)
    {
        SetCurrentStamina(Mathf.Clamp(currentStamina + amount, 0, maxStamina));
    }

    public void SetCurrentStamina(int currentHealth)
    {
        this.currentStamina = currentHealth;
        OnStaminaChange?.Invoke();
    }

    public void ChangeMaxStaminaByAmount(int amount)
    {
        SetMaxStamina(Mathf.Clamp(maxStamina + amount, 0, int.MaxValue));
    }

    public void SetMaxStamina(int maxHealth)
    {
        this.maxStamina = maxHealth;
        OnMaxStaminaChange?.Invoke();
    }

    public int GetMaxStamina()
    {
        return maxStamina;
    }
}
