using System;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    public class SerializedDayAction
    {
        [SerializeField]
        private string selectedType;

        [SerializeField]
        private DayEnd dayEnd;

        [SerializeField]
        private DayStart dayStart;

        [SerializeField]
        private Meeting meeting;

        [SerializeField]
        private Working working;

        public IDayAction ToDayAction()
        {
            return selectedType switch
            {
                "DayEnd" => dayEnd,
                "DayStart" => dayStart,
                "Meeting" => meeting,
                "Working" => working,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
