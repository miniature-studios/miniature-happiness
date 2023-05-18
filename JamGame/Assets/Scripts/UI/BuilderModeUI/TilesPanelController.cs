using Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class TilesPanelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("==== For tests ====")]
    [SerializeField] private GameObject testUI;
    [Header("==== Require variables ====")]
    [SerializeField] private ScrollViewHider scrollHider;
    [SerializeField] private TileBuilderController tileBuilderController;
    [SerializeField] private Transform Container;

    private bool mouseOverUI = false;
    private bool mouseUIClicked = false;
    private TileUI uiTileClicked = null;

    public void Start()
    {
        _ = CreateUIElement(testUI);
    }

    public TileUI CreateUIElement(GameObject UIPrefab)
    {
        GameObject UiElement = Instantiate(UIPrefab, Container);
        TileUI.InitAnsver ansver = UiElement.GetComponent<TileUI>().Init(MouseUIClick);
        if (ansver.Merged)
        {
            Destroy(UiElement);
            return ansver.MergedTo;
        }
        else
        {
            return UiElement.GetComponent<TileUI>();
        }
    }

    public void DeselectTile(bool mouse_pressed)
    {
        if (mouse_pressed && mouseOverUI)
        {
            tileBuilderController.DeleteTile();
        }
        mouseUIClicked = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mouseUIClicked)
        {
            Result result = tileBuilderController.CreateTile(uiTileClicked.TileUnionPrefab);
            if (result.Success)
            {
                uiTileClicked.TakeOne();
            }
        }
        mouseOverUI = false;
    }

    public void MouseUIClick(TileUI uITile)
    {
        mouseUIClicked = true;
        uiTileClicked = uITile;
    }
}

