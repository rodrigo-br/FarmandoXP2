using UnityEngine;

public class CardCanvas : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKey || Input.GetMouseButtonDown(0))
        {
            AudioManager.Instance.CloseCardCanvas();
        }
    }
}
