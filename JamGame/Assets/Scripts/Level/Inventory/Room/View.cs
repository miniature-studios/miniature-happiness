using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
        private RectTransform canvas;
        private InputActions inputActions;
        private Vector2 extendViewShift = new(20, 20);

        private void Awake()
        {
            inputActions = new();
            canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
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
                    extendedView.transform.SetParent(canvas);
                }
                extendedView.SetLabelText($"Rent: {CoreModel.RentCost.Value}");
                Vector2 pointerPosition = Mouse.current.position.ReadValue();
                extendedView.transform.position = (Vector3)(pointerPosition + extendViewShift);
            }
            else if (extendedView.gameObject.activeSelf)
            {
                extendedView.gameObject.SetActive(false);
                extendedView.transform.SetParent(transform);
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
