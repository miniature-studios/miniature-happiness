using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Employee.Needs;
using Level.GlobalTime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee
{
    public partial class EmployeeImpl
    {
        [ReadOnly]
        [SerializeField]
        private List<NeedModifiers> registeredModifiers = new();

        [Serializable]
        private struct AppliedBuff
        {
            [ReadOnly]
            public Buff Buff;

            [ReadOnly]
            public RealTimeSeconds RemainingTime;
        }

        [SerializeField]
        [ReadOnly]
        private ObservableCollection<AppliedBuff> appliedBuffs = new();
        public IEnumerable<Buff> AppliedBuffs => appliedBuffs.Select(b => b.Buff);

        public event NotifyCollectionChangedEventHandler AppliedBuffsChanged;

        private BuffsNeedModifiersPool buffsNeedModifiers;

        private void UpdateBuffs(RealTimeSeconds delta_time)
        {
            for (int i = appliedBuffs.Count - 1; i >= 0; i--)
            {
                AppliedBuff ab = appliedBuffs[i];
                ab.RemainingTime -= delta_time;
                if (ab.RemainingTime < RealTimeSeconds.Zero)
                {
                    UnregisterBuff(appliedBuffs[i].Buff);
                }
                else
                {
                    appliedBuffs[i] = ab;
                }
            }
        }

        public void RegisterModifier(NeedModifiers modifiers)
        {
            if (registeredModifiers.Contains(modifiers))
            {
                Debug.LogWarning("Modifiers already registered");
                return;
            }

            registeredModifiers.Add(modifiers);
            foreach (Need need in needs)
            {
                need.RegisterModifier(modifiers);
            }
        }

        public void UnregisterModifier(NeedModifiers modifiers)
        {
            if (!registeredModifiers.Remove(modifiers))
            {
                Debug.LogWarning("Modifiers to unregister not found");
                return;
            }

            foreach (Need need in needs)
            {
                need.UnregisterModifier(modifiers);
            }
        }

        private static NotifyCollectionChangedEventArgs AppliedBuffsCollectionChangedMapping(
            NotifyCollectionChangedEventArgs original_args
        )
        {
            NotifyCollectionChangedEventArgs args;
            switch (original_args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    args = new(original_args.Action, ((AppliedBuff)original_args.NewItems[0]).Buff);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    args = new(original_args.Action, ((AppliedBuff)original_args.OldItems[0]).Buff);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    args = new(
                        original_args.Action,
                        ((AppliedBuff)original_args.NewItems[0]).Buff,
                        ((AppliedBuff)original_args.OldItems[0]).Buff
                    );
                    break;
                case NotifyCollectionChangedAction.Reset:
                    List<Buff> old_items = new();
                    foreach (object old_item in original_args.OldItems)
                    {
                        old_items.Add(((AppliedBuff)old_item).Buff);
                    }
                    args = new(original_args.Action, old_items);
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {original_args.Action}"
                    );
                    throw new ArgumentException();
            }

            return args;
        }

        public void RegisterBuff(Buff buff)
        {
            appliedBuffs.Add(
                new AppliedBuff { Buff = buff, RemainingTime = buff.Time.RealTimeSeconds }
            );
            foreach (IEffect effect in buff.Effects)
            {
                RegisterEffect(effect);
            }
        }

        // TODO: match type of effect with corresponding Executor type.
        public void RegisterEffect(IEffect effect)
        {
            if (effect is StressEffect se)
            {
                Stress.RegisterEffect(se);
            }
            else if (effect is NeedModifierEffect nme)
            {
                buffsNeedModifiers.RegisterEffect(nme);
            }
            else if (effect is ControllerEffect ce)
            {
                controller.RegisterEffect(ce);
            }
            else if (effect is EarnedMoneyEffect eme)
            {
                incomeGenerator.RegisterEffect(eme);
            }
            else
            {
                throw new InvalidOperationException("Unknown effect type");
            }
        }

        public void UnregisterBuff(Buff buff)
        {
            for (int i = 0; i < appliedBuffs.Count; i++)
            {
                if (appliedBuffs[i].Buff != buff)
                {
                    continue;
                }

                appliedBuffs.RemoveAt(i);
                foreach (IEffect effect in buff.Effects)
                {
                    UnregisterEffect(effect);
                }

                return;
            }

            Debug.LogError("Failed to unregister buff: not registered");
        }

        // TODO: match type of effect with corresponding Executor type.
        private void UnregisterEffect(IEffect effect)
        {
            if (effect is StressEffect se)
            {
                Stress.UnregisterEffect(se);
            }
            else if (effect is NeedModifierEffect nme)
            {
                buffsNeedModifiers.UnregisterEffect(nme);
            }
            else if (effect is ControllerEffect ce)
            {
                controller.UnregisterEffect(ce);
            }
            else if (effect is EarnedMoneyEffect eme)
            {
                incomeGenerator.UnregisterEffect(eme);
            }
            else
            {
                throw new InvalidOperationException("Unknown effect type");
            }
        }
    }

    internal class BuffsNeedModifiersPool : IEffectExecutor<NeedModifierEffect>
    {
        private List<(NeedModifierEffect, NeedModifiers)> registeredModifiers = new();
        private EmployeeImpl employee;

        public BuffsNeedModifiersPool(EmployeeImpl employee)
        {
            this.employee = employee;
        }

        public void RegisterEffect(NeedModifierEffect effect)
        {
            GameObject mods_go = new("_buff_need_modifiers", typeof(NeedModifiers));
            mods_go.transform.SetParent(employee.transform);
            NeedModifiers mods = mods_go.GetComponent<NeedModifiers>();
            mods.SetRawModifiers(effect.NeedModifiers.ToList());

            registeredModifiers.Add((effect, mods));
            employee.RegisterModifier(mods);
        }

        public void UnregisterEffect(NeedModifierEffect effect)
        {
            int to_remove = -1;
            for (int i = 0; i < registeredModifiers.Count; i++)
            {
                if (registeredModifiers[i].Item1 == effect)
                {
                    to_remove = i;
                    break;
                }
            }

            if (to_remove == -1)
            {
                Debug.LogError("Failed to unregister NeedModifierEffect: Not registered");
                return;
            }

            employee.UnregisterModifier(registeredModifiers[to_remove].Item2);
            GameObject.Destroy(registeredModifiers[to_remove].Item2.gameObject);
            registeredModifiers.RemoveAt(to_remove);
        }
    }
}
