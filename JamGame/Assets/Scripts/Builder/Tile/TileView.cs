using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private Material errorMaterial;

    private Renderer[] renderers;
    private Dictionary<TileMaterial, Material[]> materialsByState;

    public enum TileMaterial
    {
        Default,
        Selected,
        SelectedOverlapping
    }

    private void Awake()
    {
        SetActiveChilds(transform);
        renderers = GetComponentsInChildren<Renderer>();
        materialsByState = new()
        {
            { TileMaterial.Default, new Material[1] { defaultMaterial } },
            { TileMaterial.Selected, new Material[1] { transparentMaterial } },
            { TileMaterial.SelectedOverlapping, new Material[2] { transparentMaterial, errorMaterial } },
        };
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
            render.materials = materialsByState[material];
        }
    }
}
