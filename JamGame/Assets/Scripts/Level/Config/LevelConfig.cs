﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField]
        private List<DayConfig> days;

        [SerializeField]
        private DayConfig defaultDay;
        public ImmutableList<DayConfig> Days => days.ToImmutableList();
        public DayConfig DefaultDay => defaultDay;

        [SerializeField]
        private float bossStressSpeed;

        [SerializeField]
        private Tariffs tariffs;
        public float BossStressSpeed => bossStressSpeed;
        public Tariffs Tariffs => tariffs;
    }
}
