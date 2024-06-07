using UnityEngine;

public class Gold : MonoBehaviour
{
    [SerializeField] private float goldFactor = 1;
    [SerializeField][Range(0, 1)] private float stealFactor = 0.2f;
    private int gold;

    private void Awake()
    {
        gold = 0;
    }

    public void ChangeGoldByAmount(int amount)
    {
        if (amount < 0 && Mathf.Abs(amount) > gold)
        {
            return;
        }

        gold += (int)(amount * goldFactor);
    }

    public void SetGoldFactor(float goldFactor)
    {
        this.goldFactor = goldFactor;
    }

    public void ChangeGoldFactorByAmount(float amount)
    {
        goldFactor = Mathf.Clamp(goldFactor + amount, 0, float.MaxValue);
    }

    public void SetStealFactor(float stealFactor)
    {
        this.stealFactor = stealFactor;
    }

    public void ChangeStealFactorByAmount(float amount)
    {
        stealFactor = Mathf.Clamp(stealFactor + amount, 0, 1);
    }

    public int GetStealed(float maxSteal)
    {
        int amountStealed = Random.Range(0, (int)(gold * maxSteal));
        ChangeGoldByAmount(-amountStealed);
        return amountStealed;
    }

    public void Steal(Gold target)
    {
        gold += target.GetStealed(stealFactor);
    }
}
