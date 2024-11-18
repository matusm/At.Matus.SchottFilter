using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace At.Matus.SchottFilter
{
    public class SchottFilter
    {
        public string Designation { get; internal set; }
        public double ReferenceThickness { get; internal set; } = double.NaN; // in mm
        public double Thickness => ReferenceThickness; // in mm
        public double ReflectionFactor { get; internal set; } = double.NaN;

        public SchottFilter(string fromFilename)
        {
            Designation = GetDesignationFromFilename(fromFilename);
            LoadTraFile(fromFilename);
        }

        public SchottFilter() 
        {
            Designation = "null";
            ReferenceThickness = 1; // or 0?
            //Array.Fill(internalSpectralTransmittance, 1);
            for (int i = 0; i < internalSpectralTransmittance.Length; i++)
            {
                internalSpectralTransmittance[i] = 1;
            }
        }

        public override string ToString() => $"{Designation}  d: {ReferenceThickness:F3} mm  Rho: {ReflectionFactor:F5}";

        public double GetInternalTransmittance(int lambda)
        {
            if (lambda < 200 || lambda > 1100) return double.NaN;
            return internalSpectralTransmittance[lambda - 200];
        }

        public double GetInternalTransmittance(int lambda, double thickness)
        {
            double tau = GetInternalTransmittance(lambda);
            if (tau == 1) return 1; // to speed things up
            return Math.Pow(tau, (thickness / ReferenceThickness));
        }

        public double GetRegularTransmittance(int lambda, double thickness) => ReflectionFactor * GetInternalTransmittance(lambda, thickness);

        public string ToDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{ReferenceThickness:F3}"); // the thickness in mm, resolution 1 µm
            sb.AppendLine($"{ReflectionFactor:F5}");   // the (combined) reflection factor
            for (int i = 0; i < internalSpectralTransmittance.Length; i++)
                sb.AppendLine($"{i + 200,4} {internalSpectralTransmittance[i]:F8}");
            return sb.ToString();
        }

        private void LoadTraFile(string filename)
        {
            using (StreamReader hTraFile = File.OpenText(filename))
            {
                int lineIndex = 0;
                string traLine;
                while ((traLine = hTraFile.ReadLine()) != null)
                {
                    lineIndex++;
                    if (lineIndex == 1) ReferenceThickness = ParseTraLine(traLine);
                    if (lineIndex == 2) ReflectionFactor = ParseTraLine(traLine);
                    if (lineIndex > 2 && lineIndex < 904)
                        internalSpectralTransmittance[lineIndex - 3] = ParseTraLine(traLine);
                }
            }
        }

        private string GetDesignationFromFilename(string filename)
        {
            string designation = Path.GetFileNameWithoutExtension(Path.GetFileName(filename)).ToUpper();
            return designation;
        }

        private double ParseTraLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return double.NaN;
            string[] separators = { " ", "\t" };
            string[] tokens = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length > 2) return double.NaN;
            string token = tokens.Last();
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double value)) return value;
            return double.NaN;
        }

        internal double[] internalSpectralTransmittance = new double[901]; // for the old fashioned Schott data files (200 nm - 1100 nm)
    }
}

