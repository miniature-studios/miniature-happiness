using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TileUnion.Tile
{
    [AddComponentMenu("Scripts/TileUnion/Tile/TileUnion.Tile.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private GameObject foundation;

        [SerializeField]
        private Material transparentMaterial;

        [SerializeField]
        private Material errorMaterial;

        [SerializeField]
        private Material defaultMaterial;

        private List<Renderer> renderers = new();
        private Dictionary<State, Material> materialsByState;

        public enum State
        {
            Default,
            Selected,
            SelectedOverlapping
        }

        private void Awake()
        {
            SetActiveChilds(transform);
            renderers = GetComponentsInChildren<Renderer>().ToList();
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].GetComponentInParent<IgnoreCatch>())
                {
                    renderers.RemoveAt(i);
                    i--;
                }
            }
            foreach (Renderer renderer in renderers)
            {
                renderer.SetMaterials(new List<Material>());
            }
            materialsByState = new()
            {
                { State.Default, defaultMaterial },
                { State.Selected, transparentMaterial },
                { State.SelectedOverlapping, errorMaterial },
            };
            SetMaterial(State.Default);
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

        public void SetMaterial(State state)
        {
            if (foundation != null)
            {
                foundation.SetActive(state != State.Selected);
            }

            foreach (Renderer renderer in renderers)
            {
                renderer.sharedMaterial = materialsByState[state];
            }
        }
    }
}
