using DG.Tweening;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    Vector3 imageOriginalSize;

    //DO PUNCH SCALE
    [Header("DO PUNCH SCALE")]
    [SerializeField] private Vector3 punch = Vector3.one;
    [SerializeField] private int vibrato = 10;
    [SerializeField] private float duration = 1;
    [Range(0, 1)][SerializeField] private float elasticity = 1;

    //DO SCALE
    [Header("DO SCALE")]
    [SerializeField] private float endValue = 2;
    [SerializeField] private float DOScaleDuration = 1;



    public void OnEnable()
    {
        imageOriginalSize = image.transform.localScale;
        StartCoroutine(SubscripteRoutine());
    }

    public void OnDisable()
    {
        GameManager.Instance.Observer.OnSelectAction -= OnMouseClick;
    }

    private IEnumerator SubscripteRoutine()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.Observer.OnSelectAction += OnMouseClick;
    }

    // ACONTECE TODA VEZ QUE O MOUSE CLICA
    private void OnMouseClick(Vector2 vector)
    {
        image.transform.DOPunchScale(punch, duration, vibrato, elasticity);
    }

    // ACONTECE TODA VEZ QUE O MOUSE PASSA POR CIMA DA IMAGEM
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.transform.DOScale(endValue, DOScaleDuration);
    }

    // ACONTECE TODA VEZ QUE O MOUSE SAI DE CIMA DA IMAGEM
    public void OnPointerExit(PointerEventData eventData)
    {
        image.transform.DOScale(imageOriginalSize, DOScaleDuration);
    }



    // --------------------------------------------------
    // ANIMAÇÔES PRONTAS
    // ---------------------------------------------------


    // ANIMAÇÃO FINAL PARA CASO X
    private void CASOX(Vector2 vector)
    {
        image.transform.DOPunchScale(new Vector3(1, 2, 3), 1, 5, 0.3f);
    }

    // ANIMAÇÃO FINAL PARA CASO Y
    public void CASOY(PointerEventData eventData)
    {
        image.transform.DOScale(2, 1);
    }
}
