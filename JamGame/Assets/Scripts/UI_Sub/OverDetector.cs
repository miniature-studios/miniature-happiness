using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverDetector : MonoBehaviour
{
    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult r in results)
            if (r.gameObject.GetComponent<OverDetector>() != null)
                return true;
        return false;
    }
}
