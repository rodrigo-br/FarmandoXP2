using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScene : MonoBehaviour
{
    private List<Transform> pages;
    private int currentPage;
    private int totalPages;
    private bool isChangingPage = true;

    private void Awake()
    {
        AudioManager.Instance.StopCheerSound();
        currentPage = 0;
        pages = new();
        foreach (Transform page in this.transform)
        {
            pages.Add(page);
        }
        totalPages = pages.Count;
    }

    private IEnumerator UnlockChangingPage()
    {
        yield return new WaitForSeconds(0.3f);
        isChangingPage = false;
    }

    private void Start()
    {
        GameObject referencesGO = GameObject.FindWithTag("References");
        if (referencesGO != null)
        {
            References references = referencesGO.GetComponent<References>();
            if (references != null)
            {
                ScoreManager.Instance.UpdateWinnerScreen(references);
            }
        }
        ScreenFader.Instance.FadeOff(0.3f);
        StartCoroutine(UnlockChangingPage());
    }

    private void Update()
    {
        if (isChangingPage) { return; }
        if (Input.GetMouseButtonDown(0))
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
        GameManager.Instance.NextScene();
    }
}
