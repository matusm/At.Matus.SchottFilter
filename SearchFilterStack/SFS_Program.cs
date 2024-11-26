using At.Matus.SchottFilter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SearchFilterStack
{
    class SFS_Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string workingDirectory = @"C:\Users\User\source\repos\At.Matus.SchottFilter\catalogs\Thorlabs";
            SchottFilter[] catalog = LoadFilters(workingDirectory);
            ThicknessField dRange = new ThicknessField(FieldType.Intrinsic);
            FilterSpecification spec = new FilterSpecification();
            spec.SetPassRange(340, 500, 0.30);
            spec.SetBlockingRange(650, 1100, 0.01);
            //spec.SetPassRange(340, 438, 0.50);
            //spec.SetBlockingRange(490, 1100, 0.01);
            //spec.SetPassRange(490, 1100, 0.30);
            //spec.SetBlockingRange(340, 438, 0.01);

            // some diagnostic output
            Console.WriteLine($"Filter catalog: {workingDirectory}");
            Console.WriteLine($"Number of filters: {catalog.Length}");
            if(dRange.Type==FieldType.Intrinsic)
            {
                Console.WriteLine($"Catalog intrinsic filter thicknesses.");
            }
            else
            {
                Console.WriteLine($"Thickness from {dRange.MinimumThickness} to {dRange.MaximumThickness} @ {dRange.DeltaThickness} mm");
            }
            Console.WriteLine($"Specification");
            foreach (SpecificationRange r in spec.PassBands)
            {
                Console.WriteLine($"   pass band      {r}");
            }
            foreach (SpecificationRange r in spec.BlockingBands)
            {
                Console.WriteLine($"   blocking band  {r}");
            }
            Console.WriteLine();

            // single filter
            ResultPod[] singleFilter = Try1Filter(catalog, spec, dRange);
            foreach (var f in singleFilter)
            {
                Console.WriteLine($"{f}");
            }
            if (singleFilter.Length == 0) Console.WriteLine("Specification not met with a single filter");
            Console.WriteLine();

            // two filters
            ResultPod[] twoFilters = Try2Filters(catalog, spec, dRange);
            foreach (var f in twoFilters)
            {
                Console.WriteLine($"{f}");
            }
            if (twoFilters.Length == 0) Console.WriteLine("Specification not met with two filters");
            Console.WriteLine();

            // three filters
            ResultPod[] threeFilters = Try3Filters(catalog, spec, dRange);
            foreach (var f in threeFilters)
            {
                Console.WriteLine($"{f}");
            }
            if (threeFilters.Length == 0) Console.WriteLine("Specification not met with three filters");
            Console.WriteLine();
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

    }

}
