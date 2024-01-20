using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Level.Config;
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
        public Animator Animator;
        private string AnimatorName => Animator == null ? "NULL" : Animator.name;

        [LabelText("@" + nameof(AnimatorName))]
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
                properties.Animator.SetBool("Showed", properties.Showed);
                switch (properties.OverrideState)
                {
                    case OverrideState.DoNotOverride:
                        continue;
                    case OverrideState.Hidden:
                        properties.Animator.Play("Hidden");
                        break;
                    case OverrideState.Showed:
                        properties.Animator.Play("Showed");
                        break;
                }
            }
        }
    }
}
