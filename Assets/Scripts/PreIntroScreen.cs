using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreIntroScreen : MonoBehaviour
{
    private List<Transform> pages;
    private int currentPage;
    private int totalPages;
    private bool isChangingPage = false;

    private void Awake()
    {
        currentPage = 0;
        pages = new();
        foreach (Transform page in this.transform)
        {
            pages.Add(page);
        }
        totalPages = pages.Count;
    }

    private void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            if (isChangingPage) { return; }
            isChangingPage = true;
            StartCoroutine(NextPageCoroutine());
        }
    }

    private IEnumerator NextPageCoroutine()
    {
        ScreenFader.Instance.FadeOn(0.3f);
        yield return new WaitForSeconds(0.3f);
        pages[currentPage].gameObject.SetActive(false);
        GameManager.Instance.NextScene();
    }
}
