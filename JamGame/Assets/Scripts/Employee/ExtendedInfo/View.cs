using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Common;
using Employee.Personality;
using Pickle;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.View")]
    public class View : MonoBehaviour
    {
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        [SerializeField]
        private PersonalityImpl personality;

        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        [SerializeField]
        private EmployeeImpl employee;

        [Required]
        [SerializeField]
        [ChildGameObjectsOnly]
        private TMP_Text titleLabel;

        [Required]
        [Pickle(typeof(QuirkView), LookupType = ObjectProviderType.Assets)]
        [SerializeField]
        private QuirkView quirkViewTemplate;

        [Required]
        [Pickle(typeof(BuffView), LookupType = ObjectProviderType.Assets)]
        [SerializeField]
        private BuffView buffViewTemplate;

        [Required]
        [SerializeField]
        private BuffView normalBuff;

        [Required]
        [SerializeField]
        private Transform quirksContainer;

        [Required]
        [SerializeField]
        private Transform buffsContainer;

        [Serializable]
        private struct BuffViewPair
        {
            public InternalUid BuffId;
            public BuffView BuffView;
        }

        [ReadOnly]
        [SerializeField]
        private List<BuffViewPair> appliedBuffs;

        private Camera cam;

        private void OnEnable()
        {
            employee.AppliedBuffsChanged += OnBuffsChanged;
        }

        private void OnDisable()
        {
            employee.AppliedBuffsChanged -= OnBuffsChanged;
        }

        private void Awake()
        {
            cam = Camera.main;
        }

        public void UpdateQuirks()
        {
            foreach (Quirk quirk in personality.Quirks)
            {
                AddQuirk(quirk);
            }
            if (personality.Quirks.Count() == 0)
            {
                quirksContainer.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            Quaternion rotation = Quaternion.LookRotation(-cam.transform.forward, cam.transform.up);
            transform.rotation = rotation;

            titleLabel.text = $"{personality.Profession} - {personality.Name}";
        }

        public void OnBuffsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    Buff added_buff = e.NewItems[0] as Buff;
                    AddBuff(added_buff);
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    Buff removed_buff = e.OldItems[0] as Buff;
                    RemoveBuff(removed_buff.Uid);
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    Buff added_buff = e.NewItems[0] as Buff;
                    AddBuff(added_buff);
                    Buff removed_buff = e.OldItems[0] as Buff;
                    RemoveBuff(removed_buff.Uid);
                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    break;
                }
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    throw new ArgumentException();
            }
        }

        private void AddBuff(Buff buff)
        {
            BuffView buffView = Instantiate(buffViewTemplate, buffsContainer);
            buffView.InitBuffGraphic(buff);
            appliedBuffs.Add(new BuffViewPair() { BuffId = buff.Uid, BuffView = buffView });
            normalBuff.gameObject.SetActive(false);
        }

        private void RemoveBuff(InternalUid buff_uid)
        {
            BuffViewPair toRemove = appliedBuffs.Find(x => x.BuffId == buff_uid);
            Destroy(toRemove.BuffView.gameObject);
            _ = appliedBuffs.Remove(toRemove);
            if (appliedBuffs.Count == 0)
            {
                normalBuff.gameObject.SetActive(true);
            }
        }

        private void AddQuirk(Quirk quirk)
        {
            QuirkView quirkView = Instantiate(quirkViewTemplate, quirksContainer);
            quirkView.InitQuirkGraphic(quirk);
        }
    }
}
