﻿using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeReference]
        [FoldoutGroup("All Playable Days")]
        private List<DayConfig> days = new();
        public IEnumerable<DayConfig> Days => days;

        [HideLabel]
        [SerializeField]
        [InlineProperty]
        [FoldoutGroup("Default Day")]
        private DayConfig defaultDay = new();
        public DayConfig DefaultDay => defaultDay;
    }
}
