using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaoticEncryption
{
    class Program
    {
        public static void Main()
        {
            ChaoticEncryption.EncryptionSystem ecs = new EncryptionSystem();
            Byte[] plT = new Byte[10] { 1,2,3,4,5,6,7,8,9,10 };
            ecs.Encrypt(ref plT, out plT);
            ecs.Decrypt(ref plT, out plT);
            int a = 0;
        }
    }
}
