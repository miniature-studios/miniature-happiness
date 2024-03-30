using DA_Assets.Shared;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class DebugSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] bool logDefault = true;
        public bool LogDefault { get => logDefault; set => SetValue(ref logDefault, value); }

        [SerializeField] bool logSetTag = true;
        public bool LogSetTag { get => logSetTag; set => SetValue(ref logSetTag, value); }

        [SerializeField] bool logIsDownloadable = true;
        public bool LogIsDownloadable { get => logIsDownloadable; set => SetValue(ref logIsDownloadable, value); }

        [SerializeField] bool logTransform = true;
        public bool LogTransform { get => logTransform; set => SetValue(ref logTransform, value); }

        [SerializeField] bool logGameObjectDrawer = true;
        public bool LogGameObjectDrawer { get => logGameObjectDrawer; set => SetValue(ref logGameObjectDrawer, value); }

        [SerializeField] bool debugMode = false;
        public bool DebugMode { get => debugMode; set => SetValue(ref debugMode, value); }
    }
}