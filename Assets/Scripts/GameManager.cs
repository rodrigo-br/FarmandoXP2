using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Observer Observer { get; private set; }
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Observer = new Observer();
        }
        else
        {
            Destroy(this);
        }
    }
}
