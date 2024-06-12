using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class References : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI score { get; private set; }
    [field: SerializeField] public Image[] stars { get; private set; }
    [field: SerializeField] public TextMeshProUGUI message { get; private set; }
}
