using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FileShaper
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            string workingDirectory = @"C:\Users\User\OneDrive\temp";
            string[] filenames = Directory.GetFiles(workingDirectory, @"*.csv");
            Array.Sort(filenames);
            foreach (string fn in filenames)
            {
                string baseFilename = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fn));
                string outFilename = Path.ChangeExtension(baseFilename, ".tra");
                string outputPath = Path.Combine(workingDirectory, outFilename);

                Data data = LoadCsvFile(fn);
                WriteTraFile(data, outputPath);
                Console.WriteLine(outFilename);

            }
        }

        /********************************************************************************************************/

        static void WriteTraFile(Data data, string outputPath)
        {
            using(StreamWriter outFile = new StreamWriter(outputPath, false))
            {
                outFile.WriteLine($"{data.ReferenceThickness:F3}");
                outFile.WriteLine($"{data.ReflectionFactor}");
                foreach (var v in data.SpectralTransmission)
                {
                    outFile.WriteLine($"{v.Wavelength:F0}  {v.Transmittance}");
                }
            }
        }

        /********************************************************************************************************/

        static Data LoadCsvFile(string filename)
        {
            Data data = new Data();
            using (StreamReader srCsvFile = File.OpenText(filename))
            {
                int lineIndex = 0;
                string csvLine;
                while ((csvLine = srCsvFile.ReadLine()) != null)
                {
                    lineIndex++;
                    if (lineIndex == 6) data.ReferenceThickness = ParseCsvLine(csvLine).Transmittance;
                    if (lineIndex == 5) data.ReflectionFactor = ParseCsvLine(csvLine).Transmittance;
                    if (lineIndex > 8 && lineIndex < 910)
                        data.AddValue(ParseCsvLine(csvLine));
                }
            }
            return data;
        }

        /********************************************************************************************************/

        static TraValue ParseCsvLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) 
                return new TraValue();
            string[] separators = {","};
            string[] tokens = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length > 2) 
                return new TraValue();
            if (!double.TryParse(tokens[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double wl))
                wl = double.NaN;
            if (!double.TryParse(tokens[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double tr))
                tr = double.NaN;
            return new TraValue(wl,tr);
        }

        /********************************************************************************************************/

    }
}
