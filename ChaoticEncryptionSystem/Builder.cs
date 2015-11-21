using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaoticEncryption
{
    public class Builder
    {
        double m_a = 0.7, m_b = 28, m_x0_ncm = 0.666; // para for ncm
        // para for ws
        double m_x0_ws = 0.566, m_r = 3.588;
        Byte[] m_Ke; // 32 Bytes

        public Builder()
        {
        }

        public void SetNcmParams(double a, double b, double x0)
        {
            m_a = a; m_b = b; m_x0_ncm = x0;
        }
        
        public void SetWheelSwitchParams(double r, double x0, Byte[] ke)
        {
            m_r = r; m_x0_ws = x0;
            m_Ke = new Byte[32];
            int i = 0;
            for (; i < 32 && i < ke.Length; ++i)
                m_Ke[i] = ke[i];
            if (i < 32) m_Ke[i] = 0xFF;
        }

        public EncryptionSystem CreateSystem(Byte[] plainText)
        {
            NCM ncm = new NCM(m_x0_ncm, m_a, m_b);
            WheelSwitch ws = new WheelSwitch(m_x0_ws, m_r, WheelSwitch.GenerateDecodingKey(m_Ke, plainText));
            ValueDistortion vd = new ValueDistortion(ws);
            PositionDistortion pd = new PositionDistortion(ncm);
            return new EncryptionSystem(vd, pd);
        }

    }
}
