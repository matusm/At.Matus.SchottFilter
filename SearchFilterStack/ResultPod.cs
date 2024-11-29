using At.Matus.SchottFilter;
using System;

namespace SearchFilterStack
{
    public class ResultPod : IComparable<ResultPod>
    {
        public SchottFilter FilterCombination { get; }
        public string Designation => FilterCombination.Designation;
        public double Fitness { get; }
        public double TauAvaPass { get; }
        public double TauAvaBlock { get; }
        public double TauMaxPass { get; }
        public double TauMaxBlock { get; }
        public double TauMinPass { get; }
        public double TauMinBlock { get; }

        public ResultPod(SchottFilter filter, FilterSpecification specs)
        {
            FilterCombination = filter;
            Fitness = specs.Fitness(filter);
            TauAvaPass = specs.AverageTransmission(filter);
            TauAvaBlock = specs.AverageBlocking(filter);
            TauMaxPass = specs.MaximumTransmission(filter);
            TauMaxBlock = specs.MaximumBlocking(filter);
            TauMinPass = specs.MinimumTransmission(filter);
            TauMinBlock = specs.MinimumBlocking(filter);
        }

        public override string ToString() => 
            $"[{Designation}]    {Fitness:F0} PASS%({TauMinPass * 100:F0};{TauAvaPass * 100:F0};{TauMaxPass * 100:F0}) BLOCK%({TauMinBlock * 100:F3};{TauAvaBlock * 100:F3};{TauMaxBlock * 100:F3})";

        public int CompareTo(ResultPod other)
        {
            if (Fitness < other.Fitness) return -1;
            if (Fitness > other.Fitness) return 1;
            return 0;
        }

    }
}
