using System.Collections.Generic;
using Common;
using Level.GlobalTime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Buff", menuName = "Employee/Buff", order = 1)]
    public class Buff : ScriptableObject
    {
        [SerializeField]
        [InlineProperty]
        private InternalUid uid;
        public InternalUid Uid => uid;

        public Days Time;

        [SerializeReference]
        private List<IEffect> effects = new();
        public IEnumerable<IEffect> Effects => effects;
    }
}
