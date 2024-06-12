using UnityEngine;

public class CardsPanel : MonoBehaviour
{
    private void Start()
    {
        ActivateCards(ScoreManager.Instance.GetLevel() + 2);
    }

    private void ActivateCards(int amount)
    {
        int activated = 0;
        foreach (RectTransform child in this.GetComponent<RectTransform>())
        {
            child.gameObject.SetActive(true);
            activated++;
            if (activated == amount) return;
        }
    }
}
