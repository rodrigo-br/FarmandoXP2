using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour
{
    private List<Transform> pages;
    private int currentPage;
    private int totalPages;
    private bool isChangingPage = true;

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

    private void Start()
    {
        ScreenFader.Instance.FadeOff(0.3f);
        AudioManager.Instance.PlayTutorialPageVoice(currentPage);
        pages[currentPage].gameObject.GetComponentInChildren<Button>()?.onClick.AddListener(() => {
            if (isChangingPage) { return; }
            isChangingPage = true;
            GameManager.Instance.NextScene();
            });
        StartCoroutine(UnlockChangingPage());
    }

    private IEnumerator UnlockChangingPage()
    {
        yield return new WaitForSeconds(0.3f);
        isChangingPage = false;
    }

    public void ChangePage()
    {
        if (isChangingPage) { return; }
        isChangingPage = true;
        AudioManager.Instance.PlayMouseClick();
        StartCoroutine(NextPageCoroutine());
    }

    private IEnumerator NextPageCoroutine()
    {
        ScreenFader.Instance.FadeOn(0.3f);
        yield return new WaitForSeconds(0.3f);
        pages[currentPage].gameObject.SetActive(false);
        currentPage++;
        if (currentPage < totalPages)
        {
            AudioManager.Instance.PlayTutorialPageVoice(currentPage);
            pages[currentPage].gameObject.SetActive(true);
            pages[currentPage].gameObject.GetComponentInChildren<Button>()?.onClick.AddListener(() => {
                if (isChangingPage) { return; }
                isChangingPage = true;
                GameManager.Instance.NextScene();
                });
            yield return new WaitForSeconds(0.2f);
            ScreenFader.Instance.FadeOff(0.3f);
            yield return new WaitForSeconds(0.3f);
            isChangingPage = false;
            yield break;
        }
        GameManager.Instance.NextScene();
    }
}
