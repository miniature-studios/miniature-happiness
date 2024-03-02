using System.Collections.Generic;
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

        private PersonalityImpl personality;
        private EmployeeImpl employee;

        [Required]
        [SerializeField]
        [ChildGameObjectsOnly]
        private TMP_Text text;

        [SerializeField]
        private AssetLabelReference quirkViewsLabel;

        [SerializeField]
        private AssetLabelReference buffViewsLabel;

        private static Dictionary<string, IResourceLocation> quirkModelViewMap = new();
        private static Dictionary<string, IResourceLocation> buffModelViewMap = new();

        [SerializeField]
        [Required]
        private Transform quirksContainer;

        [SerializeField]
        [Required]
        private Transform buffsContainer;

        private void Start()
        {
            cam = Camera.main;

            personality = GetComponentInParent<PersonalityImpl>();
            employee = GetComponentInParent<EmployeeImpl>();

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

        private bool debugFirstTime = true;

        private void Update()
        {
            transform.LookAt(cam.transform.position);

            text.text = $"{personality.Name}";

            if (debugFirstTime)
            {
                debugFirstTime = false;

                foreach (Quirk quirk in personality.Quirks)
                {
                    IResourceLocation view_location = quirkModelViewMap[quirk.Uid];

                    _ = Instantiate(
                        AddressableTools<QuirkView>.LoadAsset(view_location),
                        quirksContainer
                    );
                }

                foreach (Buff buff in employee.Buffs)
                {
                    IResourceLocation view_location = buffModelViewMap[buff.Uid];

                    _ = Instantiate(
                        AddressableTools<BuffView>.LoadAsset(view_location),
                        buffsContainer
                    );
                }
            }
        }
    }
}
