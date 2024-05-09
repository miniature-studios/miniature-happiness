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

        public InternalUid Uid => coreModels.FirstOrDefault()?.Uid;

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
            if (!IsEmpty && coreModel.Uid != Uid)
            {
                Debug.LogError(
                    "Trying to add room with wrong Uid. Expected: "
                        + Uid
                        + ", got: "
                        + coreModel.Uid
                );
                return;
            }

            coreModels.Add(coreModel);
            coreModel.transform.SetParent(transform);

            UpdateData();
        }

        public void RemoveCoreModel(CoreModel coreModel)
        {
            _ = coreModels.Remove(coreModel);
            if (!IsEmpty)
            {
                UpdateData();
            }
        }

        private void UpdateData()
        {
            miniature.sprite = coreModels.First().InventoryModel.Miniature;
            countLabel.text = coreModels.Count.ToString();
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
