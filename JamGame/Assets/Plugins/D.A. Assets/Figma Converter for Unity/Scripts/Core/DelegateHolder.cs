using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class DelegateHolder : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public ShowDifferenceChecker ShowDifferenceChecker { get; set; }
        public GetGameViewSize GetGameViewSize { get; set; }
        public Func<Vector2, bool> SetGameViewSize { get; set; }
    }

    public delegate void ShowDifferenceChecker(PreImportInput data, Action<PreImportOutput> callback);
    public delegate bool GetGameViewSize(out Vector2 size);
}