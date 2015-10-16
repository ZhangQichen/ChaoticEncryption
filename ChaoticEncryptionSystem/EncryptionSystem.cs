using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChaoticEncryption
{
    public class EncryptionSystem
    {
        protected ValueDistortion m_ValueDistortionSystem = null;
        protected PositionDistortion m_PositionDistortionSystem = null;

        protected static Byte[] mDefaultParameters;
        
        protected EncryptionSystem()
        { }

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
            get { return 0.566; }
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
            List<Byte> decodingParameters = new List<byte>();

            decodingParameters.AddRange(BitConverter.GetBytes(DefaultX0_WheelSwitch));
            decodingParameters.AddRange(BitConverter.GetBytes(DefaultR_WheelSwitch));

            m_Ke.AddRange(BitConverter.GetBytes(UInt32.MaxValue));
            m_Ke.AddRange(BitConverter.GetBytes(UInt32.MaxValue));
            m_Ke.AddRange(BitConverter.GetBytes(UInt32.MaxValue));
            m_Ke.AddRange(BitConverter.GetBytes(UInt32.MaxValue));
            m_Ke.AddRange(BitConverter.GetBytes(UInt32.MaxValue));
            m_Ke.AddRange(BitConverter.GetBytes(UInt32.MaxValue));
            m_Ke.AddRange(BitConverter.GetBytes(UInt32.MaxValue));
            m_Ke.AddRange(BitConverter.GetBytes(UInt32.MaxValue));

            decodingParameters.AddRange(DefaultKe_WheelSwicth.ToArray());
            decodingParameters.AddRange(BitConverter.GetBytes(DefaultX0_NCM));
            decodingParameters.AddRange(BitConverter.GetBytes(DefaultA_NCM));
            decodingParameters.AddRange(BitConverter.GetBytes(DefaultB_NCM));
            mDefaultParameters = decodingParameters.ToArray();
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

        /// <summary>
        /// Encode plainText with parameters in bytes.
        /// </summary>
        /// <param name="parameters">
        ///     byteBlock offset = 0 length = 8 type = "double": initial value X0 for Wheel-Switch system
        ///     byteBlock offset = 8 length = 8 type = "double": control parameter 'r' for Wheel-Switch system
        ///     byteBlock offset = 16 length = 32 type = "Byte[32]": encoding key K_e for Wheel-Switch system
        ///     byteBlock offset = 48 length = 8 type = "double": initial value X0 for NCM underlying PositionDistortionSystem
        ///     byteBlock offset = 56 length = 8 type = "double": control parameter 'a' for NCM underlying PositionDistortionSystem
        ///     byteBlock offset = 64 length = 8 type = "double": control parameter 'b' for NCM underlying PositionDistortionSystem
        /// </param>
        /// <returns>Decoding parameters</returns>
        public static Byte[] Encode(Byte[] plainText, Byte[] parameters, out Byte[] cipher)
        {
            // Parse parameters in bytes
            Double X0 = BitConverter.ToDouble(parameters, 0);
            Double r = BitConverter.ToDouble(parameters, 8);
            Byte[] K_e = new Byte[32];
            for (int i = 16; i < 47; ++i)
                K_e[i - 16] = parameters[i];
            Double X0_ncm = BitConverter.ToDouble(parameters, 48);
            Double a = BitConverter.ToDouble(parameters, 56);
            Double b = BitConverter.ToDouble(parameters, 64);
            
            // Construct System
            Byte[] K_d = WheelSwitch.KeyGenerator(K_e, plainText);
            WheelSwitch ws = new WheelSwitch(X0, r, K_d);
            NCM ncm = new NCM(X0_ncm, a, b);
            EncryptionSystem encryptionSystem = new EncryptionSystem();
            encryptionSystem.m_ValueDistortionSystem = new ValueDistortion(ws);
            encryptionSystem.m_PositionDistortionSystem = new PositionDistortion(ncm);

            // Encoding
            cipher = encryptionSystem.DistortValue(plainText);
            cipher = encryptionSystem.DistortPosition(cipher);

            // Generate Decoding parameters
            Byte[] decodingParameters = (Byte[])parameters.Clone();
            for (int i = 16; i < 47; ++i)
                decodingParameters[i] = K_d[i - 16];

            return decodingParameters;
        }

        /// <summary>
        /// Decode cipher with parameters in bytes.
        /// </summary>
        /// <param name="parameters">
        ///     byteBlock offset = 0 length = 8 type = "double": initial value X0 for Wheel-Switch system
        ///     byteBlock offset = 8 length = 8 type = "double": control parameter 'r' for Wheel-Switch system
        ///     byteBlock offset = 16 length = 32 type = "Byte[32]": decoding key K_d for Wheel-Switch system
        ///     byteBlock offset = 48 length = 8 type = "double": initial value X0 for NCM underlying PositionDistortionSystem
        ///     byteBlock offset = 56 length = 8 type = "double": control parameter 'a' for NCM underlying PositionDistortionSystem
        ///     byteBlock offset = 64 length = 8 type = "double": control parameter 'b' for NCM underlying PositionDistortionSystem
        /// </param>
        public static void Decode(Byte[] cipher, Byte[] parameters, out Byte[] plainText)
        {
            // Parse parameters in bytes
            Double X0 = BitConverter.ToDouble(parameters, 0);
            Double r = BitConverter.ToDouble(parameters, 8);
            Byte[] K_d = new Byte[32];
            for (int i = 16; i < 47; ++i)
                K_d[i - 16] = parameters[i];
            Double X0_ncm = BitConverter.ToDouble(parameters, 48);
            Double a = BitConverter.ToDouble(parameters, 56);
            Double b = BitConverter.ToDouble(parameters, 64);

            // Construct System
            WheelSwitch ws = new WheelSwitch(X0, r, K_d);
            NCM ncm = new NCM(X0_ncm, a, b);
            EncryptionSystem encryptionSystem = new EncryptionSystem();
            encryptionSystem.m_ValueDistortionSystem = new ValueDistortion(ws);
            encryptionSystem.m_PositionDistortionSystem = new PositionDistortion(ncm);

            // Decoding
            plainText = encryptionSystem.RestorePosition(cipher);
            plainText = encryptionSystem.RestoreValue(plainText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="cipher"></param>
        /// <returns>Decoding parameters in bytes</returns>
        public static Byte[] EncodeWithDefaultParamters(Byte[] plainText, out Byte[] cipher)
        {
            return Encode(plainText, mDefaultParameters, out cipher);
        }
        /*
        public static void Main()
        {
            Byte[] plainTextInBytes;
            Byte[] cipherInBytes;
            Byte[] decodingKey;
            Byte[] buffer = new Byte[1024];
            FileStream fs = new FileStream("testInput.txt", FileMode.Open);
            BinaryReader sr = new BinaryReader(fs);
            List<Byte> list = new List<byte>();
            while (sr.Read(buffer, 0, 1024) > 0)
            {
                list.AddRange(buffer);
            }
            sr.Close();
            fs.Close();
            
            plainTextInBytes = list.ToArray();
            
            decodingKey = EncryptionSystem.EncodeWithDefaultParamters(plainTextInBytes, out cipherInBytes);

            fs = new FileStream("EncodedFile.txt", FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(cipherInBytes);
            bw.Close();
            fs.Close();
            
            EncryptionSystem.Decode(cipherInBytes, decodingKey, out plainTextInBytes);
            fs = new FileStream("DecodedFile.txt", FileMode.OpenOrCreate);
            bw = new BinaryWriter(fs);
            bw.Write(plainTextInBytes);
            bw.Close();
            fs.Close();
        }*/
    }
}
