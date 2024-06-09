using UnityEngine.SceneManagement;
using UnityEngine;

public class SingletonCanvas : SingletonBase<SingletonCanvas>
{
    private Canvas canvas;
    public override void Awake()
    {
        base.Awake();
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
