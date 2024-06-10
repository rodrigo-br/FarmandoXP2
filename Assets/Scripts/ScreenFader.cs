using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : SingletonBase<ScreenFader>
{
    private readonly float solidAlpha = 1f;
    private readonly float clearAlpha = 0f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private float timeToFade = 1f;
    [SerializeField] private Image graphic;

    public override void Awake()
    {
        base.Awake();
    }

    private IEnumerator FadeRoutine(float alpha, float time)
    {
        float startAlpha = graphic.color.a;
        yield return new WaitForSeconds(delay);
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, alpha, elapsedTime / time);
            Color tempColor = graphic.color;
            tempColor.a = newAlpha;
            graphic.color = tempColor;
            yield return null;
        }

        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
    }

    public void FadeOn(float time=0)
    {
        float correctTime = time > 0 ? time : timeToFade;
        StartCoroutine(FadeRoutine(solidAlpha, correctTime));
    }

    public void FadeOff(float time=0)
    {
        float correctTime = time > 0 ? time : timeToFade;
        StartCoroutine(FadeRoutine(clearAlpha, correctTime));
    }
}
