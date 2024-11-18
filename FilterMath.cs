using System.Threading.Tasks;

namespace At.Matus.SchottFilter
{
    public static class FilterMath
    {
        public static SchottFilter Combine(SchottFilter f1, double d1)
        {
            SchottFilter filter = new SchottFilter();
            Parallel.For(200, 1101, i =>
            {
                filter.internalSpectralTransmittance[i - 200] = f1.GetInternalTransmittance(i, d1);
            }
            );
            filter.Designation = $"{f1.Designation} : {d1:F2} mm";
            filter.ReflectionFactor = f1.ReflectionFactor;
            filter.ReferenceThickness = d1;
            return filter;
        }

        public static SchottFilter Combine(SchottFilter f1,
                                           double d1,
                                           SchottFilter f2,
                                           double d2)
        {
            SchottFilter filter = Combine(f1, d1, f2, d2, new SchottFilter(), 0, new SchottFilter(), 0, new SchottFilter(), 0);
            filter.Designation = $"{f1.Designation} : {d1:F2} mm + {f2.Designation} : {d2:F2} mm";
            filter.ReflectionFactor = (f1.ReflectionFactor + f2.ReflectionFactor) / 2; // verkittet!
            return filter;
        }

        public static SchottFilter Combine(SchottFilter f1,
                                           double d1,
                                           SchottFilter f2,
                                           double d2,
                                           SchottFilter f3,
                                           double d3)
        {
            SchottFilter filter = Combine(f1, d1, f2, d2, f3, d3, new SchottFilter(), 0, new SchottFilter(), 0);
            filter.Designation = $"{f1.Designation} : {d1:F2} mm + {f2.Designation} : {d2:F2} mm + {f3.Designation} : {d3:F2} mm";
            filter.ReflectionFactor = (f1.ReflectionFactor + f2.ReflectionFactor + f3.ReflectionFactor) / 3; // verkittet!
            return filter;
        }

        public static SchottFilter Combine(SchottFilter f1,
                                           double d1,
                                           SchottFilter f2,
                                           double d2,
                                           SchottFilter f3,
                                           double d3,
                                           SchottFilter f4,
                                           double d4)
        {
            SchottFilter filter = Combine(f1, d1, f2, d2, f3, d3, f4, d4, new SchottFilter(), 0);
            filter.Designation = $"{f1.Designation} : {d1:F2} mm + {f2.Designation} : {d2:F2} mm + {f3.Designation} : {d3:F2} mm + {f4.Designation} : {d4:F} mm";
            filter.ReflectionFactor = (f1.ReflectionFactor + f2.ReflectionFactor + f3.ReflectionFactor + f4.ReflectionFactor) / 4; // verkittet!
            return filter;
        }

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
            filter.Designation = $"{f1.Designation} : {d1:F2} mm + {f2.Designation} : {d2:F2} mm + {f3.Designation} : {d3:F2} mm + {f4.Designation} : {d4:F2} mm + {f5.Designation} : {d5:F2} mm";
            filter.ReferenceThickness = d1 + d2 + d3 + d4 + d5; // this is actually not a _reference_ thickness
            filter.ReflectionFactor = (f1.ReflectionFactor + f2.ReflectionFactor + f3.ReflectionFactor + f4.ReflectionFactor + f5.ReflectionFactor) / 5; // verkittet!
            return filter;
        }


        public static SchottFilter Combine(SchottFilter f1, SchottFilter f2) => Combine(f1, f1.Thickness, f2, f2.Thickness);

    }
}
