using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySelector : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image enemyImage;
    [SerializeField] private Sprite[] backgroundSprite;
    [SerializeField] private Sprite[] enemySprite;

    private void Awake()
    {
        backgroundImage.sprite = backgroundSprite[Random.Range(0, backgroundSprite.Length)];
        enemyImage.sprite = enemySprite[Random.Range(0, enemySprite.Length)];
    }
}
