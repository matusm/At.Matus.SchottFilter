using At.Matus.SchottFilter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SearchFilterStack
{
    class Program
    {
        static double minD = 1;
        static double maxD = 3;
        static double deltaD = 1;

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string workingDirectory = @"C:\Users\User\source\repos\At.Matus.SchottFilter\data";
            //string workingDirectory = @"/Volumes/NO NAME/LaserModData/S01/T020S01.csv";
            //string workingDirectory = Directory.GetCurrentDirectory();

            SchottFilter[] collection = LoadFilters(workingDirectory);

            FilterSpecification spec = new FilterSpecification();
            spec.SetMinimumPermissibleSpectralTransmittance(350, 550, 0.30);
            spec.SetMaximumPermissibleSpectralTransmittance(700, 1100, 0.01);

            // single filter
            ResultPod[] singleFilter = Try1Filter(collection, spec);
            foreach (var f in singleFilter)
            {
                Console.WriteLine($"{f}");
            }
            if (singleFilter.Length == 0) Console.WriteLine("Specification not met with a single filter");
            Console.WriteLine();

            // two filters
            ResultPod[] twoFilters = Try2Filters(collection, spec);
            foreach (var f in twoFilters)
            {
                Console.WriteLine($"{f}");
            }
            if (twoFilters.Length == 0) Console.WriteLine("Specification not met with two filters");
            Console.WriteLine();

            // three filters
            ResultPod[] threeFilters = Try3Filters(collection, spec);
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

        static ResultPod[] Try1Filter(SchottFilter[] filterSet, FilterSpecification spec)
        {
            List<ResultPod> cf = new List<ResultPod>();
            for (int i = 0; i < filterSet.Length; i++)
            {
                for (double d1 = minD; d1 <= maxD; d1 += deltaD)
                {
                    var combination = FilterMath.Combine(filterSet[i], d1);
                    if (spec.Conforms(combination))
                    {
                        cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
                    }
                }
            }
            cf.Sort();
            cf.Reverse();
            return cf.ToArray();
        }

        static ResultPod[] Try2Filters(SchottFilter[] filterSet, FilterSpecification spec)
        {
            List<ResultPod> cf = new List<ResultPod>();
            for (int i = 0; i < filterSet.Length; i++)
            {
                for (int j = i + 1; j < filterSet.Length; j++)
                {
                    for (double d1 = minD; d1 <= maxD; d1 += deltaD)
                    {
                        for (double d2 = minD; d2 <= maxD; d2 += deltaD)
                        {
                            var combination = FilterMath.Combine(filterSet[i], d1, filterSet[j], d2);
                            if (spec.Conforms(combination))
                            {
                                cf.Add(new ResultPod(combination, spec.Fitness(combination), spec.AverageTransmission(combination), spec.AverageBlocking(combination)));
                            }
                        }
                    }
                }
            }
            cf.Sort();
            cf.Reverse();
            return cf.ToArray();
        }

        static ResultPod[] Try3Filters(SchottFilter[] filterSet, FilterSpecification spec)
        {
            List<ResultPod> cf = new List<ResultPod>();
            for (int i = 0; i < filterSet.Length; i++)
            {
                for (int j = i + 1; j < filterSet.Length; j++)
                {
                    for (int k = j + 1; k < filterSet.Length; k++)
                    {
                        for (double d1 = minD; d1 <= maxD; d1 += deltaD)
                        {
                            for (double d2 = minD; d2 <= maxD; d2 += deltaD)
                            {
                                for (double d3 = minD; d3 < maxD; d3 += deltaD)
                                {
                                    var combination = FilterMath.Combine(filterSet[i], d1, filterSet[j], d2, filterSet[k], d3);
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
            cf.Sort();
            cf.Reverse();
            return cf.ToArray();
        }

    }

}
