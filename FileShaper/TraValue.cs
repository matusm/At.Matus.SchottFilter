namespace FileShaper
{
    public class TraValue
    {
        public double X { get; set; } = double.NaN;
        public double Y { get; set; } = double.NaN;

        public TraValue() { }

        public TraValue(double w, double t)
        {
            X = w;
            Y = t;
        }
    }
}
