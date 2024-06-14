using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScene : MonoBehaviour
{
    private List<Transform> pages;
    private int currentPage;
    private int totalPages;
    private bool isChangingPage = false;

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

    private void Start()
    {
        GameObject referencesGO = GameObject.FindWithTag("References");
        if (referencesGO != null)
        {
            References references = referencesGO.GetComponent<References>();
            Debug.Log(references);
            if (references != null)
            {
                Debug.Log($"references.FiveStarMessages: {references.FiveStarMessages}");
                foreach (Messages message in references.FiveStarMessages)
                {
                    Debug.Log(message);
                    Debug.Log(message.message);
                    Debug.Log(message.voice);
                }
                ScoreManager.Instance.UpdateWinnerScreen(references);
            }
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
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
