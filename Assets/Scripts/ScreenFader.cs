using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    private readonly float solidAlpha = 1f;
    private readonly float clearAlpha = 0f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private float timeToFade = 1f;

    MaskableGraphic graphic;

    private void Awake()
    {
        graphic = GetComponent<MaskableGraphic>();
    }

    private IEnumerator FadeRoutine(float alpha)
    {
        yield return new WaitForSeconds(delay);
        graphic.CrossFadeAlpha(alpha, timeToFade, true);
    }

    public void FadeOn()
    {
        StartCoroutine(FadeRoutine(solidAlpha));
    }

    public void FadeOff()
    {
        StartCoroutine(FadeRoutine(clearAlpha));
    }
}
