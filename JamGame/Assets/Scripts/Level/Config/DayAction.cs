using Common;
using Level.GlobalTime;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [InterfaceEditor]
    public interface IDayAction
    {
        public void Execute(Executor executor);
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
    public class DayStart : IDayAction
    {
        [SerializeField]
        private int morningMoney;
        public int MorningMoney => morningMoney;

        [SerializeField]
        private float duration;
        public float Duration => duration;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Meeting : IDayAction
    {
        [SerializeField]
        private List<SerializedEmployeeConfig> shopEmployees;

        [SerializeField]
        private List<SerializedShopRoomConfig> shopRooms;

        [SerializeField]
        private List<InventoryRoomConfig> mandatoryRooms;

        public IEnumerable<EmployeeConfig> ShopEmployees =>
            shopEmployees.Select(x => x.ToEmployeeConfig().GetEmployeeConfig());

        public IEnumerable<ShopRoomConfig> ShopRooms =>
            shopRooms.Select(x => x.ToShopRoomConfig().GetRoomConfig());

        public ImmutableList<InventoryRoomConfig> MandatoryRooms =>
            mandatoryRooms.ToImmutableList();

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Working : IDayAction
    {
        [SerializeField]
        private Days duration;
        public Days Duration => duration;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }

    [Serializable]
    public class Cutscene : IDayAction
    {
        [SerializeField]
        private float duration;
        public float Duration => duration;

        [SerializeField]
        private string text;
        public string Text => text;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}
