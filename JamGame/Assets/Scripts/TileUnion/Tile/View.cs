using System.Collections.Generic;
using UnityEngine;

namespace TileUnion.Tile
{
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Material defaultMaterial;

        [SerializeField]
        private Material transparentMaterial;

        [SerializeField]
        private Material errorMaterial;

        private Renderer[] renderers;
        private Dictionary<State, Material[]> materialsByState;

        public enum State
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
                { State.Default, new Material[1] { defaultMaterial } },
                { State.Selected, new Material[1] { transparentMaterial } },
                {
                    State.SelectedOverlapping,
                    new Material[2] { transparentMaterial, errorMaterial }
                },
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

        public void SetMaterial(State material)
        {
            foreach (Renderer render in renderers)
            {
                render.materials = materialsByState[material];
            }
        }
    }
}
