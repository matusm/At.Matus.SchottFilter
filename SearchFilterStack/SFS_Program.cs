using At.Matus.SchottFilter;
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
            string outFilename = "SP_Hoya_01.prn";
            string title = "Shortpass filter for array spectrometer";
            string workingDirectory = @"C:\Users\User\source\repos\At.Matus.SchottFilter\catalogs\Hoya";
            ThicknessField dRange = new ThicknessField(1, 3, 1);
            FilterSpecification spec = new FilterSpecification();
            spec.SetPassRange(345, 438, 0.55);
            spec.SetBlockingRange(525, 1100, 0.01);
            #endregion

            SchottFilter[] catalog = LoadFilters(workingDirectory);

            Console.WriteLine(HeaderText());

            // single filter
            Console.WriteLine("Single filter");
            ResultPod[] singleFilter = Try1Filter(catalog, spec, dRange);
            Console.WriteLine(ResultText(singleFilter));

            // two filters
            Console.WriteLine("Two filters");
            ResultPod[] twoFilters = Try2Filters(catalog, spec, dRange);
            Console.WriteLine(ResultText(twoFilters));

            // three filters
            Console.WriteLine("Three filters");
            ResultPod[] threeFilters = Try3Filters(catalog, spec, dRange);
            Console.WriteLine(ResultText(threeFilters));

            // four filters
            Console.WriteLine("Four filters");
            ResultPod[] fourFilters = Try4Filters(catalog, spec, dRange);
            Console.WriteLine(ResultText(fourFilters));

            Console.WriteLine($"Found filter combinations: {singleFilter.Length + twoFilters.Length + threeFilters.Length + fourFilters.Length}");
            Console.WriteLine();
            
            //===========================================//
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
            //===========================================//
            string ResultText(ResultPod[] result)
            {
                StringBuilder sb = new StringBuilder();
                if (result.Length == 0)
                    sb.AppendLine("   [---]");
                foreach (var f in result)
                    sb.AppendLine($"   {f}");
                return sb.ToString();
            }
            //===========================================//
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

        static ResultPod[] Try1Filter(SchottFilter[] catalog, FilterSpecification spec, ThicknessField d)
        {
            SchottFilter combination;
            List<ResultPod> cf = new List<ResultPod>();
            for (int i = 0; i < catalog.Length; i++)
            {
                if (d.UseIntrinsic)
                {
                    combination = FilterMath.Combine(catalog[i]);
                    if (spec.Conforms(combination))
                    {
                        cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
                    }
                }
                else
                {
                    for (double d1 = d.MinimumThickness; d1 <= d.MaximumThickness; d1 += d.DeltaThickness)
                    {
                        combination = FilterMath.Combine(catalog[i], d1);
                        if (spec.Conforms(combination))
                        {
                            cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
                        }
                    }
                }
            }
            cf.Sort();
            cf.Reverse();
            return cf.ToArray();
        }

        static ResultPod[] Try2Filters(SchottFilter[] catalog, FilterSpecification spec, ThicknessField d)
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
                        if (spec.Conforms(combination))
                        {
                            cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
                        }

                    }
                    else
                    {
                        for (double d1 = d.MinimumThickness; d1 <= d.MaximumThickness; d1 += d.DeltaThickness)
                        {
                            for (double d2 = d.MinimumThickness; d2 <= d.MaximumThickness; d2 += d.DeltaThickness)
                            {
                                combination = FilterMath.Combine(catalog[i], d1, catalog[j], d2);
                                if (spec.Conforms(combination))
                                {
                                    cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
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

        static ResultPod[] Try3Filters(SchottFilter[] catalog, FilterSpecification spec, ThicknessField d)
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
                            if (spec.Conforms(combination))
                            {
                                cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
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
                                        if (spec.Conforms(combination))
                                        {
                                            cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
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

        static ResultPod[] Try4Filters(SchottFilter[] catalog, FilterSpecification spec, ThicknessField d)
        {
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
                                combination = FilterMath.Combine(catalog[i], catalog[j], catalog[k], catalog[m]);
                                if (spec.Conforms(combination))
                                {
                                    cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
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
                                                combination = FilterMath.Combine(catalog[i], d1, catalog[j], d2, catalog[k], d3, catalog[m], d4);
                                                if (spec.Conforms(combination))
                                                {
                                                    cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
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
