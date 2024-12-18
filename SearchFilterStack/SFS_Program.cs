﻿using At.Matus.SchottFilter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SearchFilterStack
{
    class SFS_Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            #region Set parameters for analysis

            string outFilename = "Thorlabs_Bpass_ADblock.prn";
            string title = "Bandpass filter for array spectrometer";
            string workingDirectory = @"C:\Users\User\source\repos\At.Matus.SchottFilter\catalogs\Thorlabs";
            ThicknessField dRange = new ThicknessField(FieldType.Intrinsic);
            FilterSpecification spec = new FilterSpecification();
            spec.SetPassRange(438, 525, 0.40);
            spec.SetBlockingRange(340, 400, 0.01);
            spec.SetBlockingRange(612, 1100, 0.01);

            #endregion

            StreamWriter streamWriter = new StreamWriter(outFilename, false);
            SchottFilter[] catalog = LoadFilters(workingDirectory);

            LogAndDisplay(HeaderText());

            // single filter
            LogAndDisplay("Single filter");
            ResultPod[] singleFilter = Try1Filter(catalog, spec, dRange);
            LogAndDisplay(ResultText(singleFilter));

            // two filters
            LogAndDisplay("Two filters");
            ResultPod[] twoFilters = Try2Filters(catalog, spec, dRange);
            LogAndDisplay(ResultText(twoFilters));

            // three filters
            LogAndDisplay("Three filters");
            ResultPod[] threeFilters = Try3Filters(catalog, spec, dRange);
            LogAndDisplay(ResultText(threeFilters));

            // four filters
            LogAndDisplay("Four filters");
            ResultPod[] fourFilters = Try4Filters(catalog, spec, dRange);
            LogAndDisplay(ResultText(fourFilters));

            LogAndDisplay($"Found filter combinations: {singleFilter.Length + twoFilters.Length + threeFilters.Length + fourFilters.Length}");
            Console.WriteLine();
            streamWriter.Close();

            /***************************************************/
            string HeaderText()
            {
                StringBuilder sb = new StringBuilder();
                string AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                string AppVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                sb.AppendLine($"{AppName} version {AppVer}");
                sb.AppendLine($"Title: {title}");
                sb.AppendLine($"Result file: {outFilename}");
                sb.AppendLine($"Filter catalog: {workingDirectory}");
                sb.AppendLine($"Number of filters: {catalog.Length}");
                if (dRange.Type == FieldType.Intrinsic)
                {
                    sb.AppendLine($"Thickness probing range: Catalog values.");
                }
                else
                {
                    sb.AppendLine($"Thickness probing range: {dRange.MinimumThickness} to {dRange.MaximumThickness} @ {dRange.DeltaThickness} mm");
                }
                sb.AppendLine($"Specification");
                foreach (SpecificationRange r in spec.PassBands)
                {
                    sb.AppendLine($"   pass band      {r}");
                }
                foreach (SpecificationRange r in spec.BlockingBands)
                {
                    sb.AppendLine($"   blocking band  {r}");
                }
                sb.AppendLine();
                return sb.ToString();
            }
            /***************************************************/
            string ResultText(ResultPod[] result)
            {
                StringBuilder sb = new StringBuilder();
                if (result.Length == 0)
                    sb.AppendLine("   [---]");
                foreach (var f in result)
                    sb.AppendLine($"   {f}");
                return sb.ToString();
            }
            /***************************************************/
            void LogAndDisplay(string line)
            {
                DisplayOnly(line);
                LogOnly(line);
            }
            /***************************************************/
            void LogOnly(string line)
            {
                streamWriter.WriteLine(line);
                streamWriter.Flush();
            }
            /***************************************************/
            void DisplayOnly(string line)
            {
                Console.WriteLine(line);
            }
            /***************************************************/
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

        static ResultPod[] Try1Filter(SchottFilter[] catalog, FilterSpecification specs, ThicknessField d)
        {
            SchottFilter combination;
            List<ResultPod> cf = new List<ResultPod>();
            for (int i = 0; i < catalog.Length; i++)
            {
                if (d.UseIntrinsic)
                {
                    combination = FilterMath.Combine(catalog[i]);
                    if (specs.Conforms(combination))
                    {
                        cf.Add(new ResultPod(combination, specs));
                    }
                }
                else
                {
                    for (double d1 = d.MinimumThickness; d1 <= d.MaximumThickness; d1 += d.DeltaThickness)
                    {
                        combination = FilterMath.Combine(catalog[i], d1);
                        if (specs.Conforms(combination))
                        {
                            cf.Add(new ResultPod(combination, specs));
                        }
                    }
                }
            }
            cf.Sort();
            cf.Reverse();
            return cf.ToArray();
        }

        static ResultPod[] Try2Filters(SchottFilter[] catalog, FilterSpecification specs, ThicknessField d)
        {
            SchottFilter combination;
            List<ResultPod> cf = new List<ResultPod>();
            for (int i = 0; i < catalog.Length; i++)
            {
                for (int j = i + 1; j < catalog.Length; j++)
                {
                    if (d.UseIntrinsic)
                    {
                        combination = FilterMath.Combine(catalog[i], catalog[j]);
                        if (specs.Conforms(combination))
                        {
                            cf.Add(new ResultPod(combination, specs));
                        }

                    }
                    else
                    {
                        for (double d1 = d.MinimumThickness; d1 <= d.MaximumThickness; d1 += d.DeltaThickness)
                        {
                            for (double d2 = d.MinimumThickness; d2 <= d.MaximumThickness; d2 += d.DeltaThickness)
                            {
                                combination = FilterMath.Combine(catalog[i], d1, catalog[j], d2);
                                if (specs.Conforms(combination))
                                {
                                    cf.Add(new ResultPod(combination, specs));
                                }
                            }
                        }
                    }
                }
            }
            cf.Sort();
            cf.Reverse();
            return cf.ToArray();
        }

        static ResultPod[] Try3Filters(SchottFilter[] catalog, FilterSpecification specs, ThicknessField d)
        {
            SchottFilter combination;
            List<ResultPod> cf = new List<ResultPod>();
            for (int i = 0; i < catalog.Length; i++)
            {
                for (int j = i + 1; j < catalog.Length; j++)
                {
                    for (int k = j + 1; k < catalog.Length; k++)
                    {
                        if (d.UseIntrinsic)
                        {
                            combination = FilterMath.Combine(catalog[i], catalog[j], catalog[k]);
                            if (specs.Conforms(combination))
                            {
                                cf.Add(new ResultPod(combination, specs));
                            }
                        }
                        else
                        {
                            for (double d1 = d.MinimumThickness; d1 <= d.MaximumThickness; d1 += d.DeltaThickness)
                            {
                                for (double d2 = d.MinimumThickness; d2 <= d.MaximumThickness; d2 += d.DeltaThickness)
                                {
                                    for (double d3 = d.MinimumThickness; d3 <= d.MaximumThickness; d3 += d.DeltaThickness)
                                    {
                                        combination = FilterMath.Combine(catalog[i], d1, catalog[j], d2, catalog[k], d3);
                                        if (specs.Conforms(combination))
                                        {
                                            cf.Add(new ResultPod(combination, specs));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            cf.Sort();
            cf.Reverse();
            return cf.ToArray();
        }

        static ResultPod[] Try4Filters(SchottFilter[] catalog, FilterSpecification specs, ThicknessField d)
        {
            long iterations = 0;
            SchottFilter combination;
            List<ResultPod> cf = new List<ResultPod>();
            for (int i = 0; i < catalog.Length; i++)
            {
                for (int j = i + 1; j < catalog.Length; j++)
                {
                    for (int k = j + 1; k < catalog.Length; k++)
                    {
                        for (int m = k + 1; m < catalog.Length; m++)
                        {
                            if (d.UseIntrinsic)
                            {
                                iterations++;
                                combination = FilterMath.Combine(catalog[i], catalog[j], catalog[k], catalog[m]);
                                if (specs.Conforms(combination))
                                {
                                    cf.Add(new ResultPod(combination, specs));
                                }
                            }
                            else
                            {
                                for (double d1 = d.MinimumThickness; d1 <= d.MaximumThickness; d1 += d.DeltaThickness)
                                {
                                    for (double d2 = d.MinimumThickness; d2 <= d.MaximumThickness; d2 += d.DeltaThickness)
                                    {
                                        for (double d3 = d.MinimumThickness; d3 <= d.MaximumThickness; d3 += d.DeltaThickness)
                                        {
                                            for (double d4 = d.MinimumThickness; d4 <= d.MaximumThickness; d4 += d.DeltaThickness)
                                            {
                                                iterations++;
                                                combination = FilterMath.Combine(catalog[i], d1, catalog[j], d2, catalog[k], d3, catalog[m], d4);
                                                if (specs.Conforms(combination))
                                                {
                                                    cf.Add(new ResultPod(combination, specs));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            cf.Sort();
            cf.Reverse();
            return cf.ToArray();
        }
    }

}
