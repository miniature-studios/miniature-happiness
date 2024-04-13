using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using System;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [Serializable]
    public class FcuEvents : MonoBehaviourBinder<FigmaConverterUnity>
    {
        /// <summary>
        /// Called when the project's json downloading fails. Fires once per import.
        /// </summary>
        [SerializeField] UnityEvent<FigmaConverterUnity> onProjectDownloadFail;
        [SerializeProperty(nameof(onProjectDownloadFail))]
        public UnityEvent<FigmaConverterUnity> OnProjectDownloadFail => onProjectDownloadFail;

        /// <summary>
        /// Called when the project's json downloading is started. Fires once per import.
        /// </summary>
        [SerializeField] UnityEvent<FigmaConverterUnity> onProjectDownloadStart;
        [SerializeProperty(nameof(onProjectDownloadStart))]
        public UnityEvent<FigmaConverterUnity> OnProjectDownloadStart => onProjectDownloadStart;

        /// <summary>
        /// Called when the project's json file has downloaded. Fires once per import.
        /// </summary>
        [SerializeField] UnityEvent<FigmaConverterUnity> onProjectDownloaded;
        [SerializeProperty(nameof(onProjectDownloaded))]
        public UnityEvent<FigmaConverterUnity> OnProjectDownloaded => onProjectDownloaded;

        /// <summary>
        /// Called when import starts. Fires once per import.
        /// </summary>
        [SerializeField] UnityEvent<FigmaConverterUnity> onImportStart;
        [SerializeProperty(nameof(onImportStart))]
        public UnityEvent<FigmaConverterUnity> OnImportStart => onImportStart;

        /// <summary>
        /// Called after import is successfully complete. Fires once per import.
        /// </summary>
        [SerializeField] UnityEvent<FigmaConverterUnity> onImportComplete;
        [SerializeProperty(nameof(onImportComplete))]
        public UnityEvent<FigmaConverterUnity> OnImportComplete => onImportComplete;

        /// <summary>
        /// Called when import stops due to an error. Fires once per import.
        /// </summary>
        [SerializeField] UnityEvent<FigmaConverterUnity> onImportFail;
        [SerializeProperty(nameof(onImportFail))]
        public UnityEvent<FigmaConverterUnity> OnImportFail => onImportFail;

        /// <summary>
        /// Called when a fobject's GameObject is created on the scene. Called once per GameObject.
        /// </summary>
        [SerializeField] UnityEvent<FigmaConverterUnity, GameObject> onObjectInstantiate;
        [SerializeProperty(nameof(onObjectInstantiate))]
        public UnityEvent<FigmaConverterUnity, GameObject> OnObjectInstantiate => onObjectInstantiate;

        /// <summary>
        /// Called when a component is added to a GameObject based on tag. Called multiple times per GameObject.
        /// </summary>
        [SerializeField] UnityEvent<FigmaConverterUnity, SyncData, FcuTag> onAddComponent;
        [SerializeProperty(nameof(onAddComponent))]
        public UnityEvent<FigmaConverterUnity, SyncData, FcuTag> OnAddComponent => onAddComponent;
    }
}