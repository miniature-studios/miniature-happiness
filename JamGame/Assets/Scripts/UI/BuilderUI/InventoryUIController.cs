using Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("==== For tests ====")]
    [SerializeField] private RoomInventoryUI testUI;
    [Header("==== Require variables ====")]
    [SerializeField] private TileBuilderController tileBuilderController;
    [SerializeField] private Transform container;
    [SerializeField] private TMP_Text button_text;

    private Animator tilesInventoryAnimator;

    private bool mouseOverUI = false;
    private bool mouseUIClicked = false;
    private RoomInventoryUI uiTileClicked = null;

    private void Awake()
    {
        tilesInventoryAnimator = GetComponent<Animator>();
    }

    private bool inventoryShowed = false;
    public void InventoryButtonClick()
    {
        inventoryShowed = !inventoryShowed;
        tilesInventoryAnimator.SetBool("Showed", inventoryShowed);
        button_text.text = inventoryShowed ? "Close" : "Open";
    }

    private void Start()
    {
        _ = CreateUIElement(testUI);
    }

    public RoomInventoryUI CreateUIElement(RoomInventoryUI ui_prefab)
    {
        RoomInventoryUI UiElement = Instantiate(ui_prefab, container);
        RoomInventoryUI.InitAnsver ansver = UiElement.Init(MouseUIClick);
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

    public void MouseUIClick(RoomInventoryUI ui_tile)
    {
        mouseUIClicked = true;
        uiTileClicked = ui_tile;
    }
}

