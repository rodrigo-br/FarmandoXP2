using UnityEngine.SceneManagement;
using UnityEngine;

public class SingletonCanvas : MonoBehaviour
{
    private Canvas canvas;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        canvas.worldCamera = Camera.main;
    }
}
