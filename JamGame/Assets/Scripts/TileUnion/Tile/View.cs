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
        private Dictionary<TileState, Material> materialsByState;

        // TODO: move all parameters to animations
        private readonly float selectLiftingHeight = 3;
        private float unselectedFoundationYPosition;
        private float selectedFoundationYPosition;

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

                unselectedFoundationYPosition = foundation.transform.position.y;
                selectedFoundationYPosition = unselectedFoundationYPosition - selectLiftingHeight;
            }
            foreach (Renderer renderer in renderers)
            {
                renderer.SetMaterials(new List<Material>());
            }
            materialsByState = new()
            {
                { TileState.Normal, defaultMaterial },
                { TileState.Selected, transparentMaterial },
                { TileState.SelectedAndErrored, errorMaterial },
            };
            ApplyTileState(TileState.Normal);
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
        public void ApplyTileState(TileState state)
        {
            if (foundation != null)
            {
                bool active = state switch
                {
                    TileState.Normal => true,
                    TileState.Selected => false,
                    TileState.SelectedAndErrored => true,
                    _ => throw new System.ArgumentException()
                };
                foundation.SetActive(active);

                float foundationNewY = state switch
                {
                    TileState.Normal => unselectedFoundationYPosition,
                    TileState.Selected => selectedFoundationYPosition,
                    TileState.SelectedAndErrored => selectedFoundationYPosition,
                    _ => throw new InvalidOperationException()
                };
                foundation.transform.SetLocalYPosition(foundationNewY);
            }

            foreach (Renderer renderer in renderers)
            {
                renderer.sharedMaterial = materialsByState[state];
            }
        }
    }
}
