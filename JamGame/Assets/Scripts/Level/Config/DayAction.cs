using Level.GlobalTime;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Level.Config
{
    [HideReferenceObjectPicker]
    public interface IDayAction
    {
        public void Execute(Executor executor);
    }

    [Serializable]
    public class Cutscene : IDayAction
    {
        [OdinSerialize]
        [FoldoutGroup("Cutscene")]
        public float Duration { get; private set; }

        [OdinSerialize]
        [FoldoutGroup("Cutscene")]
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
        [FoldoutGroup("Day Start")]
        public int MorningMoney { get; private set; }

        [OdinSerialize]
        [FoldoutGroup("Day Start")]
        public float Duration { get; private set; }

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class PreMeeting : IDayAction
    {
        [ReadOnly]
        [Discardable]
        [FoldoutGroup("Pre Meeting")]
        public string UselessDescription = "UselessDescription";

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Meeting : IDayAction
    {
        [OdinSerialize]
        [FoldoutGroup("Meeting")]
        public List<IEmployeeConfig> ShopEmployeesConfig = new();
        public IEnumerable<EmployeeConfig> ShopEmployees =>
            ShopEmployeesConfig.Select(x => x.GetEmployeeConfig());

        [OdinSerialize]
        [FoldoutGroup("Meeting")]
        public List<IShopRoomConfig> ShopRoomsConfig = new();
        public IEnumerable<ShopRoomConfig> ShopRooms =>
            ShopRoomsConfig.Select(x => x.GetRoomConfig());

        [OdinSerialize]
        [FoldoutGroup("Meeting")]
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
        [FoldoutGroup("Working")]
        public Days Duration { get; private set; }

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class PreDayEnd : IDayAction
    {
        [ReadOnly]
        [Discardable]
        [FoldoutGroup("Pre Day End")]
        public string UselessDescription = "UselessDescription";

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class DayEnd : IDayAction
    {
        [ReadOnly]
        [Discardable]
        [FoldoutGroup("Day End")]
        public string UselessDescription = "UselessDescription";

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class LoseGame : IDayAction
    {
        [ReadOnly]
        [Discardable]
        [FoldoutGroup("Lose Game")]
        public string UselessDescription = "UselessDescription";

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class WinGame : IDayAction
    {
        [ReadOnly]
        [Discardable]
        [FoldoutGroup("Win Game")]
        public string UselessDescription = "UselessDescription";

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}
