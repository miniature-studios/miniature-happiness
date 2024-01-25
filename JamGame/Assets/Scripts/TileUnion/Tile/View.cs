using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TileUnion.Tile
{
    [AddComponentMenu("Scripts/TileUnion/Tile/TileUnion.Tile.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private GameObject foundation;

        [SerializeField]
        private Transform tileBase;

        [SerializeField]
        private Material transparentMaterial;

        [SerializeField]
        private Material errorMaterial;

        [SerializeField]
        private Material defaultMaterial;

        [ReadOnly]
        [SerializeField]
        private List<Renderer> renderers = new();

        [ReadOnly]
        [SerializeField]
        private Dictionary<State, Material> materialsByState;

        private readonly float selectLiftingHeight = 3;
        private float unselectedYPosition;
        private float selectedYPosition;

        private void Awake()
        {
            SetActiveChilds(transform);
            renderers = GetComponentsInChildren<Renderer>().ToList();
            if (foundation != null)
            {
                foreach (Renderer toRemove in foundation.GetComponentsInChildren<Renderer>())
                {
                    _ = renderers.Remove(toRemove);
                }

                unselectedYPosition = tileBase.position.y;
                selectedYPosition = unselectedYPosition + selectLiftingHeight;
            }
            foreach (Renderer renderer in renderers)
            {
                renderer.SetMaterials(new List<Material>());
            }
            materialsByState = new()
            {
                { State.Normal, defaultMaterial },
                { State.Selected, transparentMaterial },
                { State.SelectedAndErrored, errorMaterial },
            };
            ApplyTileState(State.Normal);
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

        // Must be called by TileImpl event
        public void ApplyTileState(State state)
        {
            if (foundation != null)
            {
                bool active = state switch
                {
                    State.Normal => true,
                    State.Selected => false,
                    State.SelectedAndErrored => true,
                    _ => throw new System.ArgumentException()
                };
                foundation.SetActive(active);

                float newY = state switch
                {
                    State.Normal => unselectedYPosition,
                    State.Selected => selectedYPosition,
                    State.SelectedAndErrored => selectedYPosition,
                    _ => throw new InvalidOperationException()
                };
                tileBase.SetYPosition(newY);
            }

            foreach (Renderer renderer in renderers)
            {
                renderer.sharedMaterial = materialsByState[state];
            }
        }
    }
}
