using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
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
        if (isChangingPage) { return; }
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            isChangingPage = true;
            StartCoroutine(NextPageCoroutine());
        }
    }

    private IEnumerator NextPageCoroutine()
    {
        ScreenFader.Instance.FadeOn(0.3f);
        yield return new WaitForSeconds(0.3f);
        pages[currentPage].gameObject.SetActive(false);
        currentPage++;
        if (currentPage < totalPages)
        {
            pages[currentPage].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            ScreenFader.Instance.FadeOff(0.3f);
            yield return new WaitForSeconds(0.3f);
            isChangingPage = false;
            yield break;
        }
        GameManager.Instance.NextScene();
    }
}
