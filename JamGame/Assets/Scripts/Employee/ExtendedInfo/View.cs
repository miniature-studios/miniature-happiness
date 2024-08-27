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
        private EmployeeImpl employee;

        [Required]
        [SerializeField]
        [ChildGameObjectsOnly]
        private TMP_Text name_text;

        [Required]
        [SerializeField]
        [Pickle(typeof(QuirkView), LookupType = ObjectProviderType.Assets)]
        private QuirkView quirkViewTemplate;

        [Required]
        [SerializeField]
        [Pickle(typeof(BuffView), LookupType = ObjectProviderType.Assets)]
        private BuffView buffViewTemplate;

        [Required]
        [SerializeField]
        private BuffView normalBuff;

        [Required]
        [SerializeField]
        private Transform quirksContainer;

        [SerializeField]
        [Required]
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
            employee.OnPersonalityInitialized += UpdateQuirks;
        }

        private void OnDisable()
        {
            employee.AppliedBuffsChanged -= OnBuffsChanged;
            employee.OnPersonalityInitialized -= UpdateQuirks;
        }

        private void Awake()
        {
            cam = Camera.main;
        }

        public void UpdateQuirks()
        {
            foreach (Quirk quirk in employee.Personality.Quirks)
            {
                AddQuirk(quirk);
            }
            if (employee.Personality.Quirks.Count() == 0)
            {
                quirksContainer.gameObject.SetActive(false);
            }
        }

        private void AddQuirk(Quirk quirk)
        {
            QuirkView quirkView = Instantiate(quirkViewTemplate, quirksContainer);
            quirkView.InitQuirkGraphic(quirk);
        }

        private void Update()
        {
            Quaternion rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
            transform.rotation = rotation;

            name_text.text = $"{employee.Personality.Profession} - {employee.Personality.Name}";
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
                    while (appliedBuffs.Count > 0)
                    {
                        RemoveBuff(appliedBuffs.Last().BuffId);
                    }
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
    }
}
