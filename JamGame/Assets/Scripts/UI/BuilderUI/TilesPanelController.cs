using Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class TilesPanelController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("==== For tests ====")]
    [SerializeField] private TileUI testUI;
    [Header("==== Require variables ====")]
    [SerializeField] private ScrollViewHider scrollHider;
    [SerializeField] private TileBuilderController tileBuilderController;
    [SerializeField] private Transform container;

    public ScrollViewHider ScrollViewHider => scrollHider;

    private bool mouseOverUI = false;
    private bool mouseUIClicked = false;
    private TileUI uiTileClicked = null;

    public void Start()
    {
        _ = CreateUIElement(testUI);
    }

    public TileUI CreateUIElement(TileUI ui_prefab)
    {
        TileUI UiElement = Instantiate(ui_prefab, container);
        TileUI.InitAnsver ansver = UiElement.Init(MouseUIClick);
        if (ansver.Merged)
        {
            Destroy(UiElement);
            return ansver.MergedTo;
        }
        else
        {
            return UiElement;
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
    public void OnPointerEnter(PointerEventData event_data)
    {
        mouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData event_data)
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

    public void MouseUIClick(TileUI ui_tile)
    {
        mouseUIClicked = true;
        uiTileClicked = ui_tile;
    }
}

