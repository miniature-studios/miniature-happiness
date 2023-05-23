using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomInventoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TileUnion tileUnionPrefab;
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text counter;

    private Action<RoomInventoryUI> clickEvent;
    private bool over = false;

    public TileUnion TileUnionPrefab => tileUnionPrefab;

    public class InitAnsver
    {
        public bool Merged;
        public RoomInventoryUI MergedTo;
        public InitAnsver(bool merged, RoomInventoryUI merged_to)
        {
            Merged = merged;
            MergedTo = merged_to;
        }
    }

    public InitAnsver Init(Action<RoomInventoryUI> click_event)
    {
        clickEvent = click_event;
        IEnumerable<RoomInventoryUI> uis = transform.parent.GetComponentsInChildren<RoomInventoryUI>().Where(x => x != this && x.text.text == text.text);
        if (uis.Count() > 0)
        {
            uis.First().counter.text = Convert.ToString(Convert.ToInt32(uis.First().counter.text) + 1);
            return new InitAnsver(true, uis.First());
        }
        else
        {
            return new InitAnsver(false, null);
        }
    }

    public void TakeOne()
    {
        if (Convert.ToInt32(counter.text) > 1)
        {
            counter.text = Convert.ToString(Convert.ToInt32(counter.text) - 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
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
