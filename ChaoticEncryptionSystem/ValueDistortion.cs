using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaoticEncryption
{
    public class ValueDistortion
    {
        //
        private ChaoticSequenceGenerator m_SeqGenerator;

        private Byte[] m_GenerateDistortionSequence(int length)
        {
            Byte[] byteSeq = m_SeqGenerator.GenerateSequence(length);
            return byteSeq;
        }

        public ValueDistortion(ChaoticSequenceGenerator generator)
        {
            this.m_SeqGenerator = generator;
        }

        public void Distort(Byte[] plainText, out Byte[] distortedText)
        {
            int length = plainText.Length;
            distortedText = new Byte[length];
            Byte[] distortSequence = m_GenerateDistortionSequence(length);
            for(int i = 0; i < length; ++i)
            {
                distortedText[i] = BitConverter.GetBytes((plainText[i] ^ distortSequence[i])).First();
            }
        }

        public void Restore(Byte[] cipher, out Byte[] plainText)
        {
            int length = cipher.Length;
            plainText = new Byte[length];
            Byte[] distortSequence = m_GenerateDistortionSequence(length);
            for (int i = 0; i < length; ++i)
            {
                plainText[i] = BitConverter.GetBytes((cipher[i] ^ distortSequence[i])).First();
            }
        }
    }
}
