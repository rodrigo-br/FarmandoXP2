using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => AudioManager.Instance.OpenVolumeModal());
    }
}
