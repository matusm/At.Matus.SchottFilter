namespace At.Matus.SchottFilter
{
    public class SpecificationRange
    {
        public int LowerWavelength { get; }
        public int UpperWavelength { get; }
        public double Tau { get; }

        public SpecificationRange(int lwl, int uwl, double t)
        {
            LowerWavelength = lwl;
            UpperWavelength = uwl;
            Tau = t;
        }

        public override string ToString() => $@"[{LowerWavelength} nm - {UpperWavelength} nm] : {Tau * 100} %";
    }
}
