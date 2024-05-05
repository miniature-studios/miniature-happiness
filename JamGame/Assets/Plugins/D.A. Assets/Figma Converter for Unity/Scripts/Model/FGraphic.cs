using DA_Assets.FCU.Model;

namespace DA_Assets.FCU
{
    public struct FGraphic
    {
        public bool HasFill { get; set; }
        public bool HasStroke { get; set; }
        public Paint SolidFill { get; set; }
        public Paint SolidStroke { get; set; }
        public Paint GradientFill { get; set; }
        public Paint GradientStroke { get; set; }
    }
}
