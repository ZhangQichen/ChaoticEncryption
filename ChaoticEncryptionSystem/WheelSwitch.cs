/*
Reference:
	"A Wheel-Switch Chaotic System for Image Encryption"
By
	Yue Wu, Joseph P. Noonan, Department of Electrical and Computer Engineering, Tufts University.
	Sos Agaian, Department of Electrical and Computer Engineering line, University of Texas at San Antonio, San Antonio.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaoticEncryption
{
    class WheelSwitch : ChaoticSequenceGenerator
    {
        private double m_r = 3.585;
        private Byte[] m_Q;
        int m_itr_Q = 0;

        override protected void m_Restore()
        {
            base.m_Restore();
            m_itr_Q = 0;
        }

        /// <summary>
        /// Set 'r' for Logistic/Tent/Sine map.
        /// </summary>
        public Double ControlParameter
        {
            set
            { m_r = value; }
            get
            { return m_r; }
        }

        /// <summary>
        /// Decoding Key or Wheel-Switch sequence Q in encoding.
        /// </summary>
        public Byte[] K_D
        {
            set { m_Q = value; m_Restore(); }
            get { return m_Q; }
        }

        public WheelSwitch() : base()
        {
            m_Q = new Byte[3] { 2, 1, 0 };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_x0">Within (0,1)</param>
        /// <param name="m_r">Within [3.57, 4]</param>
        public WheelSwitch(double x0, double r, Byte[] Q) : base(x0)
        {
            m_r = r;
            m_Q = Q;
            m_Restore();
        }

        public double m_CalculateNextNumber()
        {
            if (m_Q[m_itr_Q] % 3 == 0 )
            {
                return m_xn = Utils.LogisticMap(m_xn, m_r);
            }
            else if (m_Q[m_itr_Q] % 3 == 1)
            {
                return m_xn = Utils.TentMap(m_xn, m_r);
            }
            else
            {
                return m_xn = Utils.SineMap(m_xn, m_r);
            }
        }

        override public Byte[] GenerateSequence(int length)
        {
            List<Byte> sequence = new List<Byte>();
            for (int i = 0; i < length / 4 + 1; ++i)
                sequence.AddRange(BitConverter.GetBytes(m_CalculateNextNumber()));
            m_Restore();
            return sequence.GetRange(0, length).ToArray();
        }

        /// <summary>
        /// KD = KE + sum(Each byte of plainText);
        /// </summary>
        /// <param name="encodingKey">Encoding Key in bytes</param>
        /// <returns>Decoding Key in bytes</returns>
        public static Byte[] KeyGenerator(Byte[] encodingKey, Byte[] plainText)
        {
            int itr_K = 0;
            int length = encodingKey.Length;
            Byte[] decodingKey = (Byte[])encodingKey.Clone();
            foreach(Byte b in plainText)
            {
                decodingKey[(itr_K++) % length] += b;
            }
            return decodingKey;
        }
    }
}
