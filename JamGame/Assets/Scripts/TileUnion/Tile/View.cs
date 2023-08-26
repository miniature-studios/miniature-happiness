using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TileUnion.Tile
{
    [AddComponentMenu("Scripts/TileUnion.Tile.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Material transparentMaterial;

        [SerializeField]
        private Material errorMaterial;

        private Dictionary<Renderer, Material[]> initialMaterialsMap = new();
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
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                initialMaterialsMap.Add(renderer, renderer.materials);
            }
            materialsByState = new()
            {
                { State.Selected, new Material[1] { transparentMaterial } },
                {
                    State.SelectedOverlapping,
                    new Material[1] { errorMaterial }
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

        public void SetMaterial(State state)
        {
            foreach (KeyValuePair<Renderer, Material[]> materialsMap in initialMaterialsMap)
            {
                materialsMap.Key.materials = state == State.Default
                    ? materialsMap.Value
                    : Enumerable.Range(0, materialsMap.Value.Count())
                        .Select(x => materialsByState[state].ToList()).Aggregate((x, y) => x.Concat(y).ToList()).ToArray();
            }
        }
    }
}
