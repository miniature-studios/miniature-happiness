using Common;
using System.Collections.Generic;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion.SpecialRooms
{
    [AddComponentMenu("Scripts/TileUnion.SpecialRooms.MeetingRoom")]
    public class MeetingRoom : MonoBehaviour
    {
        [SerializeField]
        private TileUnionImpl tileUnion;

        public TileUnionImpl TileUnion => tileUnion;

        [SerializeField]
        private List<TileImpl> tilesToAdd;

        public List<TileImpl> TilesToAdd => tilesToAdd;

        [SerializeField]
        private Direction growDirection;
        public Direction GrowDirection => growDirection;

        [SerializeField]
        private int maximumSize;
        public int MaximumSize => maximumSize;

        [SerializeField]
        [InspectorReadOnly]
        private int currentSize = 2;

        public int CurrentSize
        {
            get => currentSize;
            set => currentSize = value;
        }

        [SerializeField]
        private List<string> incorrectMarks = new();
        public IEnumerable<string> IncorrectMarks => incorrectMarks;

        public bool IsNowFitEmployees(int employeeCount)
        {
            return employeeCount <= (currentSize * 2) - 1;
        }

        public bool IsPotentiallyFitEmployees(int employeeCount)
        {
            return employeeCount <= (maximumSize * 2) - 1;
        }
    }
}
