using At.Matus.SchottFilter;
using System;

namespace SearchFilterStack
{
    public class ResultPod : IComparable<ResultPod>
    {
        public SchottFilter FilterCombination { get; }
        public string Designation => FilterCombination.Designation;
        public double Fitness { get; }
        public double TauPass { get; }
        public double TauBlock { get; }

        public ResultPod(SchottFilter filter,
                         double fitness,
                         double tauPass,
                         double tauBlock)
        {
            FilterCombination = filter;
            Fitness = fitness;
            TauPass = tauPass;
            TauBlock = tauBlock;
        }

        public override string ToString() => $"[{Designation}]    {Fitness:F0} {TauPass * 100:F0}% {TauBlock * 100:F3}%";

        public int CompareTo(ResultPod other)
        {
            if (Fitness < other.Fitness) return -1;
            if (Fitness > other.Fitness) return 1;
            return 0;
        }
    }
}
