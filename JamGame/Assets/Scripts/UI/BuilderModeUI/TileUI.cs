using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Linq;

public class TileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public GameObject TileUnionPrefab;
    [SerializeField] TMP_Text text;
    [SerializeField] public TMP_Text Counter;

    Action<TileUI> clickEvent;
    bool over = false;

    public class InitAnsver
    {
        public bool Merged;
        public TileUI MergedTo;
        public InitAnsver(bool merged, TileUI mergedTo)
        {
            Merged = merged;
            MergedTo = mergedTo;
        }
    }
    public InitAnsver Init(Action<TileUI> clickEvent)
    {
        this.clickEvent = clickEvent;
        var uis = transform.parent.GetComponentsInChildren<TileUI>().Where(x => x != this);
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
