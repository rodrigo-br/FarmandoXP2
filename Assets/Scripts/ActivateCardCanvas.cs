using UnityEngine;
using UnityEngine.UI;

public class ActivateCardCanvas : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private int index;
    public void ActiveCard()
    {
        Debug.Log("Cavalinho");
        AudioManager.Instance.OpenCardCanvas(backgroundImage.sprite, index);
    }
}
