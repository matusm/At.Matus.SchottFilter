//****************************************************************************************
//
// Quick and dirty!
// Command line app to convert CSV files to the legacy Schott filter transmission format.
//
// Usage:
// 1.) Extract the respective filter data from the Schott catalogue
//     (SCHOTT-filter-glass_2024-2-en.xlsx)
//     https://www.schott.com/en-gb/products/optical-filter-glass-p1000266/downloads
// 2.) Save this data (2 columns + matadata) as a separate EXCEL file with the
//     filter designation as filename, e.g. BG23.xlsx
// 3.) Batch convert these EXCEL files in csv format using the following tool:
//     http://stackoverflow.com/a/11252731/715608
//     https://gist.github.com/tonyerskine/77250575b166bec997f33a679a0dfbe4
// 4.) Put these csv files to a working directory and execute this app.
//     The resulting *.tra files can be used by the SchotFilter class
// 
// The csv file format structure is hardwired in the code.
// Future releases of the Schott catalogue may require code change.
// 
// Author: Michael Matus, 2024
//
//****************************************************************************************

using System;
using System.Globalization;
using System.IO;

namespace FileShaper
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            // the working directory must be hard coded!
            string workingDirectory = @"C:\Users\User\OneDrive\temp";
            string[] filenames = Directory.GetFiles(workingDirectory, @"*.csv");
            Array.Sort(filenames);

            foreach (string filename in filenames)
            {
                string baseFilename = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filename)); // BG14.xlsx.csv => BG14
                string outFilename = Path.ChangeExtension(baseFilename, ".tra");
                string outputPath = Path.Combine(workingDirectory, outFilename);

                FilterData data = LoadCsvFile(filename);
                WriteTraFile(data, outputPath);
                Console.WriteLine(outFilename);
            }
        }

        /********************************************************************************************************/

        private static void WriteTraFile(FilterData data, string outputPath)
        {
            using(StreamWriter outFile = new StreamWriter(outputPath, false))
            {
                outFile.WriteLine($"{data.ReferenceThickness:F3}");
                outFile.WriteLine($"{data.ReflectionFactor}");
                foreach (var v in data.SpectralTransmission)
                {
                    outFile.WriteLine($"{v.X:F0}  {v.Y}");
                }
            }
        }

        /********************************************************************************************************/

        private static FilterData LoadCsvFile(string filename)
        {
            FilterData data = new FilterData();
            using (StreamReader srCsvFile = File.OpenText(filename))
            {
                int lineIndex = 0;
                string csvLine;
                while ((csvLine = srCsvFile.ReadLine()) != null)
                {
                    lineIndex++;
                    if (lineIndex == 6) data.ReferenceThickness = ParseCsvLine(csvLine).Y;
                    if (lineIndex == 5) data.ReflectionFactor = ParseCsvLine(csvLine).Y;
                    if (lineIndex > 8 && lineIndex < 910)
                        data.AddValue(ParseCsvLine(csvLine));
                }
            }
            return data;
        }

        /********************************************************************************************************/

        private static TraValue ParseCsvLine(string line)
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
