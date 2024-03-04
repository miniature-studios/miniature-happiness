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
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.View")]
    public class View : MonoBehaviour
    {
        private Camera cam;

        [SerializeField]
        [Required]
        private PersonalityImpl personality;

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

        private static Dictionary<InternalUid, IResourceLocation> quirkModelViewMap = new();
        private static Dictionary<InternalUid, IResourceLocation> buffModelViewMap = new();

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

            foreach (Quirk quirk in personality.Quirks)
            {
                AddQuirk(quirk);
            }
        }

        private void InitModelViewMaps()
        {
            if (quirkModelViewMap.Count == 0)
            {
                foreach (
                    AssetWithLocation<QuirkView> quirk_view in AddressableTools<QuirkView>.LoadAllFromLabel(
                        quirkViewsLabel
                    )
                )
                {
                    quirkModelViewMap.Add(quirk_view.Asset.Uid, quirk_view.Location);
                }
            }

            if (buffModelViewMap.Count == 0)
            {
                foreach (
                    AssetWithLocation<BuffView> buff_view in AddressableTools<BuffView>.LoadAllFromLabel(
                        buffViewsLabel
                    )
                )
                {
                    buffModelViewMap.Add(buff_view.Asset.Uid, buff_view.Location);
                }
            }
        }

        private void Update()
        {
            transform.LookAt(cam.transform.position);
            name_text.text = $"{personality.Name}";
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
            IResourceLocation view_location = buffModelViewMap[buff.Uid];
            BuffView buff_view = Instantiate(
                AddressableTools<BuffView>.LoadAsset(view_location),
                buffsContainer
            );
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
            IResourceLocation view_location = quirkModelViewMap[quirk.Uid];
            _ = Instantiate(AddressableTools<QuirkView>.LoadAsset(view_location), quirksContainer);
        }
    }
}
