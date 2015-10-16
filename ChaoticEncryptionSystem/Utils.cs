using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaoticEncryption
{
    class Utils
    {
        public static double Ctan(double x)
        {
            return (1 / Math.Tan(x));
        }

        /// <summary>
        /// X(n+1) = r * X(n) * (1 - X(n))
        /// </summary>
        /// <param name="x_n">Within (0, 1)</param>
        /// <param name="r">Within [3.57, 4]</param>
        /// <returns></returns>
        public static double LogisticMap(double x_n, double r)
        {
            return r * x_n * (1 - x_n);
        }

        /// <summary>
        /// X(n+1) = u * X(n) if x(n) is less than 0.5.
        ///         u * (1 - X(n)) otherwise.
        /// </summary>
        /// <param name="x_n">Within (0, 1)</param>
        /// <param name="u">[sqrt(2), 2]</param>
        /// <returns></returns>
        public static double TentMap(double x_n, double u)
        {
            return x_n < 0.5 ? u * x_n : u * (1 - x_n);
        }

        /// <summary>
        /// X(n+1) = a * sin(PI * X(n)
        /// </summary>
        /// <param name="x_n">(0, 1)</param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double SineMap(double x_n, double a)
        {
            return a * Math.Sin(x_n * Math.PI);
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T tmp = lhs;
            lhs = rhs;
            rhs = tmp;
        }
    }
}
