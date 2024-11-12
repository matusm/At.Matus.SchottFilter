namespace At.Matus.SchottFilter
{
    public class FilterSpecification
    {
        private const int fieldSize = 901;
        private const int minWavelength = 200;  // nm
        private const int maxWavelength = 1100; // nm

        public FilterSpecification()
        {
            _SetMinimumPermissibleSpectralTransmittance(0, fieldSize, 0);
            _SetMaximumPermissibleSpectralTransmittance(0, fieldSize, 1);
        }

        public void SetMinimumPermissibleSpectralTransmittance(int lowerWavelength, int upperWavelength, double tau) => _SetMinimumPermissibleSpectralTransmittance(lowerWavelength - minWavelength, upperWavelength - minWavelength, tau);

        public void SetMaximumPermissibleSpectralTransmittance(int lowerWavelength, int upperWavelength, double tau) => _SetMaximumPermissibleSpectralTransmittance(lowerWavelength - minWavelength, upperWavelength - minWavelength, tau);

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
            double fitness = AverageTransmission(filter) / AverageBlocking(filter);
            return fitness;
        }

        public double AverageTransmission(SchottFilter filter)
        {
            double sumTau = 0;
            int count = 0;
            for (int i = 0; i < fieldSize; i++)
            {
                double tau = filter.GetInternalTransmittance(i + minWavelength);
                if (minimumPermissibleSpectralTransmittance[i] != 0)
                {
                    sumTau += tau;
                    count++;
                }
            }
            return sumTau / count;
        }

        public double AverageBlocking(SchottFilter filter)
        {
            double sumTau = 0;
            int count = 0;
            for (int i = 0; i < fieldSize; i++)
            {
                double tau = filter.GetInternalTransmittance(i + minWavelength);
                if (maximumPermissibleSpectralTransmittance[i] != 1)
                {
                    sumTau += tau;
                    count++;
                }
            }
            return sumTau / count;
        }

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
    }
}
