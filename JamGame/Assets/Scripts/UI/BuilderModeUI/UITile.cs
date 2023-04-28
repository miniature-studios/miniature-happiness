using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Common;

public class UITile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public GameObject TileUnionPrefab;
    [SerializeField] public Action<UITile> clickEvent;
    [SerializeField] TMP_Text text;
    bool over = false;
    public void Init(GameObject TileUnionPrefab, Action<UITile> clickEvent)
    {
        this.TileUnionPrefab = TileUnionPrefab;
        this.clickEvent = clickEvent;
        text.text = TileUnionPrefab.GetComponent<TileUnion>().UiName;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && over)
        {
            clickEvent(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        over = false;
    }
}
