using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashWhite : MonoBehaviour
{
    [SerializeField] private Material whiteFlashMaterial;
    [SerializeField] private float restoreDefaultMaterialTime = 0.3f;
    private Image image; 
    private Material defaultMaterial;

    private void Awake()
    {
        image = GetComponent<Image>();
        defaultMaterial = image.material;
    }

    public IEnumerator FlashRoutine()
    {
        image.material = whiteFlashMaterial;
        yield return new WaitForSeconds(restoreDefaultMaterialTime);
        image.material = defaultMaterial;
    }
}
