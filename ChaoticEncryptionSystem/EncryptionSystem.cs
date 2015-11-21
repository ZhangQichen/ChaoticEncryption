using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ChaoticEncryption
{
    public class EncryptionSystem
    {
        protected ValueDistortion m_ValueDistortionSystem = null;

        protected PositionDistortion m_PositionDistortionSystem = null;
        
        public EncryptionSystem()
        {
            m_ValueDistortionSystem = new ValueDistortion(new NCM());
            m_PositionDistortionSystem = new PositionDistortion(new WheelSwitch());
        }

        public EncryptionSystem(ValueDistortion vd, PositionDistortion pd)
        {
            m_ValueDistortionSystem = vd;
            m_PositionDistortionSystem = pd;
        }
        
        public static Double DefaultX0_WheelSwitch
        {
            get { return 0.566; }
        }

        public static Double DefaultR_WheelSwitch
        {
            get { return 3.588; }
        }

        protected static List<Byte> m_Ke = new List<byte>();
        public static Byte[] DefaultKe_WheelSwicth
        {
            get { return m_Ke.ToArray(); }
        }

        public static Double DefaultX0_NCM
        {
            get { return 0.666; }
        }

        public static Double DefaultA_NCM
        {
            get { return 0.7; }
        }

        public static Double DefaultB_NCM
        {
            get { return 28.00; }
        }

        static EncryptionSystem()
        {
            m_Ke.AddRange(Encoding.UTF8.GetBytes("ChaosEncryptionSysNcmWheelSwitch"));
        }

        public Byte[] DistortValue(Byte[] plainText)
        {
            Byte[] distortedText;
            m_ValueDistortionSystem.Distort(plainText, out distortedText);
            return distortedText;
        }

        public Byte[] DistortPosition(Byte[] plainText)
        {
            Byte[] distortedText;
            m_PositionDistortionSystem.Distort(plainText, out distortedText);
            return distortedText;
        }

        public void Encrypt(ref Byte[] plainText, out Byte[] cipher)
        {
            cipher = DistortValue(plainText);
            cipher = DistortPosition(cipher);
        }

        public void Decrypt(ref Byte[] cipher, out Byte[] plainText)
        {
            plainText = RestorePosition(cipher);
            plainText = RestoreValue(plainText);
        }

        public Byte[] RestoreValue(Byte[] cipher)
        {
            Byte[] plainText;
            m_ValueDistortionSystem.Restore(cipher, out plainText);
            return plainText;
        }

        public Byte[] RestorePosition(Byte[] cipher)
        {
            Byte[] plainText;
            m_PositionDistortionSystem.Restore(cipher, out plainText);
            return plainText;
        }
    }
}
