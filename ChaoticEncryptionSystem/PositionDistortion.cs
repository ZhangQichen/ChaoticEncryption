using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaoticEncryption
{
    public class PositionDistortion
    {
        private ChaoticSequenceGenerator m_SeqGenerator;

        private UInt32[] m_GenerateDistortionSequence(int length)
        {
            UInt32[] seq = new UInt32[length];
            Byte[] byteSeq = m_SeqGenerator.GenerateSequence(length * 4);
            for (int i = 0; i < length; ++i)
            {
                seq[i] = (UInt32)BitConverter.ToInt32(byteSeq, i * 4);
            }
            return seq;
        }
        
        public PositionDistortion(ChaoticSequenceGenerator generator)
        {
            this.m_SeqGenerator = generator;
        }

        public void Distort(Byte[] plainText, out Byte[] distortedText)
        {
            int length = plainText.Length;
            distortedText = (Byte[])plainText.Clone();
            UInt32[] posDistortionSeq = m_GenerateDistortionSequence(length);
            for (int i = 0; i < length; ++i)
            {
                Utils.Swap(ref distortedText[i], ref distortedText[posDistortionSeq[i] % length]);
            }
        }

        public void Restore(Byte[] distortedText, out Byte[] plainText)
        {
            int length = distortedText.Length;
            int ceiling = length - 1;
            UInt32[] rChaoticSequence = m_GenerateDistortionSequence(length).Reverse().ToArray();
            Byte[] rDistortedText = distortedText.Reverse().ToArray();
            for(int i = 0; i < length; ++i)
            {
                Utils.Swap(ref rDistortedText[i], ref rDistortedText[ceiling - rChaoticSequence[i] % length]);
            }
            plainText = rDistortedText.Reverse().ToArray();
        }
    }
}
