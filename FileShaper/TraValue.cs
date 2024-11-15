namespace FileShaper
{
    public class TraValue
    {
        public double Wavelength { get; set; } = double.NaN;
        public double Transmittance { get; set; } = double.NaN;

        public TraValue() { }

        public TraValue(double w, double t)
        {
            Wavelength = w;
            Transmittance = t;
        }
    }
}
