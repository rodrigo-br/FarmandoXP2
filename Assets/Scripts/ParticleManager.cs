using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject clearFXPrefab;
    [SerializeField] private GameObject breakFXPrefab;
    [SerializeField] private GameObject bombFXPrefab;

    public void ClearPieceFXAt(Tile tile)
    {
        GameObject clearFX = Instantiate(clearFXPrefab) as GameObject;
        clearFX.GetComponent<RectTransform>().SetParent(tile.RectTransform, false);

        ParticlePlayer player = clearFX.GetComponent<ParticlePlayer>();

        if (player == null) { return; }

        player.Play();
    }

    public void ClearBombFXAt(Tile tile)
    {
        GameObject bombFX = Instantiate(bombFXPrefab) as GameObject;
        bombFX.GetComponent<RectTransform>().SetParent(tile.RectTransform, false);

        ParticlePlayer player = bombFX.GetComponent<ParticlePlayer>();

        if (player == null) { return; }

        player.Play();
    }

    public void BreakPieceFXAt(Tile tile)
    {
        GameObject breakFX = Instantiate(breakFXPrefab) as GameObject;
        breakFX.GetComponent<RectTransform>().SetParent(tile.RectTransform, false);

        ParticlePlayer player = breakFX.GetComponent<ParticlePlayer>();

        if (player == null) { return; }

        player.Play();
    }
}
