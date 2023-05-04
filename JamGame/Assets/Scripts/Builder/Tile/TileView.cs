using UnityEngine;

public class TileView : MonoBehaviour
{
    [SerializeField] public Material startMaterial;
    [SerializeField] public Material transparentMaterial;
    [SerializeField] public Material errorMaterial;
    private Renderer[] renderers;

    private void Awake()
    {
        SetActileChilds(transform);
        renderers = GetComponentsInChildren<Renderer>();
        SetDefaultMaterial();
    }

    private void SetActileChilds(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);
            SetActileChilds(child);
        }
    }

    public void SetDefaultMaterial()
    {
        foreach (Renderer render in renderers)
        {
            render.materials = new Material[1] { startMaterial };
        }
    }

    public void SetSelectedMaterial()
    {
        foreach (Renderer render in renderers)
        {
            render.materials = new Material[1] { transparentMaterial };
        }
    }

    public void SetSelectedAndErroredMaterial()
    {
        foreach (Renderer render in renderers)
        {
            render.materials = new Material[2] { transparentMaterial, errorMaterial };
        }
    }
}
