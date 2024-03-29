﻿using Common;
using Pickle;
using UnityEngine;

namespace Employee.ExtendedInfo
{
    [AddComponentMenu("Scripts/Scripts/Employee/ExtendedInfo/Employee.ExtendedInfo.QuirkView")]
    internal class QuirkView : MonoBehaviour
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public Quirk QuirkModelPrefab;

        public InternalUid Uid => QuirkModelPrefab.Uid;
    }
}
