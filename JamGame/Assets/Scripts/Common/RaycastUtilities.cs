using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common
{
    public static class RaycastUtilities
    {
        public static bool PointerIsOverUI(Vector2 screenPosition)
        {
            GameObject hitObject = UIRaycast(ScreenPosToPointerData(screenPosition))?.First();
            return hitObject != null && hitObject.layer == LayerMask.NameToLayer("UI");
        }

        public static bool PointerIsOverTargetGO(Vector2 screenPosition, GameObject targetUI)
        {
            IEnumerable<GameObject> go_list = UIRaycast(ScreenPosToPointerData(screenPosition));
            return go_list != null && go_list.Any((x) => x == targetUI);
        }

        public static IEnumerable<GameObject> UIRaycast(Vector2 screenPos)
        {
            return UIRaycast(ScreenPosToPointerData(screenPos));
        }

        private static IEnumerable<GameObject> UIRaycast(PointerEventData pointerData)
        {
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);

            return results.Count < 1 ? null : results.Select(x => x.gameObject);
        }

        private static PointerEventData ScreenPosToPointerData(Vector2 screenPos)
        {
            return new(EventSystem.current) { position = screenPos };
        }
    }
}
