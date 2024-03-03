using Pickle;
using UnityEngine;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.BuffView")]
    internal class BuffView : MonoBehaviour
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public Buff BuffModelPrefab;

        public string Uid => BuffModelPrefab.Uid;
    }
}
