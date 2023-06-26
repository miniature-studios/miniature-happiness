using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [InterfaceEditor]
    public interface IDayAction
    {
        public void Execute(Executor executor, Action next_action);
    }

    [Serializable]
    public class DayEnd : IDayAction
    {
        public void Execute(Executor executor, Action next_action)
        {
            executor.Execute(this, next_action);
        }
    }

    [Serializable]
    public class DayStart : IDayAction
    {
        [SerializeField]
        private int morningMoney;
        public int MorningMoney => morningMoney;

        public void Execute(Executor executor, Action next_action)
        {
            executor.Execute(this, next_action);
        }
    }

    [Serializable]
    public class Meeting : IDayAction
    {
        [SerializeField]
        private List<SerializedEmployeeConfig> shopEmployees;

        [SerializeField]
        private List<SerializedRoomConfig> shopRooms;

        public IEnumerable<EmployeeConfig> ShopEmployees =>
            shopEmployees.Select(x => x.ToEmployeeConfig().GetEmployeeConfig());
        public IEnumerable<RoomConfig> ShopRooms =>
            shopRooms.Select(x => x.ToRoomConfig().GetRoomConfig());

        public void Execute(Executor executor, Action next_action)
        {
            executor.Execute(this, next_action);
        }
    }

    [Serializable]
    public class Working : IDayAction
    {
        [SerializeField]
        private float workingTime;
        public float WorkingTime => workingTime;

        public void Execute(Executor executor, Action next_action)
        {
            executor.Execute(this, next_action);
        }
    }
}
