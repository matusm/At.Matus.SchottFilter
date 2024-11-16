using System.Collections.Generic;

namespace FileShaper
{
    public class FilterData
    {
        public double ReferenceThickness { get; set; } = double.NaN;
        public double ReflectionFactor { get; set; } = double.NaN;
        public TraValue[] SpectralTransmission => spectralTransmission.ToArray();
            
        private readonly List<TraValue> spectralTransmission = new List<TraValue>();

        public void AddValue(TraValue point)
        {
            if (double.IsNaN(point.Y)) point.Y = 0;
            spectralTransmission.Add(point);
        }
    }
}
