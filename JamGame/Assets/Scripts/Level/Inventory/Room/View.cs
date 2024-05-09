using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level/Inventory/Room/Level.Inventory.Room.View")]
    public class View : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IUidHandle
    {
        [Required]
        [SerializeField]
        private TMP_Text countLabel;

        [Required]
        [SerializeField]
        private Image miniature;

        [ReadOnly]
        [SerializeField]
        private List<CoreModel> coreModels = new();

        public InternalUid Uid => coreModels.Last().Uid;

        public bool IsEmpty => coreModels.Count == 0;

        private bool isHovered = false;
        private InputActions inputActions;

        private void Awake()
        {
            inputActions = new();
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void AddCoreModel(CoreModel coreModel)
        {
            coreModels.Add(coreModel);
            coreModel.transform.SetParent(transform);

            UpdateData();
        }

        private void UpdateData()
        {
            miniature.sprite = coreModels.First().InventoryModel.Miniature;
            countLabel.text = coreModels.Count.ToString();
        }

        public void RemoveCoreModel(CoreModel coreModel)
        {
            _ = coreModels.Remove(coreModel);
        }

        public void Update()
        {
            //countLabel.text = coreModels.Count.ToString();
            //if (inputActions.UI.ExtendInventoryTileInfo.IsPressed() && isHovered)
            //{
            //    //if (!extendedView.IsVisible)
            //    //{
            //    //    extendedView.Show();
            //    //    extendedView.SetLabelText($"Rent: {CoreModelPrefab.RoomInfo.RentCost.Value}");
            //    //}
            //}
            //else if (extendedView.IsVisible)
            //{
            //    extendedView.Hide();
            //}
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }
    }
}
