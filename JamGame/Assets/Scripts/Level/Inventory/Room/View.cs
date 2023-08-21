using Common;
using Level.Room;
using Pickle;
using System;
using TMPro;
using UnityEngine;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level.Inventory.Room.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        [Pickle(LookupType = ObjectProviderType.Assets)]
        private CoreModel coreModel;

        public CoreModel CoreModel => coreModel;

        [SerializeField]
        private TMP_Text counterText;

        [SerializeField]
        private GameObject extendedUIInfoPrefab;

        private RectTransform targetInfo = null;
        private Canvas canvas;

        private Func<Cost> getCost = null;
        private Func<TariffProperties> getTariff = null;

        private Func<int> getCount = null;
        public Func<int> GetCount => getCount;

        private Func<CoreModel> getCoreModelInstance;
        public Func<CoreModel> GetCoreModelInstance => getCoreModelInstance;

        private bool pointerOver;
        public bool PointerOver => pointerOver;

        public void Awake()
        {
            canvas = FindObjectOfType<Canvas>();
        }

        public void Constructor(
            Func<Cost> getCost,
            Func<TariffProperties> getTariff,
            Func<int> getCount,
            Func<CoreModel> getCoreModelInstance
        )
        {
            this.getCost = getCost;
            this.getTariff = getTariff;
            this.getCount = getCount;
            this.getCoreModelInstance = getCoreModelInstance;
        }

        public void Update()
        {
            counterText.text = Convert.ToString(getCount());
            pointerOver = RayCastUtilities.PointerIsOverTargetGO(Input.mousePosition, gameObject);
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                switch (pointerOver, targetInfo == null)
                {
                    case (true, true):
                        targetInfo = Instantiate(
                                extendedUIInfoPrefab,
                                Input.mousePosition + new Vector3(20, 20, 0),
                                new Quaternion(),
                                canvas.transform
                            )
                            .GetComponent<RectTransform>();
                        targetInfo.GetComponentInChildren<TMP_Text>().text =
                            $"Electricity. Con.: {getTariff().ElectricityConsumption}\n"
                            + $"Water Con.: {getTariff().WaterConsumption}\n"
                            + $"Cost: {getCost().Value}";
                        break;
                    case (true, false):
                        targetInfo.position = Input.mousePosition + new Vector3(20, 20, 0);
                        break;
                    case (false, false):
                        Destroy(targetInfo.gameObject);
                        break;
                }
            }
        }
    }
}
