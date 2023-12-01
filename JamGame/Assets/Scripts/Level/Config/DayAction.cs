using Level.GlobalTime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Level.Config
{
    public interface IDayAction
    {
        public void Execute(Executor executor);
    }

    [Serializable]
    public class Cutscene : IDayAction
    {
        [OdinSerialize]
        public float Duration { get; private set; }

        [OdinSerialize]
        public string Text { get; private set; }

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class DayStart : IDayAction
    {
        [OdinSerialize]
        public int MorningMoney { get; private set; }

        [OdinSerialize]
        public float Duration { get; private set; }

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class PreMeeting : IDayAction
    {
        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Meeting : IDayAction
    {
        [OdinSerialize]
        [ShowInInspector]
        private List<IEmployeeConfig> shopEmployees = new();

        public IEnumerable<EmployeeConfig> ShopEmployees =>
            shopEmployees.Select(x => x.GetEmployeeConfig());

        [OdinSerialize]
        [ShowInInspector]
        private List<IShopRoomConfig> shopRooms = new();
        public IEnumerable<ShopRoomConfig> ShopRooms => shopRooms.Select(x => x.GetRoomConfig());

        [OdinSerialize]
        public IEnumerable<InventoryRoomConfig> MandatoryRooms { get; private set; } =
            new List<InventoryRoomConfig>();

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Working : IDayAction
    {
        [OdinSerialize]
        public Days Duration { get; private set; }

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
