using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level/Inventory/Room/Level.Inventory.Room.View")]
    public class View : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [RequiredIn(PrefabKind.Variant | PrefabKind.InstanceInScene)]
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel CoreModelPrefab;

        public InternalUid Uid => CoreModelPrefab.Uid;

        [Required]
        [SerializeField]
        private ExtendedView extendedView;

        [Required]
        [SerializeField]
        private TMP_Text countLabel;

        [ReadOnly]
        [SerializeField]
        private List<CoreModel> coreModels = new();
        public bool IsEmpty => coreModels.Count == 0;
        public CoreModel CoreModel => coreModels.Last();

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
        }

        public void RemoveCoreModel(CoreModel coreModel)
        {
            _ = coreModels.Remove(coreModel);
        }

        public void Update()
        {
            countLabel.text = coreModels.Count.ToString();
            if (inputActions.UI.ExtendInventoryTileInfo.IsPressed() && isHovered)
            {
                if (!extendedView.IsVisible)
                {
                    extendedView.Show();
                    extendedView.SetLabelText($"Rent: {CoreModel.RentCost.Value}");
                }
            }
            else if (extendedView.IsVisible)
            {
                extendedView.Hide();
            }
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
