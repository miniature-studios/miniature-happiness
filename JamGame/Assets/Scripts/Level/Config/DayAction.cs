using System;
using System.Collections.Generic;
using System.Linq;
using Level.GlobalTime;
using UnityEngine;

namespace Level.Config
{
    public interface IDayAction
    {
        public void Execute(Executor executor);
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
        [SerializeReference]
        private List<IEmployeeConfig> shopEmployees = new();

        public IEnumerable<EmployeeConfig> ShopEmployees =>
            shopEmployees.Select(x => x.GetEmployeeConfig());

        [SerializeReference]
        private List<IShopRoomConfig> shopRooms = new();
        public IEnumerable<ShopRoomConfig> ShopRooms => shopRooms.Select(x => x.GetRoomConfig());

        [SerializeField]
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
        private Days duration = new();
        public Days Duration => duration;

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
