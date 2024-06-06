using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Observer Observer { get; private set; }

    private void Awake()
    {
        Observer = new Observer();
    }
}
