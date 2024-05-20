﻿using System;
using System.Collections.Generic;
using Common;
using Employee.Personality;
using Level.Config;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Level.Shop.Employee
{
    [AddComponentMenu("Scripts/Level/Shop.Employee/Level.Shop.Employee.CardView")]
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Required]
        [SerializeField]
        private TMP_Text nameLabel;

        [Required]
        [SerializeField]
        private TMP_Text hireCostLabel;

        [Required]
        [SerializeField]
        private TMP_Text professionLabel;

        [Required]
        [SerializeField]
        private AssetLabelReference quirkLabel;

        [Required]
        [AssetsOnly]
        [SerializeField]
        private CardQuirkIcon quirkIconPrefab;

        [Required]
        [SerializeField]
        private Transform iconsParent;

        [Required]
        [SerializeField]
        private Image coinIcon;

        [Required]
        [SerializeField]
        private Sprite hiredIconSprite;

        [Required]
        [SerializeField]
        private Button hireButton;

        [ReadOnly]
        [SerializeField]
        private bool hired = false;

        [ReadOnly]
        [SerializeField]
        private EmployeeConfig employeeConfig;
        public EmployeeConfig EmployeeConfig => employeeConfig;

        private Controller controller;
        private Dictionary<InternalUid, Quirk> quirksByUid;
        public Dictionary<InternalUid, Quirk> QuirksByUid => quirksByUid;

        public event Action OnPointerEnterEvent;
        public event Action OnPointerExitEvent;

        public void Initialize()
        {
            controller = GetComponentInParent<Controller>(true);
            quirksByUid = AddressableTools.LoadAllScriptableObjectAssets<Quirk>(quirkLabel);
        }

        public void SetEmployeeConfig(EmployeeConfig employeeConfig)
        {
            this.employeeConfig = employeeConfig;
            UpdateData();
        }

        private void UpdateData()
        {
            nameLabel.text = EmployeeConfig.Name;
            hireCostLabel.text = EmployeeConfig.HireCost.ToString();
            professionLabel.text = EmployeeConfig.Profession;

            iconsParent.DestroyChildrenImmediate();
            bool isQuirksExists = EmployeeConfig.Quirks.Count != 0;
            iconsParent.gameObject.SetActive(isQuirksExists);
            if (isQuirksExists)
            {
                foreach (Quirk quirk in EmployeeConfig.Quirks)
                {
                    CardQuirkIcon quirkIcon = Instantiate(quirkIconPrefab, iconsParent);
                    quirkIcon.SetIcon(quirksByUid[quirk.Uid].Icon);
                }
            }
        }

        // Called by `hire` button.
        public void TryHireEmployee()
        {
            if (hired)
            {
                return;
            }

            if (controller.TryBuyEmployee(EmployeeConfig).Success)
            {
                coinIcon.sprite = hiredIconSprite;
                hireCostLabel.gameObject.SetActive(false);
                hireButton.interactable = false;
                hired = true;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke();
        }
    }
}
