﻿using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.View")]
    public class View : MonoBehaviour
    {
        [RequiredIn(PrefabKind.Variant | PrefabKind.InstanceInScene)]
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel CoreModelPrefab;
        public InternalUid Uid => CoreModelPrefab.Uid;

        [Required]
        [SerializeField]
        private TMP_Text costLabel;

        [Required]
        [SerializeField]
        private TMP_Text countLabel;

        private Controller shopController;

        [ReadOnly]
        [SerializeField]
        private List<CoreModel> coreModels = new();
        public bool IsEmpty => coreModels.Count == 0;

        public void AddCoreModel(CoreModel coreModel)
        {
            coreModels.Add(coreModel);
            coreModel.transform.SetParent(transform);
        }

        public void RemoveCoreModel(CoreModel coreModel)
        {
            _ = coreModels.Remove(coreModel);
        }

        private void Awake()
        {
            shopController = GetComponentInParent<Controller>();
        }

        private void Update()
        {
            costLabel.text = $"Money cost: {CoreModelPrefab.ShopModel.Cost.Value}";
            countLabel.text = coreModels.Count.ToString();
        }

        // Called by pressing button.
        public void TryBuyRoom()
        {
            shopController.TryBuyRoom(coreModels.Last());
        }
    }
}
