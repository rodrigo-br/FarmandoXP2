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

    //DO SCALE
    [Header("DO SCALE")]
    [SerializeField] private float endValue = 1.1f;
    [SerializeField] private float DOScaleDuration = 1;

    private Tweener scaleTweenUp;
    private Tweener scaleTweenDown;
    private bool disable = false;

    //=======================================================================================================================================================================
    void AnimationOutlineOut(){
        scaleTweenUp?.Kill();
        scaleTweenDown?.Kill();
    }
    //===============================================================================================================================================================

    public void DisableAll()
    {
        disable = true;
        AnimationOutlineOut();
    }

    public void SetDisable(bool value)
    {
        disable = value;
    }

    public void OnEnable()
    {
        imageOriginalSize = image.transform.localScale;
        StartCoroutine(SubscripteRoutine());
    }

    public void OnDisable()
    {
        AnimationOutlineOut();
        GameManager.Instance.Observer.OnSelectAction -= OnMouseClick;
    }

    public void OnDestroy()
    {
        AnimationOutlineOut();
    }

    private IEnumerator SubscripteRoutine()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.Observer.OnSelectAction += OnMouseClick;
    }

    // ACONTECE TODA VEZ QUE O MOUSE CLICA
    private void OnMouseClick(Vector2 vector)
    {
    
    }

    // ACONTECE TODA VEZ QUE O MOUSE PASSA POR CIMA DA IMAGEM
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disable) { return; }
        if (image == null || image.transform == null) { return; }
        scaleTweenUp = image.transform.DOScale(endValue, DOScaleDuration);
        AudioManager.Instance.PlayHoverMouseGem();

        // Certifique-se de que o GameObject tem um componente Image
        //image = GetComponent<Image>();
        //if (image != null)
        //{
        //    // Adicionar o componente Outline se ele não existir
        //    outline = image.gameObject.GetComponent<Outline>();
        //    if (outline == null)
        //    {
        //        outline = image.gameObject.AddComponent<Outline>();
        //    }

        //    // Configurar animação inicial do Outline
        //    outline.effectColor = Color.black; // Defina a cor inicial do contorno
        //    outline.effectDistance = new Vector2(2, 2); // Defina a distância inicial do contorno

        //    // Iniciar animação com DoTween
        //    AnimateOutlineEnter();
        //}
        //else
        //{
        //    Debug.LogError("O GameObject não tem um componente Image.");
        //}
    }

    // ACONTECE TODA VEZ QUE O MOUSE SAI DE CIMA DA IMAGEM
    public void OnPointerExit(PointerEventData eventData)
    {
        if (disable) { return; }
        scaleTweenUp?.Kill();
        if (image == null || image.transform == null) { return; }
        scaleTweenDown = image.transform.DOScale(imageOriginalSize, DOScaleDuration);
    }



    // --------------------------------------------------
    // ANIMA��ES PRONTAS
    // ---------------------------------------------------


    // ANIMA��O FINAL PARA CASO X
    //private void CASOX(Vector2 vector)
    //{
    //    image.transform.DOPunchScale(new Vector3(1, 2, 3), 1, 5, 0.3f);
    //}

    //// ANIMA��O FINAL PARA CASO Y
    //public void CASOY(PointerEventData eventData)
    //{
    //    image.transform.DOScale(2, 1);
    //}
}
