using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    private List<Transform> pages;
    private int currentPage;
    private int totalPages;

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
            pages[currentPage].gameObject.SetActive(false);
            currentPage++;
            if (currentPage < totalPages)
            {
                pages[currentPage].gameObject.SetActive(true);
                return;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
