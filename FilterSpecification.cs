using At.Matus.StatisticPod;
using System.Collections.Generic;

namespace At.Matus.SchottFilter
{
    public class FilterSpecification
    {
        public SpecificationRange[] PassBands => passBands.ToArray();
        public SpecificationRange[] BlockingBands => blockingBands.ToArray();

        public FilterSpecification()
        {
            _SetMinimumPermissibleSpectralTransmittance(0, fieldSize, 0);
            _SetMaximumPermissibleSpectralTransmittance(0, fieldSize, 1);
        }

        public void SetPassRange(int lowerWavelength, int upperWavelength, double tau)
        {
            _SetMinimumPermissibleSpectralTransmittance(lowerWavelength - minWavelength, upperWavelength - minWavelength, tau);
            passBands.Add(new SpecificationRange(lowerWavelength, upperWavelength, tau));
        }

        public void SetBlockingRange(int lowerWavelength, int upperWavelength, double tau)
        {
            _SetMaximumPermissibleSpectralTransmittance(lowerWavelength - minWavelength, upperWavelength - minWavelength, tau);
            blockingBands.Add(new SpecificationRange(lowerWavelength, upperWavelength, tau));
        }

        public bool Conforms(SchottFilter filter)
        {
            for (int i = 0; i < fieldSize; i++)
            {
                double tau = filter.GetInternalTransmittance(i + minWavelength);
                if (tau > maximumPermissibleSpectralTransmittance[i]) return false;
                if (tau < minimumPermissibleSpectralTransmittance[i]) return false;
            }
            return true;
        }

        public double Fitness(SchottFilter filter)
        {
            StatisticPod.StatisticPod pa = PassParameters(filter);
            StatisticPod.StatisticPod bl = BlockParameters(filter);
            // parameters which may be used to construct a fitness function
            double averagePass = pa.AverageValue*100;
            double maxPass = pa.MaximumValue*100;
            double minPas = pa.MinimumValue*100;
            double averageBlock = bl.AverageValue*100;
            double maxBlock = bl.MaximumValue*100;
            double minBlock = bl.MinimumValue*100;
            // this is the fitness function
            double fitness = averagePass / (maxBlock);
            return fitness;
        }

        public StatisticPod.StatisticPod PassParameters(SchottFilter filter)
        {
            StatisticPod.StatisticPod passStat = new StatisticPod.StatisticPod();
            for (int i = 0; i < fieldSize; i++)
            {
                double tau = filter.GetInternalTransmittance(i + minWavelength);
                if (minimumPermissibleSpectralTransmittance[i] != 0)
                {
                    passStat.Update(tau);
                }
            }
            return passStat;
        }

        public StatisticPod.StatisticPod BlockParameters(SchottFilter filter)
        {
            StatisticPod.StatisticPod blockStat = new StatisticPod.StatisticPod();
            for (int i = 0; i < fieldSize; i++)
            {
                double tau = filter.GetInternalTransmittance(i + minWavelength);
                if (maximumPermissibleSpectralTransmittance[i] != 1)
                {
                    blockStat.Update(tau);
                }
            }
            return blockStat;
        }

        public double AverageTransmission(SchottFilter filter) => PassParameters(filter).AverageValue;
        public double AverageBlocking(SchottFilter filter) => BlockParameters(filter).AverageValue;
        public double MinimumTransmission(SchottFilter filter) => PassParameters(filter).MinimumValue;
        public double MinimumBlocking(SchottFilter filter) => BlockParameters(filter).MinimumValue;
        public double MaximumTransmission(SchottFilter filter) => PassParameters(filter).MaximumValue;
        public double MaximumBlocking(SchottFilter filter) => BlockParameters(filter).MaximumValue;

        private void _SetMinimumPermissibleSpectralTransmittance(int lowerIdx, int upperIdx, double tau)
        {
            if (lowerIdx > upperIdx) return;
            if (lowerIdx < 0) lowerIdx = 0;
            if (upperIdx > fieldSize) upperIdx = fieldSize;
            for (int i = lowerIdx; i < upperIdx; i++)
            {
                minimumPermissibleSpectralTransmittance[i] = tau;
            }
        }

        private void _SetMaximumPermissibleSpectralTransmittance(int lowerIdx, int upperIdx, double tau)
        {
            if (lowerIdx > upperIdx) return;
            if (lowerIdx < 0) lowerIdx = 0;
            if (upperIdx > fieldSize) upperIdx = fieldSize;
            for (int i = lowerIdx; i < upperIdx; i++)
            {
                maximumPermissibleSpectralTransmittance[i] = tau;
            }
        }

        private readonly double[] maximumPermissibleSpectralTransmittance = new double[fieldSize];
        private readonly double[] minimumPermissibleSpectralTransmittance = new double[fieldSize];
        private readonly List<SpecificationRange> passBands = new List<SpecificationRange>();
        private readonly List<SpecificationRange> blockingBands = new List<SpecificationRange>();

        private const int fieldSize = 901;
        private const int minWavelength = 200;  // nm
        private const int maxWavelength = 1100; // nm

    }
}
