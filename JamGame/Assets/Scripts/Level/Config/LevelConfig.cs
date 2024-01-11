using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/LevelConfig", order = 0)]
    public class LevelConfig : SerializedScriptableObject
    {
        [OdinSerialize]
        [FoldoutGroup("All Playable Days")]
        public IEnumerable<DayConfig> Days { get; private set; } = new List<DayConfig>();

        [HideLabel]
        [OdinSerialize]
        [InlineProperty]
        [FoldoutGroup("Default Day")]
        public DayConfig DefaultDay { get; private set; } = new();

        [OdinSerialize]
        [FoldoutGroup("General Information")]
        public float BossStressSpeed { get; private set; }
    }
}
