﻿using System.Collections.Generic;
using UnityEngine;
using Employee;

namespace Location
{
    [AddComponentMenu("Scripts/Location.NeedProviderManager")]
    public class NeedProviderManager : MonoBehaviour
    {
        private List<NeedProvider> needProviders;

        // TODO: Call it each time room added/removed.
        public void InitGameMode()
        {
            needProviders = new List<NeedProvider>(
                transform.GetComponentsInChildren<NeedProvider>()
            );
        }

        public IEnumerable<NeedProvider> FindAllAvailableProviders(
            EmployeeImpl employee,
            NeedType need_type
        )
        {
            foreach (NeedProvider provider in needProviders)
            {
                if (provider.NeedType == need_type && provider.IsAvailable(employee))
                {
                    yield return provider;
                }
            }
        }
    }
}