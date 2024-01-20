using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Level.Config.DayAction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AnimatorsSwitcher
{
    public enum OverrideState
    {
        DoNotOverride,
        Hidden,
        Showed
    }

    [Serializable]
    public class AnimatorProperties
    {
        [HideInInspector]
        public string AnimatorName;

        [LabelText("@AnimatorName")]
        [HorizontalGroup()]
        public bool Showed;

        [HideLabel]
        [HorizontalGroup(Width = 200, LabelWidth = 0, DisableAutomaticLabelWidth = true)]
        public OverrideState OverrideState;
    }

    [Serializable]
    public class InterfaceMatch
    {
        [HideInInspector]
        public string InterfaceName;

        [LabelText("@" + nameof(InterfaceName))]
        [ListDrawerSettings(
            DraggableItems = false,
            HideRemoveButton = true,
            HideAddButton = true,
            IsReadOnly = true
        )]
        public List<AnimatorProperties> AnimatorsProperties;
    }

    [Serializable]
    public class AnimatorList
    {
        [ListDrawerSettings(
            OnTitleBarGUI = nameof(TitleBarGUIAnimators),
            CustomRemoveElementFunction = nameof(CustomRemoveElementFunction),
            CustomRemoveIndexFunction = nameof(CustomRemoveIndexFunction)
        )]
        public List<Animator> Animators = new();

        [ListDrawerSettings(
            DraggableItems = false,
            HideRemoveButton = true,
            HideAddButton = true,
            OnTitleBarGUI = nameof(TitleBarGUIInterfaceMatcher)
        )]
        public List<InterfaceMatch> InterfaceMatcher = new();

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
                    _ = match.AnimatorsProperties.RemoveAll(x => x.AnimatorName == animator.name);
                }
            }
            Animators.RemoveAt(index);
        }

        private void TitleBarGUIAnimators()
        {
            if (InterfaceMatcher.Count() != ActionNames.Value.Count)
            {
                List<string> cache = InterfaceMatcher.Select(x => x.InterfaceName).ToList();
                List<string> names = new();
                foreach (string interfaceName in ActionNames.Value)
                {
                    if (!cache.Contains(interfaceName))
                    {
                        InterfaceMatcher.Add(
                            new InterfaceMatch
                            {
                                InterfaceName = interfaceName,
                                AnimatorsProperties = new()
                            }
                        );
                    }
                    names.Add(interfaceName);
                }
                _ = InterfaceMatcher.RemoveAll(x => !names.Contains(x.InterfaceName));
            }
        }

        private void TitleBarGUIInterfaceMatcher()
        {
            foreach (Animator animator in Animators)
            {
                if (animator == null)
                {
                    continue;
                }
                foreach (InterfaceMatch match in InterfaceMatcher)
                {
                    if (!match.AnimatorsProperties.Any(x => x.AnimatorName == animator.name))
                    {
                        match.AnimatorsProperties.Add(new() { AnimatorName = animator.name });
                    }
                }
            }
        }

        private static List<string> GetActionNames()
        {
            Type[] types = Assembly.GetAssembly(typeof(IDayAction)).GetTypes();
            return types
                .Where(type => typeof(IDayAction).IsAssignableFrom(type) && !type.IsInterface)
                .Select(t => t.Name)
                .ToList();
        }
    }

    [AddComponentMenu("Scripts/AnimatorsSwitcher/AnimatorsSwitcher")]
    public class AnimatorsSwitcherImpl : MonoBehaviour
    {
        [SerializeField]
        [InlineProperty]
        [HideLabel]
        private AnimatorList animatorList;

        public void SetAnimatorStates(Type dayType)
        {
            InterfaceMatch interfaceMatch = animatorList.InterfaceMatcher.Find(x =>
                x.InterfaceName == dayType.Name
            );
            foreach (AnimatorProperties properties in interfaceMatch.AnimatorsProperties)
            {
                Animator animator = animatorList.Animators.Find(x =>
                    x.name == properties.AnimatorName
                );
                animator.SetBool("Showed", properties.Showed);
                switch (properties.OverrideState)
                {
                    case OverrideState.DoNotOverride:
                        continue;
                    case OverrideState.Hidden:
                        animator.Play("Hidden");
                        break;
                    case OverrideState.Showed:
                        animator.Play("Showed");
                        break;
                }
            }
        }
    }
}
