using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimOutline : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    private Outline outline;
    private Tweener colorTween;
    private Tweener distanceTween;

    private void OnDisable()
    {
        AnimationOutlineOut();
    }

    private void OnDestroy()
    {
        AnimationOutlineOut();
    }

    void AnimateOutlineEnter()
    {
        if (outline == null) { return; }
        // Animação de cor do contorno
        colorTween = outline.DOColor(Color.green, 0.5f)
            .SetLoops(-1, LoopType.Yoyo); // Animação vai e volta

        // Animação de distância do contorno
        distanceTween = DOTween.To(() => outline.effectDistance, x => outline.effectDistance = x, new Vector2(3, 3), 0.5f)
            .SetLoops(-1, LoopType.Yoyo); // Animação vai e volta
    }
    void AnimationOutlineOut(){
        colorTween?.Kill();
        distanceTween?.Kill();
        if (outline != null)
        {
            outline.effectColor = Color.black;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
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
    public void OnPointerExit(PointerEventData eventData){
        AnimationOutlineOut();
    }
}
