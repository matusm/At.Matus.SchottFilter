namespace FileShaper
{
    public class TraValue
    {
        public double Wavelength { get; } = double.NaN;
        public double Transmittance { get; } = double.NaN;

        public TraValue() { }

        public TraValue(double w, double t)
        {
            Wavelength = w;
            Transmittance = t;
        }
    }
}
