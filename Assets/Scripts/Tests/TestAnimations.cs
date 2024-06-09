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
    [SerializeField] private float endValue = 1.1f;
    [SerializeField] private float DOScaleDuration = 1;


    private Outline outline;
    private Tweener colorTween;
    private Tweener distanceTween;
//=======================================================================================================================================================================
    void Start()
    {

    }

    void AnimateOutlineEnter()
    {
        // Animação de cor do contorno
        colorTween = outline.DOColor(Color.green, 1f)
            .SetLoops(-1, LoopType.Yoyo); // Animação vai e volta

        // Animação de distância do contorno
        distanceTween = DOTween.To(() => outline.effectDistance, x => outline.effectDistance = x, new Vector2(5, 5), 1f)
            .SetLoops(-1, LoopType.Yoyo); // Animação vai e volta
    }
    void AnimationOutlineOut(){
        colorTween.Kill();
        distanceTween.Kill();
        outline.effectColor = Color.black;
    }
    //===============================================================================================================================================================

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
    
    }

    // ACONTECE TODA VEZ QUE O MOUSE PASSA POR CIMA DA IMAGEM
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.transform.DOScale(endValue, DOScaleDuration);

        // Certifique-se de que o GameObject tem um componente Image
        image = GetComponent<Image>();
        if (image != null)
        {
            // Adicionar o componente Outline se ele não existir
            outline = image.gameObject.GetComponent<Outline>();
            if (outline == null)
            {
                outline = image.gameObject.AddComponent<Outline>();
            }

            // Configurar animação inicial do Outline
            outline.effectColor = Color.black; // Defina a cor inicial do contorno
            outline.effectDistance = new Vector2(2, 2); // Defina a distância inicial do contorno

            // Iniciar animação com DoTween
            AnimateOutlineEnter();
        }
        else
        {
            Debug.LogError("O GameObject não tem um componente Image.");
        }
    }

    // ACONTECE TODA VEZ QUE O MOUSE SAI DE CIMA DA IMAGEM
    public void OnPointerExit(PointerEventData eventData)
    {
        image.transform.DOScale(imageOriginalSize, DOScaleDuration);
        AnimationOutlineOut();
    }



    // --------------------------------------------------
    // ANIMA��ES PRONTAS
    // ---------------------------------------------------


    // ANIMA��O FINAL PARA CASO X
    private void CASOX(Vector2 vector)
    {
        image.transform.DOPunchScale(new Vector3(1, 2, 3), 1, 5, 0.3f);
    }

    // ANIMA��O FINAL PARA CASO Y
    public void CASOY(PointerEventData eventData)
    {
        image.transform.DOScale(2, 1);
    }
}
