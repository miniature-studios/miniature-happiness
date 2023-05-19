using Common;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private TileBuilderController tileBuilderController;
    [SerializeField] private FinancesController financesController;
    [SerializeField] private Transform roomsUIContainer;
    [SerializeField] private Transform employeeUIContainer;
    [SerializeField] private CustomButton buttonShopOpen;

    public Transform ShopPanel;

    public UIHider ButtonShopOpenUIHider => buttonShopOpen.UIHider;

    public void CloseShop()
    {
        SetShopState(UIElementState.Hidden);
    }
    public void OpenShop()
    {
        SetShopState(UIElementState.Shown);
    }

    public void SetShopState(UIElementState ui_element_state)
    {
        switch (ui_element_state)
        {
            case UIElementState.Shown:
                ShopPanel.gameObject.SetActive(true);
                break;
            default:
            case UIElementState.Hidden:
                ShopPanel.gameObject.SetActive(false);
                break;
        }
    }

    public void SetShopRooms(IEnumerable<RoomConfig> room_configs)
    {
        foreach (GameObject child in roomsUIContainer.transform)
        {
            Destroy(child);
        }
        foreach (RoomConfig config in room_configs)
        {
            RoomShopUI shop_ui = Instantiate(config.RoomShopUI, roomsUIContainer);
            shop_ui.Init(TryBuyRoom);
        }
    }

    public bool TryBuyRoom(int cost, TileUI tile_ui)
    {
        if (financesController.MoneyCount - cost > 0)
        {
            financesController.TakeMoney(cost);
            _ = tileBuilderController.TilesPanelController.CreateUIElement(tile_ui);
            return true;
        }
        else
        {
            Debug.Log("Not enough money");
            return false;
        }
    }

    public void SetShopEmployees(IEnumerable<EmployeeConfig> employee_configs)
    {

    }
}

