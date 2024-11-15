using System.Collections.Generic;

namespace FileShaper
{
    public class Data
    {
        public double ReferenceThickness { get; set; } = double.NaN;
        public double ReflectionFactor { get; set; } = double.NaN;
        public TraValue[] SpectralTransmission => spectralTransmission.ToArray();
            
        private readonly List<TraValue> spectralTransmission = new List<TraValue>();

        public void AddValue(TraValue point) => spectralTransmission.Add(point);

    }
}
