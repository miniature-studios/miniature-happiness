using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonRunGame : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    LocationBuilder _locationbuilder;
    [SerializeField]
    Location _location;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_locationbuilder.ValidateLocation())
        {
            _location.InitGameMode();
        }
    }
}
