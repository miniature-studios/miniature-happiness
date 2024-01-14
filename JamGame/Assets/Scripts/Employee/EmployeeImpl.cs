using Location;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Employee
{
    [SelectionBase]
    [RequireComponent(typeof(Controller))]
    [RequireComponent(typeof(StressEffect))]
    [AddComponentMenu("Scripts/Employee.Employee")]
    public class EmployeeImpl : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Walking,
            SatisfyingNeed
        }

        [SerializeField]
        [ReadOnly]
        private State state = State.Idle;

        [SerializeField]
        private NeedProviderManager needProviderManager;

        [SerializeField]
        private List<Need> needs = new();
        private Need topPriorityNeed;
        private Need currentlySatisfyingNeed;
        private Need latestSatisfiedNeed;
        private float satisfyingNeedRemaining = 0.0f;
        private NeedProvider targetNeedProvider = null;

        private Controller controller;

        private List<NeedModifiers> registeredModifiers = new();

        [SerializeField]
        private IncomeGenerator.Model incomeGenerator;

        public StressMeter Stress { get; private set; }

        public NeedType? LatestSatisfiedNeedType => latestSatisfiedNeed?.NeedType;

        [Serializable]
        private struct AppliedBuff
        {
            [ReadOnly]
            public Buff Buff;

            [ReadOnly]
            public float RemainingTime;
        }

        [SerializeField]
        private List<AppliedBuff> appliedBuffs = new();

        // TODO: Will change when BuffView will be implemented.
        public IEnumerable<Buff> Buffs => appliedBuffs.Select(buff => buff.Buff);

        private BuffsNeedModifiersPool buffsNeedModifiers;

        private void Start()
        {
            controller = GetComponent<Controller>();
            Stress = GetComponent<StressMeter>();

            buffsNeedModifiers = new BuffsNeedModifiersPool(this);
        }

        private void Update()
        {
            UpdateNeeds(Time.deltaTime);
            Stress.UpdateStress(needs, Time.deltaTime);
            UpdateBuffs(Time.deltaTime);

            switch (state)
            {
                case State.Idle:
                    targetNeedProvider = GetTargetNeedProvider();

                    if (targetNeedProvider == null)
                    {
                        return;
                    }

                    state = State.Walking;
                    controller.SetDestination(targetNeedProvider.transform.position);
                    break;
                case State.Walking:
                    if (
                        !targetNeedProvider.IsAvailable(this)
                        || targetNeedProvider.NeedType != topPriorityNeed.NeedType
                    )
                    {
                        state = State.Idle;
                    }
                    break;
                case State.SatisfyingNeed:
                    satisfyingNeedRemaining -= Time.deltaTime;
                    if (satisfyingNeedRemaining < 0.0f)
                    {
                        state = State.Idle;
                        currentlySatisfyingNeed.Satisfy();
                        latestSatisfiedNeed = currentlySatisfyingNeed;
                        incomeGenerator.NeedComplete(currentlySatisfyingNeed);
                        currentlySatisfyingNeed = null;

                        targetNeedProvider.Release();
                        targetNeedProvider = null;
                    }
                    break;
            }
        }

        private void UpdateNeeds(float delta_time)
        {
            foreach (Need need in needs)
            {
                need.Dissatisfy(delta_time);
            }
        }

        private void UpdateBuffs(float delta_time)
        {
            for (int i = appliedBuffs.Count - 1; i >= 0; i--)
            {
                AppliedBuff ab = appliedBuffs[i];
                ab.RemainingTime -= delta_time;
                if (ab.RemainingTime < 0.0f)
                {
                    UnregisterBuff(appliedBuffs[i].Buff);
                }
                else
                {
                    appliedBuffs[i] = ab;
                }
            }
        }

        public void AddNeed(Need.NeedProperties properties)
        {
            Need need = new(properties);
            foreach (NeedModifiers modifer in registeredModifiers)
            {
                need.RegisterModifier(modifer);
            }
            needs.Add(need);
        }

        // NOTE: We may want to preserve it between levels, so we may need to serialize it in this case.
        private Dictionary<NeedType, NeedProvider> needProviderBindings = new();

        private NeedProvider GetTargetNeedProvider()
        {
            needs.Sort((x, y) => x.Satisfied.CompareTo(y.Satisfied));

            if (state == State.Idle)
            {
                // Force select top-priority need.
                topPriorityNeed = null;
            }

            foreach (Need need in needs)
            {
                if (need == topPriorityNeed)
                {
                    break;
                }

                List<NeedProvider> available_providers = needProviderManager
                    .FindAllAvailableProviders(this, need.NeedType)
                    .Where(
                        np =>
                            !needProviderBindings.ContainsKey(np.NeedType)
                            || needProviderBindings[np.NeedType] == np
                    )
                    .ToList();

                NeedProvider selected_provider = null;
                float min_distance = float.PositiveInfinity;
                foreach (NeedProvider provider in available_providers)
                {
                    float? distance = controller.ComputePathLength(provider);
                    if (!distance.HasValue)
                    {
                        Debug.LogWarning($"NeedProvider {provider.name} is inaccessible");
                        continue;
                    }

                    if (distance.Value < min_distance)
                    {
                        min_distance = distance.Value;
                        selected_provider = provider;
                    }
                }

                if (selected_provider == null)
                {
                    continue;
                }

                topPriorityNeed = need;
                return selected_provider;
            }

            Debug.LogError("Failed to select target NeedProvider");
            return null;
        }

        public void TeleportToNeedProvider(NeedProvider needProvider)
        {
            controller.Teleport(needProvider);
        }

        public void BindToNeedProvider(NeedProvider need_provider)
        {
            if (needProviderBindings.ContainsKey(need_provider.NeedType))
            {
                if (needProviderBindings[need_provider.NeedType] != need_provider)
                {
                    Debug.LogError("Trying to bind NeedProvider when there's already one binding");
                }

                return;
            }

            needProviderBindings.Add(need_provider.NeedType, need_provider);
        }

        private void FinishedMoving()
        {
            if (targetNeedProvider.TryTake(this))
            {
                state = State.SatisfyingNeed;
                satisfyingNeedRemaining = topPriorityNeed.GetProperties().SatisfactionTime;
                currentlySatisfyingNeed = topPriorityNeed;
            }
            else
            {
                state = State.Idle;
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

        private void OnEnable()
        {
            controller ??= GetComponent<Controller>();

            controller.OnFinishedMoving += FinishedMoving;
        }

        private void OnDisable()
        {
            controller.OnFinishedMoving -= FinishedMoving;
        }

        // TODO: match type of effect with corresponding Executor type.
        public void RegisterBuff(Buff buff)
        {
            appliedBuffs.Add(new AppliedBuff { Buff = buff, RemainingTime = buff.Time });
            foreach (IEffect effect in buff.Effects)
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
            }
        }

        // TODO: match type of effect with corresponding Executor type.
        public void UnregisterBuff(Buff buff)
        {
            for (int i = 0; i < appliedBuffs.Count; i++)
            {
                if (appliedBuffs[i].Buff == buff)
                {
                    appliedBuffs.RemoveAt(i);

                    foreach (IEffect effect in buff.Effects)
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
                    }

                    return;
                }
            }

            Debug.LogError("Failed to unregister buff: not registered");
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
