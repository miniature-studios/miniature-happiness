using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Common;
using Employee.Personality;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.View")]
    public class View : MonoBehaviour
    {
        private Camera cam;

        [SerializeField]
        [Required]
        private EmployeeImpl employee;

        [Required]
        [SerializeField]
        [ChildGameObjectsOnly]
        private TMP_Text name_text;

        [SerializeField]
        private AssetLabelReference quirkViewsLabel;

        [SerializeField]
        private AssetLabelReference buffViewsLabel;

        private static Dictionary<InternalUid, QuirkView> quirkModelViewMap = new();
        private static Dictionary<InternalUid, BuffView> buffModelViewMap = new();

        [SerializeField]
        [Required]
        private Transform quirksContainer;

        [SerializeField]
        [Required]
        private Transform buffsContainer;

        private List<BuffView> instantiatedBuffViews = new();

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

            InitModelViewMaps();

            foreach (Buff buff in employee.AppliedBuffs)
            {
                AddBuff(buff);
            }

            foreach (Quirk quirk in employee.Personality.Quirks)
            {
                AddQuirk(quirk);
            }
        }

        private void InitModelViewMaps()
        {
            if (quirkModelViewMap.Count == 0)
            {
                quirkModelViewMap = AddressableTools.LoadAllGameObjectAssets<QuirkView>(
                    quirkViewsLabel
                );
            }

            if (buffModelViewMap.Count == 0)
            {
                buffModelViewMap = AddressableTools.LoadAllGameObjectAssets<BuffView>(
                    buffViewsLabel
                );
            }
        }

        private void Update()
        {
            Quaternion rotation = Quaternion.LookRotation(-cam.transform.forward, cam.transform.up);
            transform.rotation = rotation;

            name_text.text = $"{employee.Personality.Name}";
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
                    while (instantiatedBuffViews.Count > 0)
                    {
                        BuffView last = instantiatedBuffViews.Last();
                        _ = instantiatedBuffViews.Remove(last);
                        Destroy(last.gameObject);
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
            BuffView buff_view = Instantiate(buffModelViewMap[buff.Uid], buffsContainer);
            instantiatedBuffViews.Add(buff_view);
        }

        private void RemoveBuff(InternalUid buff_uid)
        {
            BuffView to_remove = instantiatedBuffViews.Find((b) => b.Uid == buff_uid);
            _ = instantiatedBuffViews.Remove(to_remove);
            Destroy(to_remove.gameObject);
        }

        private void AddQuirk(Quirk quirk)
        {
            _ = Instantiate(quirkModelViewMap[quirk.Uid], quirksContainer);
        }
    }
}
