/*
Reference:
	"A Fast Image Encryption Scheme Based on a Nonlinear Chaotic Map"
By
	Shujiang Xu, Yinglong Wang, Jizhi Wang. Shandong Computer Science Center, Jinan, China.
	Yucui Guo. Beijing University of Posts and Telecommunications, Beijing, China.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaoticEncryption
{
    class NCM : ChaoticSequenceGenerator
    {
        private double m_a = 0.7;
        private double m_b = 28.0;

        public double m_CalculateNextNumber()
        {
            return m_xn =
                (1 - Math.Pow(m_b, -4.0)) * Utils.Ctan(m_a / (1 + m_b)) * Math.Pow(1 + 1 / m_b, m_b) * Math.Tan(m_a * m_xn) * Math.Pow(1 - m_xn, m_b);
        }

        public Double ControlParameterA
        {
            set { m_a = value; }
            get { return m_a; }
        }

        public Double ControlParameterB
        {
            set { m_b = value; }
            get { return m_b; }
        }

        /// <summary>
        /// Construct with default values.
        /// a = 0.7, b = 28.0
        /// </summary>
        public NCM() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x_0">initial value. within(0,1)</param>
        /// <param name="a">control parameter</param>
        /// <param name="b">control parameter</param>
        /// a within (0, 1.4), b within (5, 43) or
	    /// a within (1.4, 1.5), b within (9, 38) or
	    /// a within (1.5, 1.57), b within (3, 15)
        public NCM(double x_0, double a, double b) : base(x_0)
        {
            this.m_a = a;
            this.m_b = b;
        }

        /// <summary>
        /// Generate chaotic sequence in bytes.
        /// </summary>
        /// <param name="length">The length of generated sequence</param>
        /// <returns></returns>
        override public Byte[] GenerateSequence(int length)
        {
            List<Byte> seq = new List<Byte>();
            for (int i = 0; i < length / 4 + 1; ++i)
            {
                seq.AddRange(BitConverter.GetBytes(m_CalculateNextNumber()));
            }
            m_Restore();
            return seq.GetRange(0, length).ToArray();
        }


    }
}
