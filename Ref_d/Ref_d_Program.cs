using At.Matus.SchottFilter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Ref_d
{
    class Ref_d_Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            // the working directory must be hard coded!
            string workingDirectory = @"C:\Users\User\source\repos\At.Matus.SchottFilter\catalogs\Thorlabs";
            SchottFilter[] catalog = LoadFilters(workingDirectory);
            double d = 2.0;

            foreach (SchottFilter filter in catalog)
            {
                string baseFilename = filter.Designation;
                string outFilename = Path.ChangeExtension(baseFilename, ".tra2mm");
                string outputPath = Path.Combine(workingDirectory, outFilename);
                string output = ComposeAdjustedData(filter, d);
                WriteFile(output, outputPath);
                Console.WriteLine($"{baseFilename,20} {filter.Thickness:F2} mm  ->  {d:F2} mm");
            }
        }

        //=========================================================================================//

        private static void WriteFile(string output, string outputPath)
        {
            using (StreamWriter outFile = new StreamWriter(outputPath, false))
            {
                outFile.WriteLine(output);
            }
        }

        //=========================================================================================//

        private static string ComposeAdjustedData(SchottFilter filter, double d)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{d:F3}"); // the thickness in mm, resolution 1 µm
            sb.AppendLine($"{filter.ReflectionFactor:F5}");   // the (combined) reflection factor
            for (int lambda = 200; lambda <= 1100; lambda++)
            {
                sb.AppendLine($"{lambda,4} {filter.GetInternalTransmittance(lambda, d):F7}");
            }
            return sb.ToString();
        }

        //=========================================================================================//

        static SchottFilter[] LoadFilters(string fromDirectory)
        {
            List<SchottFilter> filters = new List<SchottFilter>();
            string[] filenames = Directory.GetFiles(fromDirectory, @"*.tra");
            Array.Sort(filenames);
            foreach (string fn in filenames)
            {
                filters.Add(new SchottFilter(fn));
            }
            return filters.ToArray();
        }
    }
}
