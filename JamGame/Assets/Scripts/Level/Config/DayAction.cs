using System;
using System.Collections.Generic;
using System.Linq;
using Employee.Personality;
using Level.GlobalTime;
using Sirenix.OdinInspector;
using TileBuilder;
using UnityEngine;
using IPersonalityConfig = Employee.Personality.IConfig;

namespace Level.Config
{
    public interface IDayAction
    {
        public void Execute(Executor executor);
    }

    [Serializable]
    public class LoadLevel : IDayAction
    {
        [SerializeField]
        private BuildingConfig buildingConfig;
        public BuildingConfig BuildingConfig => buildingConfig;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Cutscene : IDayAction
    {
        [SerializeField]
        private RealTimeSeconds duration;
        public RealTimeSeconds Duration => duration;

        [SerializeField]
        private string text;
        public string Text => text;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class DayStart : IDayAction
    {
        [SerializeField]
        private int morningMoney;
        public int MorningMoney => morningMoney;

        [SerializeField]
        private RealTimeSeconds duration;
        public RealTimeSeconds Duration => duration;

        [SerializeField]
        private float employeeEnableDelay;
        public float EmployeeEnableDelay => employeeEnableDelay;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class PreMeeting : IDayAction
    {
        [SerializeField]
        private float minWaitingTime;
        public float MinWaitingTime => minWaitingTime;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Meeting : IDayAction
    {
        [SerializeReference]
        [FoldoutGroup("Employees to sell in shop")]
        private List<IPersonalityConfig> shopEmployees = new();
        public IEnumerable<PersonalityImpl> ShopEmployees =>
            shopEmployees.Select(x => x.GetPersonality());

        private enum SourceMode
        {
            Raw,
            ScriptableObject
        }

        [SerializeField]
        [EnumToggleButtons]
        [FoldoutGroup("Rooms to sell in shop")]
        private SourceMode mode = SourceMode.Raw;

        [SerializeReference]
        [FoldoutGroup("Rooms to sell in shop")]
        [ShowIf("@" + nameof(mode), SourceMode.Raw)]
        private List<IShopRoomConfig> shopRooms = new();

        [SerializeField]
        [FoldoutGroup("Rooms to sell in shop")]
        [ShowIf("@" + nameof(mode), SourceMode.ScriptableObject)]
        private MeetingShopRoomBundle meetingShopRoomBundle;

        public IEnumerable<ShopRoomConfig> ShopRooms =>
            mode switch
            {
                SourceMode.ScriptableObject => meetingShopRoomBundle.GetShopRooms(),
                SourceMode.Raw => shopRooms.Select(x => x.GetRoomConfig()),
                _ => throw new ArgumentOutOfRangeException()
            };

        [SerializeField]
        [FoldoutGroup("Rooms that will placed in inventory")]
        private List<InventoryRoomConfig> mandatoryRooms = new();
        public IEnumerable<InventoryRoomConfig> MandatoryRooms => mandatoryRooms;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Working : IDayAction
    {
        [SerializeField]
        private InGameTime duration = new();
        public InGameTime Duration => duration;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class PreDayEnd : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class DayEnd : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class LoseGame : IDayAction
    {
        public enum Cause
        {
            BossOverstress,
            NegativeMoney
        }

        [ReadOnly]
        [SerializeField]
        private Cause cause;
        public Cause LoseCause => cause;

        public LoseGame(Cause cause)
        {
            this.cause = cause;
        }

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class WinGame : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}
