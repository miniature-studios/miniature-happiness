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

        [SerializeField]
        private Cutscene cutscene;

        public IDayAction ToDayAction()
        {
            return selectedType switch
            {
                "DayEnd" => dayEnd,
                "DayStart" => dayStart,
                "Meeting" => meeting,
                "Working" => working,
                "Cutscene" => cutscene,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
