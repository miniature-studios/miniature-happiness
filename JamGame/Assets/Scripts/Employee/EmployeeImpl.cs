using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Employee.Controller;
using Employee.Needs;
using Employee.StressMeter;
using Level.GlobalTime;
using Location;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee
{
    [SelectionBase]
    [RequireComponent(typeof(ControllerImpl))]
    [RequireComponent(typeof(StressEffect))]
    [RequireComponent(typeof(StressMeterImpl))]
    [AddComponentMenu("Scripts/Employee/Employee")]
    public class EmployeeImpl : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Walking,
            InWaitingLine,
            ApproachingNeedProvider,
            SatisfyingNeed
        }

        [SerializeField]
        [ReadOnly]
        private State state = State.Idle;

        [SerializeField]
        private NeedProviderManager needProviderManager;

        [SerializeField]
        private List<Need> needs = new();
        private Need currentNeed;
        private Need currentlySatisfyingNeed;
        private NeedProvider targetNeedProvider = null;

        public event Action<NeedType> NeedSatisifed;

        private ControllerImpl controller;

        [ReadOnly]
        [SerializeField]
        private List<NeedModifiers> registeredModifiers = new();

        [SerializeField]
        private IncomeGenerator.Model incomeGenerator;

        public StressMeterImpl Stress { get; private set; }

        public NeedType? CurrentNeedType => currentlySatisfyingNeed?.NeedType;

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

        private NeedProvider.PlaceInWaitingLine placeInWaitingLine = null;

        [MinMaxSlider(0, 10, true)]
        [SerializeField]
        private Vector2 keepDistanceFromNextInLine = new(1.0f, 1.5f);

        private void Awake()
        {
            appliedBuffs.CollectionChanged += (s, e) =>
                AppliedBuffsChanged?.Invoke(s, AppliedBuffsCollectionChangedMapping(e));
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

        private void Start()
        {
            controller = GetComponent<ControllerImpl>();
            Stress = GetComponent<StressMeterImpl>();

            buffsNeedModifiers = new BuffsNeedModifiersPool(this);
        }

        private void Update()
        {
            RealTimeSeconds delta_time = RealTimeSeconds.FromDeltaTime();

            UpdateNeeds(delta_time);
            Stress.UpdateStress(needs, delta_time);
            UpdateBuffs(delta_time);

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
                    if (targetNeedProvider.NeedType != currentNeed.NeedType)
                    {
                        state = State.Idle;
                    }

                    placeInWaitingLine = targetNeedProvider.TryLineUp(this);
                    if (placeInWaitingLine == null)
                    {
                        state = State.Idle;
                        break;
                    }

                    state = State.InWaitingLine;

                    break;
                case State.InWaitingLine:
                    if (targetNeedProvider.NeedType != currentNeed.NeedType)
                    {
                        placeInWaitingLine.Drop();
                        placeInWaitingLine = null;
                        state = State.Idle;
                    }

                    if (placeInWaitingLine.GetNextInLine() == null)
                    {
                        controller.SetNavigationMode(ControllerImpl.NavigationMode.Navmesh);
                        controller.SetDestination(targetNeedProvider.transform.position);
                        state = State.ApproachingNeedProvider;
                        break;
                    }

                    Vector3 next_in_line_position = placeInWaitingLine
                        .GetNextInLine()
                        .transform.position;

                    Vector3 direction_to_next_in_line = transform.position - next_in_line_position;
                    direction_to_next_in_line.y = 0;
                    float distance_to_next_in_line = direction_to_next_in_line.magnitude;

                    if (distance_to_next_in_line < keepDistanceFromNextInLine.x)
                    {
                        // NOTE: do we need to process this case?
                        break;
                    }

                    if (distance_to_next_in_line > keepDistanceFromNextInLine.y)
                    {
                        controller.SetNavigationMode(ControllerImpl.NavigationMode.Navmesh);
                        controller.SetDestination(next_in_line_position);
                        break;
                    }

                    controller.SetNavigationMode(ControllerImpl.NavigationMode.FreeMove);

                    break;
                case State.ApproachingNeedProvider:
                    break;
                case State.SatisfyingNeed:
                    break;
            }
        }

        private void UpdateNeeds(RealTimeSeconds delta_time)
        {
            foreach (Need need in needs)
            {
                need.Dissatisfy(delta_time);
            }
        }

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

        public void AddNeed(Need.NeedProperties properties)
        {
            Need need = new(properties);
            foreach (NeedModifiers modifier in registeredModifiers)
            {
                need.RegisterModifier(modifier);
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
                currentNeed = null;
            }

            foreach (Need need in needs)
            {
                if (need == currentNeed)
                {
                    break;
                }

                List<NeedProvider> available_providers = needProviderManager
                    .FindAllAvailableProviders(this, need.NeedType)
                    .Where(np =>
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

                currentNeed = need;
                return selected_provider;
            }

            Debug.LogError("Failed to select target NeedProvider");
            return null;
        }

        public bool TryForceTakeNeedProvider(NeedProvider needProvider)
        {
            placeInWaitingLine = needProvider.TryLineUp(this);
            if (placeInWaitingLine == null)
            {
                return false;
            }
            if (placeInWaitingLine.GetNextInLine() != null)
            {
                placeInWaitingLine.Drop();
                return false;
            }

            currentNeed = needs
                .Where(need => need.NeedType == needProvider.NeedType)
                .FirstOrDefault();
            if (currentNeed == null)
            {
                Debug.LogError(
                    "Cannot force employee to take place in need provider: Employee don't have matching need"
                );
                return false;
            }

            targetNeedProvider = needProvider;

            controller.Teleport(needProvider);
            controller.SetNavigationMode(ControllerImpl.NavigationMode.FreeMove);

            ReachedNeedProvider();

            return true;
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

        private void ReachedNeedProvider()
        {
            if (placeInWaitingLine.GetNextInLine() != null)
            {
                Debug.LogWarning(
                    "Possible unwanted employee collision at "
                        + targetNeedProvider.transform.position
                );
                return;
            }

            RealTimeSeconds satisfying_need_remaining = currentNeed
                .GetProperties()
                .SatisfactionTime;
            currentlySatisfyingNeed = currentNeed;
            targetNeedProvider.Take(
                placeInWaitingLine,
                satisfying_need_remaining,
                ReleasedFromNeedProvider
            );
            state = State.SatisfyingNeed;

            // TODO: Remove it when employee serialization will be implemented (#121)
            if (currentlySatisfyingNeed.NeedType == NeedType.Leave)
            {
                placeInWaitingLine.Drop();
                gameObject.SetActive(false);
            }
        }

        public void ReleasedFromNeedProvider()
        {
            state = State.Idle;
            currentlySatisfyingNeed.Satisfy();
            incomeGenerator.NeedComplete(currentlySatisfyingNeed);
            NeedSatisifed?.Invoke(currentlySatisfyingNeed.NeedType);
            currentlySatisfyingNeed = null;
            targetNeedProvider = null;

            controller.SetNavigationMode(ControllerImpl.NavigationMode.Navmesh);
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
            controller = controller != null ? controller : GetComponent<ControllerImpl>();

            controller.OnReachedNeedProvider += ReachedNeedProvider;
        }

        private void OnDisable()
        {
            controller.OnReachedNeedProvider -= ReachedNeedProvider;
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
