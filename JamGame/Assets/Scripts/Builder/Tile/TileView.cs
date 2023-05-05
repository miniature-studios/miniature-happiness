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
        SetMaterial(TileMaterial.Default);
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

    public enum TileMaterial
    {
        Default,
        Transparent,
        TransparentAndError
    }

    public void SetMaterial(TileMaterial material)
    {
        switch (material)
        {
            default:
            case TileMaterial.Default:
                foreach (Renderer render in renderers)
                {
                    render.materials = new Material[1] { startMaterial };
                }
                break;
            case TileMaterial.Transparent:
                foreach (Renderer render in renderers)
                {
                    render.materials = new Material[1] { transparentMaterial };
                }
                break;
            case TileMaterial.TransparentAndError:
                foreach (Renderer render in renderers)
                {
                    render.materials = new Material[2] { transparentMaterial, errorMaterial };
                }
                break;
        }
    }
}
