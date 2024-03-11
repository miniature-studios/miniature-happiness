using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Level.Config;
using UnityEngine;

namespace AnimatorsSwitcher
{
    public partial class AnimatorList
    {
        private Lazy<List<string>> ActionNames { get; set; } = new(GetActionNames());

        private void CustomRemoveElementFunction(Animator animator)
        {
            CustomRemoveIndexFunction(Animators.IndexOf(animator));
        }

        private void CustomRemoveIndexFunction(int index)
        {
            Animator animator = Animators[index];
            if (animator != null)
            {
                foreach (InterfaceMatch match in InterfaceMatcher)
                {
                    _ = match.AnimatorsProperties.RemoveAll(x => x.Animator == animator);
                }
            }
            Animators.RemoveAt(index);
        }

        private void TitleBarGUIAnimators()
        {
            for (int i = 0; i < InterfaceMatcher.Count; i++)
            {
                if (InterfaceMatcher[i] == null || InterfaceMatcher[i].InterfaceName == null)
                {
                    InterfaceMatcher.RemoveAt(i);
                    i--;
                }
            }
            if (InterfaceMatcher.Count != ActionNames.Value.Count)
            {
                List<string> cache = InterfaceMatcher.Select(x => x.InterfaceName).ToList();
                List<string> names = new();
                foreach (string interfaceNames in ActionNames.Value)
                {
                    if (!cache.Contains(interfaceNames))
                    {
                        InterfaceMatcher.Add(
                            new InterfaceMatch
                            {
                                InterfaceName = interfaceNames,
                                AnimatorsProperties = new()
                            }
                        );
                    }
                    names.Add(interfaceNames);
                }
                _ = InterfaceMatcher.RemoveAll(x => !names.Contains(x.InterfaceName));
            }
        }

        private void TitleBarGUIInterfaceMatcher()
        {
            foreach (InterfaceMatch match in InterfaceMatcher)
            {
                match.AnimatorsProperties = match
                    .AnimatorsProperties.Where(x => x.Animator != null)
                    .ToList();
            }
            foreach (Animator animator in Animators)
            {
                if (animator == null)
                {
                    continue;
                }
                foreach (InterfaceMatch match in InterfaceMatcher)
                {
                    if (!match.AnimatorsProperties.Any(x => x.Animator == animator))
                    {
                        match.AnimatorsProperties.Add(new() { Animator = animator });
                    }
                }
            }
        }

        private static List<string> GetActionNames()
        {
            Type[] types = Assembly.GetAssembly(typeof(IDayAction)).GetTypes();
            return types
                .Where(type => typeof(IDayAction).IsAssignableFrom(type) && !type.IsInterface)
                .Select(x => x.Name)
                .ToList();
        }
    }
}
