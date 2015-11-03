using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace ChaoticEncryption
{
    static class SystemAnalysis
    {
        public static void AlgoAnalysis()
        {
            String KeStr = "CHAOTICENCRYPTIONSYSTEMBYZHANGQC";
            Byte[] Ke = Encoding.Default.GetBytes(KeStr);
            Double r = EncryptionSystem.DefaultR_WheelSwitch;
            Double A = EncryptionSystem.DefaultA_NCM;
            Double B = EncryptionSystem.DefaultB_NCM;
            Double X0_ncm = EncryptionSystem.DefaultX0_NCM;
            Double X0_ws = EncryptionSystem.DefaultX0_WheelSwitch;
            Byte[] P;
            FileStream pfs = new FileStream("PlainText.txt", FileMode.Open);
            BinaryReader sr = new BinaryReader(pfs);
            List<Byte> ptL = new List<byte>();
            Byte[] buffer = new Byte[1];
            while (sr.Read(buffer, 0, 1) > 0)
            {
                ptL.AddRange(buffer);
            }
            P = ptL.ToArray();

            // Write plain text
            FileStream fs = new FileStream("KeySensitivityAnalysis.csv", FileMode.OpenOrCreate);
            StreamWriter SW = new StreamWriter(fs);
            SW.Write("PlainText,");
            foreach (Byte b in P)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');

            List<Byte> parameters = new List<byte>();
            parameters.AddRange(BitConverter.GetBytes(X0_ws));
            parameters.AddRange(BitConverter.GetBytes(r));
            parameters.AddRange(Ke);
            parameters.AddRange(BitConverter.GetBytes(X0_ncm));
            parameters.AddRange(BitConverter.GetBytes(A));
            parameters.AddRange(BitConverter.GetBytes(B));
            Byte[] ParaInBytes = parameters.ToArray();
            Byte[] cipher;
            Byte[] ParaDecInBytes = EncryptionSystem.Encode(P, ParaInBytes, out cipher);
            EncryptionSystem.Decode(cipher, ParaDecInBytes, out P);

            // Write cipher
            SW.Write("Cipher,");
            foreach (Byte b in cipher)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');

            // Write correctly decoded text
            SW.Write("CorrectDecodedText,");
            foreach (Byte b in P)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');

            // Sensitivity Analysis
            // r GOOD!
            Double changedR = r - 1e-6;
            Byte[] changedRInBytes = BitConverter.GetBytes(changedR);
            Byte[] paraForR = (Byte[])ParaDecInBytes.Clone();
            for (int i = 0, j = 8; i < changedRInBytes.Length; ++i, ++j)
            {
                paraForR[j] = changedRInBytes[i];
            }
            EncryptionSystem.Decode(cipher, paraForR, out P);
            // Write decoded text
            SW.Write("r changed by 10^(-6),");
            foreach (Byte b in P)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');

            //X0_ncm GOOD!
            Double X0_ncm2 = X0_ncm - 1e-15;
            Byte[] changedX0_ncmInBytes = BitConverter.GetBytes(X0_ncm2);
            Byte[] paraForX0ncm = (Byte[])ParaDecInBytes.Clone();
            for (int i = 0, j = 48; i < changedX0_ncmInBytes.Length; ++i, ++j)
            {
                paraForX0ncm[j] = changedX0_ncmInBytes[i];
            }
            EncryptionSystem.Decode(cipher, paraForX0ncm, out P);
            // Write decoded text
            SW.Write("X0_ncm changed by 10^(-15),");
            foreach (Byte b in P)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');

            //X0_ws GOOD!
            Double X0_ws2 = X0_ws - 1e-7;
            Byte[] changedX0_wsInBytes = BitConverter.GetBytes(X0_ws2);
            Byte[] paraForX0ws = (Byte[])ParaDecInBytes.Clone();
            for (int i = 0, j = 0; i < changedX0_wsInBytes.Length; ++i, ++j)
            {
                paraForX0ws[j] = changedX0_wsInBytes[i];
            }
            EncryptionSystem.Decode(cipher, paraForX0ws, out P);
            // Write decoded text
            SW.Write("X0_ws changed by 10^(-7),");
            foreach (Byte b in P)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');

            //a Good!
            Double changedA = A - 1e-15;
            Byte[] changedAInBytes = BitConverter.GetBytes(changedA);
            Byte[] paraForA = (Byte[])ParaDecInBytes.Clone();
            for (int i = 0, j = 56; i < changedAInBytes.Length; ++i, ++j)
            {
                paraForA[j] = changedAInBytes[i];
            }
            EncryptionSystem.Decode(cipher, paraForA, out P);
            // Write decoded text
            SW.Write("a changed by 10^(-15),");
            foreach (Byte b in P)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');

            //b GOOD!
            Double changedB = B - 1e-14;
            Byte[] changedBInBytes = BitConverter.GetBytes(changedB);
            Byte[] paraForB = (Byte[])ParaDecInBytes.Clone();
            for (int i = 0, j = 64; i < changedBInBytes.Length; ++i, ++j)
            {
                paraForB[j] = changedBInBytes[i];
            }
            EncryptionSystem.Decode(cipher, paraForB, out P);
            // Write decoded text
            SW.Write("b changed by 10^(-14),");
            foreach (Byte b in P)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');

            //Kd
            Byte[] paraForKd = (Byte[])ParaDecInBytes.Clone();
            paraForKd[16 + 16] -= 0x01;
            EncryptionSystem.Decode(cipher, paraForKd, out P);
            // Write decoded text
            SW.Write("Kd,");
            foreach (Byte b in P)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');
            SW.Close();
            fs.Close();
        }

        public static void Histogram()
        {
            FileStream fs = new FileStream("Histogram.csv", FileMode.OpenOrCreate);
            StreamWriter SW = new StreamWriter(fs);
            Byte[] seq = new NCM(0.666, 0.7, 28.00).GenerateSequence(200);
            SW.Write("NCM,");
            foreach (Byte b in seq)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');
            seq = new WheelSwitch().GenerateSequence(200);
            SW.Write("Wheel-Switch,");
            foreach (Byte b in seq)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Close();
            fs.Close();
        }

        public static void Comparison()
        {
            FileStream fs = new FileStream("Comparison.csv", FileMode.OpenOrCreate);
            StreamWriter SW = new StreamWriter(fs);

            ValueDistortion vd = new ValueDistortion(new NCM(0.666, 0.7, 28.00));
            PositionDistortion pd = new PositionDistortion(new WheelSwitch());

            Byte[] plainText = new Byte[200];
            for (int i = 0; i < 200; ++i)
            {
                plainText[i] = 0x00;
            }
            Byte[] intermediateCipher;
            Byte[] cipher;

            vd.Distort(plainText, out intermediateCipher);
            pd.Distort(intermediateCipher, out cipher);

            SW.Write("PlainText,");
            foreach (Byte b in plainText)
            {
                SW.Write(b);
                SW.Write(',');
            }

            SW.Write('\n');
            SW.Write("IntermediateCipher,");
            foreach (Byte b in intermediateCipher)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Write('\n');
            SW.Write("Cipher,");
            foreach (Byte b in cipher)
            {
                SW.Write(b);
                SW.Write(',');
            }
            SW.Close();
            fs.Close();
        }

        public static void EncryptPicture(String filename)
        {
            String fileType = filename.Substring(filename.LastIndexOf('.') + 1);
            Bitmap bitmap = new Bitmap(filename);
            // need to compress or the bitmap will be too huge to analyze.
            int x = bitmap.Width / 50;
            bitmap = (Bitmap)bitmap.GetThumbnailImage(bitmap.Width / x, bitmap.Height / x, () => false, IntPtr.Zero);

            List<Byte> list = new List<Byte>();
            for (int j = 0; j < bitmap.Height; ++j)
                for (int i = 0; i < bitmap.Width; ++i)
                {
                    // Argb is a 32bit value and occupies 4bytes.
                    list.AddRange(BitConverter.GetBytes(bitmap.GetPixel(i, j).ToArgb()));
                }

            Byte[] encodedBytes;
            Byte[] decodingParams;
            Byte[] decodedBytes;

            // Encoding
            decodingParams = EncryptionSystem.EncodeWithDefaultParamters(list.ToArray(), out encodedBytes);
            // Save encrypted bitmap
            Bitmap encryptedBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int j = 0, k = 0; j < bitmap.Height; ++j)
                for (int i = 0; i < bitmap.Width; ++i)
                {
                    encryptedBitmap.SetPixel(i, j, Color.FromArgb(BitConverter.ToInt32(encodedBytes, k)));
                    k += 4;
                }
            encryptedBitmap.Save("encrypted." + fileType);

            // Decoding
            EncryptionSystem.Decode(encodedBytes, decodingParams, out decodedBytes);
            // Save decoded bitmap
            Bitmap decodedBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int j = 0, k = 0; j < bitmap.Height; ++j)
                for (int i = 0; i < bitmap.Width; ++i)
                {
                    decodedBitmap.SetPixel(i, j, Color.FromArgb(BitConverter.ToInt32(decodedBytes, k)));
                    k += 4;
                }
            decodedBitmap.Save("Decoded." + fileType);

            Utils.IO.WritePictureInCsvFile("originalPic.csv", bitmap);
            Utils.IO.WritePictureInCsvFile("encodedPic.csv", encryptedBitmap);
            Utils.IO.WritePictureInCsvFile("decoded.csv", decodedBitmap);
        }

        public static void Main()
        {
            //EncryptPicture("sysu.jpg");
            AlgoAnalysis();
        }
    }
}
