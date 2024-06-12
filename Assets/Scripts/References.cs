using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class References : MonoBehaviour
{
    [field: SerializeField] public TextMeshProUGUI Score { get; private set; }
    [field: SerializeField] public Image[] Stars { get; private set; }
    [field: SerializeField] public TextMeshProUGUI Message { get; private set; }
    [field: SerializeField] public Messages[] FiveStarMessages { get; private set; }
    [field: SerializeField] public Messages[] FourStarMessages { get; private set; }
    [field: SerializeField] public Messages[] ThreeStarMessages { get; private set; }
    [field: SerializeField] public Messages[] TwoStarMessages { get; private set; }
    [field: SerializeField] public Messages[] OneStarMessages { get; private set; }
    [field: SerializeField] public Messages[] ZeroStarMessages { get; private set; }
}
