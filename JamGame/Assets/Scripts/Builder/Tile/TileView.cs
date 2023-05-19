using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private Material startMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Material errorMaterial;

    private Renderer[] renderers;
    private readonly Dictionary<TileMaterial, Material[]> materailPairs = new();

    public enum TileMaterial
    {
        Default,
        Transparent,
        TransparentAndError
    }

    private void Awake()
    {
        SetActiveChilds(transform);
        renderers = GetComponentsInChildren<Renderer>();
        materailPairs.Add(TileMaterial.Default, new Material[1] { startMaterial });
        materailPairs.Add(TileMaterial.Transparent, new Material[1] { transparentMaterial });
        materailPairs.Add(TileMaterial.TransparentAndError, new Material[2] { transparentMaterial, errorMaterial });
        SetMaterial(TileMaterial.Default);
    }

    private void SetActiveChilds(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);
            SetActiveChilds(child);
        }
    }

    public void SetMaterial(TileMaterial material)
    {
        foreach (Renderer render in renderers)
        {
            render.materials = materailPairs[material];
        }
    }
}
