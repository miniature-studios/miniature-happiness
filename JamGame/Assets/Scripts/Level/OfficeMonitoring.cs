using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using UnityEngine;

namespace Level
{
    public class OfficeMonitoring : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        public struct OfficeInfo
        {
            public int InsideTilesCount;
            public IEnumerable<RoomProperties> RoomProperties;
        }

        public OfficeInfo GetOfficeInfo()
        {
            return new()
            {
                InsideTilesCount = tileBuilder.GetAllInsideListPositions().Count(),
                RoomProperties = tileBuilder
                    .GetTileUnionsInPositions(tileBuilder.GetAllInsideListPositions())
                    .Where(x => x.TryGetComponent(out RoomProperties roomProperties))
                    .Select(x => x.GetComponent<RoomProperties>())
            };
        }
    }
}
