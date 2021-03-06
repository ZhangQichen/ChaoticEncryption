﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace ChaoticEncryption
{
    public static class Utils
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

        public static Byte[] StringToBytes(String str)
        {
            List<Byte> list = new List<byte>();
            Byte v = 0;
            bool isByteEnd = false;
            foreach (char c in str)
            {
                if (c.Equals('-')) continue;
                if (c.CompareTo('0') >= 0 && c.CompareTo('9') <= 0)
                {
                    if (!isByteEnd)
                    {
                        v += (Byte)(c - '0');
                        isByteEnd = true;
                    }
                    else
                    {
                        v += (Byte)(16 * ((Byte)(c - '0')));
                        list.Add(v);
                        isByteEnd = false;
                    }
                }
                else if (c.CompareTo('a') >= 0 && c.CompareTo('f') <= 0)
                {
                    if (!isByteEnd)
                    {
                        v += (Byte)(c - 'a' + 10);
                        isByteEnd = true;
                    }
                    else
                    {
                        v += (Byte)(16 * ((Byte)(c - 'a' + 10)));
                        list.Add(v);
                        isByteEnd = false;
                    }
                }
                else if (c.CompareTo('A') >= 0 && c.CompareTo('F') <= 0)
                {
                    if (!isByteEnd)
                    {
                        v += (Byte)(c - 'A' + 10);
                        isByteEnd = true;
                    }
                    else
                    {
                        v += (Byte)(16 * ((Byte)(c - 'A' + 10)));
                        list.Add(v);
                        isByteEnd = false;
                    }
                }
            }
            return list.ToArray();
        }

        public static class IO
        {
            public static void WritePictureInCsvFile(String filename, Bitmap picture)
            {
                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                for (int j = 0; j < picture.Height; ++j)
                {
                    for (int i = 0; i < picture.Width; ++i)
                    {
                        sw.Write(picture.GetPixel(i, j).ToArgb());
                        sw.Write(',');
                    }
                    //sw.Write('\n');
                }
                sw.Close();
                fs.Close();
            }
        }
    }
}