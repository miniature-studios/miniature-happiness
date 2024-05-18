using Common;
using Employee.Personality;
using Pickle;
using UnityEngine;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.QuirkView")]
    internal class QuirkView : MonoBehaviour, IUidHandle
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public Quirk QuirkModelPrefab;

        public InternalUid Uid => QuirkModelPrefab.Uid;
    }
}
