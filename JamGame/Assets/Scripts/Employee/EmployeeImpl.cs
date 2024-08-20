using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Employee.Controller;
using Employee.Needs;
using Employee.Personality;
using Employee.StressMeter;
using Level.Boss.Task;
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
    public partial class EmployeeImpl : MonoBehaviour
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

        private NeedProviderManager needProviderManager;
        public NeedProviderManager NeedProviderManager
        {
            set => needProviderManager = value;
        }

        [SerializeField]
        private List<Need> needs = new();

        private Need currentNeed;
        private Need currentlySatisfyingNeed;
        private NeedProvider targetNeedProvider = null;

        public event Action<NeedType> NeedSatisifed;

        private ControllerImpl controller;

        [SerializeField]
        private IncomeGenerator.Model incomeGenerator;
        public IncomeGenerator.Model IncomeGenerator => incomeGenerator;

        public StressMeterImpl Stress { get; private set; }

        public NeedType? CurrentNeedType => currentlySatisfyingNeed?.NeedType;

        private NeedProvider.PlaceInWaitingLine placeInWaitingLine = null;

        [MinMaxSlider(0, 10, true)]
        [SerializeField]
        private Vector2 keepDistanceFromNextInLine = new(1.0f, 1.5f);

        [SerializeField]
        [ReadOnly]
        private PersonalityImpl personality;
        public PersonalityImpl Personality => personality;

        private DataProvider<EmployeeQuirks> employeeQuirksDataProvider;

        private void OnEnable()
        {
            controller.OnReachedNeedProvider += ReachedNeedProvider;
            state = State.Idle;
        }

        private void OnDisable()
        {
            controller.OnReachedNeedProvider -= ReachedNeedProvider;
            placeInWaitingLine?.Drop();
        }

        private void Awake()
        {
            controller = GetComponent<ControllerImpl>();
            Stress = GetComponent<StressMeterImpl>();

            buffsNeedModifiers = new BuffsNeedModifiersPool(this);

            appliedBuffs.CollectionChanged += (s, e) =>
                AppliedBuffsChanged?.Invoke(s, AppliedBuffsCollectionChangedMapping(e));
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
                        controller.MoveToNeedProvider(targetNeedProvider.transform.position);
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

        public void AddNeed(Need.NeedProperties properties)
        {
            Need need = new(properties);
            foreach (NeedModifiers modifier in registeredModifiers)
            {
                need.RegisterModifier(modifier);
            }
            needs.Add(need);
        }

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
                    .ToList();

                List<NeedProvider> bound_to = available_providers
                    .Where(need_provider => need_provider.IsEmployeeBound(this))
                    .ToList();

                switch (bound_to.Count)
                {
                    case 0:
                        break;
                    case 1:
                        available_providers = bound_to;
                        break;
                    default:
                        Debug.LogError(
                            "Employee is bound to multiple NeedProviders with the same NeedType"
                        );
                        break;
                }

                NeedProvider selected_provider = null;
                float min_distance = float.PositiveInfinity;
                foreach (NeedProvider provider in available_providers)
                {
                    float? distance = controller.ApproximatePathLength(provider.transform.position);
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

            controller.Teleport(needProvider.transform.position);
            controller.SetNavigationMode(ControllerImpl.NavigationMode.FreeMove);

            ReachedNeedProvider();

            return true;
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

        public void SetPersonality(PersonalityImpl personality)
        {
            this.personality = personality;

            foreach (Quirk quirk in personality.Quirks)
            {
                foreach (Need.NeedProperties additional_need in quirk.AdditionalNeeds)
                {
                    AddNeed(additional_need);
                }

                foreach (IEffect effect in quirk.Effects)
                {
                    RegisterEffect(effect);
                }
            }

            employeeQuirksDataProvider = new DataProvider<EmployeeQuirks>(
                () => new EmployeeQuirks() { Quirks = this.personality.Quirks },
                DataProviderServiceLocator.ResolveType.MultipleSources
            );
        }
    }
}
