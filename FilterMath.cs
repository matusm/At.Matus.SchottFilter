﻿using System.Threading.Tasks;

namespace At.Matus.SchottFilter
{
    public static class FilterMath
    {

        public static SchottFilter Combine(SchottFilter f1,
                                           double d1,
                                           SchottFilter f2,
                                           double d2) => Combine(f1, d1, f2, d2, new SchottFilter(), 0, new SchottFilter(), 0, new SchottFilter(), 0);

        public static SchottFilter Combine(SchottFilter f1,
                                           double d1,
                                           SchottFilter f2,
                                           double d2,
                                           SchottFilter f3,
                                           double d3) => Combine(f1, d1, f2, d2, f3, d3, new SchottFilter(), 0, new SchottFilter(), 0);

        public static SchottFilter Combine(SchottFilter f1,
                                           double d1,
                                           SchottFilter f2,
                                           double d2,
                                           SchottFilter f3,
                                           double d3,
                                           SchottFilter f4,
                                           double d4) => Combine(f1, d1, f2, d2, f3, d3, f4, d4, new SchottFilter(), 0);

        public static SchottFilter Combine(SchottFilter f1,
                                           double d1,
                                           SchottFilter f2,
                                           double d2,
                                           SchottFilter f3,
                                           double d3,
                                           SchottFilter f4,
                                           double d4,
                                           SchottFilter f5,
                                           double d5)
        {
            SchottFilter filter = new SchottFilter();
            Parallel.For(200, 1101, i =>
            {
                filter.internalSpectralTransmittance[i - 200] = f1.GetInternalTransmittance(i, d1) * f2.GetInternalTransmittance(i, d2) * f3.GetInternalTransmittance(i, d3) * f4.GetInternalTransmittance(i, d4) * f5.GetInternalTransmittance(i, d5);
            }
            );
            filter.Designation = $"{f1.Designation} : {d1:F3} mm + {f2.Designation} : {d2:F3} mm + {f3.Designation} : {d3:F3} mm + {f4.Designation} : {d4:F3} mm + {f5.Designation} : {d5:F3} mm";
            filter.ReferenceThickness = d1 + d2 + d3 + d4 + d5; // this is actually not a _reference_ thickness
            filter.ReflectionFactor = f1.ReflectionFactor * f2.ReflectionFactor * f3.ReflectionFactor * f4.ReflectionFactor * f5.ReflectionFactor; // unverkittet!
            return filter;
        }
    }
}