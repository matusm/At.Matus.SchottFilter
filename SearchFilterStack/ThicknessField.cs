using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFilterStack
{
    public class ThicknessField
    {
        public double MinimumThickness { get; } = 1;
        public double MaximumThickness { get; } = 3;
        public double DeltaThickness { get; } = 1;
        public FieldType Type { get; }

        public ThicknessField() => Type = FieldType.Default;

        public ThicknessField(double min, double max, double delta )
        {
            Type = FieldType.Custom;
            MinimumThickness = min;
            MaximumThickness = max;
            DeltaThickness = delta;
        }

        public ThicknessField(FieldType type) => Type = type;

    }

    public enum FieldType
    {
        Intrinsic,
        Default,
        Custom
    }
}
