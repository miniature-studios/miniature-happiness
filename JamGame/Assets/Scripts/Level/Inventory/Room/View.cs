using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level/Inventory/Room/Level.Inventory.Room.View")]
    public class View : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel CoreModelPrefab;

        public string Uid => CoreModelPrefab.Uid;

        [SerializeField]
        private ExtendedView extendedView;

        [ReadOnly]
        [SerializeField]
        private CoreModel coreModel;
        public CoreModel CoreModel => coreModel;

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

        public void SetCoreModel(CoreModel coreModel)
        {
            this.coreModel = coreModel;
        }

        public void Update()
        {
            if (inputActions.UI.ExtendInventoryTileInfo.IsPressed() && isHovered)
            {
                if (!extendedView.gameObject.activeSelf)
                {
                    extendedView.gameObject.SetActive(true);
                    extendedView.SetLabelText($"Rent: {CoreModel.RentCost.Value}");
                }
            }
            else if (extendedView.gameObject.activeSelf)
            {
                extendedView.gameObject.SetActive(false);
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
