using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaoticEncryption
{
    public abstract class ChaoticSequenceGenerator
    {
        protected double m_x0 = 0.5;
        protected double m_xn;

        /// <summary>
        /// Generate byte sequence.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        abstract public Byte[] GenerateSequence(int length);

        //The remaining part includes constructors and  

        /// <summary>
        /// Initial value X0 for any chaotic function that inherits this class.
        /// </summary>
        public Double InitialValueX0
        {
            set { m_x0 = value; m_Restore(); }
            get { return m_x0; }
        }

        public ChaoticSequenceGenerator() { m_Restore(); }

        public ChaoticSequenceGenerator(double x0)
        {
            m_x0 = x0;
            m_Restore();
        }

        virtual protected void m_Restore()
        {
            m_xn = m_x0;
        }
    }
}
