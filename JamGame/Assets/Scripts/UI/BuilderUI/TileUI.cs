using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TileUnion TileUnionPrefab;
    public TMP_Text Text;
    public TMP_Text Counter;

    private Action<TileUI> clickEvent;
    private bool over = false;

    public class InitAnsver
    {
        public bool Merged;
        public TileUI MergedTo;
        public InitAnsver(bool merged, TileUI merged_to)
        {
            Merged = merged;
            MergedTo = merged_to;
        }
    }

    public InitAnsver Init(Action<TileUI> click_event)
    {
        clickEvent = click_event;
        IEnumerable<TileUI> uis = transform.parent.GetComponentsInChildren<TileUI>().Where(x => x != this && x.Text.text == Text.text);
        if (uis.Count() > 0)
        {
            uis.First().Counter.text = Convert.ToString(Convert.ToInt32(uis.First().Counter.text) + 1);
            return new InitAnsver(true, uis.First());
        }
        else
        {
            return new InitAnsver(false, null);
        }
    }

    public void TakeOne()
    {
        if (Convert.ToInt32(Counter.text) > 1)
        {
            Counter.text = Convert.ToString(Convert.ToInt32(Counter.text) - 1);
        }
        else
        {
            Destroy(gameObject);
        }
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
