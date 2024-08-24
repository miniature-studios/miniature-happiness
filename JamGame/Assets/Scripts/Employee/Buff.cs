using System.Collections.Generic;
using Common;
using Level.GlobalTime;
using UnityEngine;

namespace Employee
{
    [CreateAssetMenu(fileName = "Buff", menuName = "Employee/Buff")]
    public class Buff : ScriptableObject, IPostprocessedUidHandle
    {
        [SerializeField]
        private InternalUid uid;
        public InternalUid Uid => uid;

        [SerializeField]
        public InGameTime Time;

        [SerializeReference]
        private List<IEffect> effects = new();
        public IEnumerable<IEffect> Effects => effects;
    }
}
