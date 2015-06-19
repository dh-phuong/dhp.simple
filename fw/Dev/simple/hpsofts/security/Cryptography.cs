using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hpsofts.security
{
    /// <summary>
    /// Cryptography Level
    /// </summary>
    public enum Bits
    {
        /// <summary>
        ///
        /// </summary>
        Bit128 = 128,

        /// <summary>
        ///
        /// </summary>
        Bit192 = 192,

        /// <summary>
        ///
        /// </summary>
        Bit256 = 256,
    }

    /// <summary>
    /// 
    /// </summary>
    public class Cryptography
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="Cryptography"/> class from being created.
        /// </summary>
        public Cryptography()
        {

        }

        private static IAES _instance;
        private static IAES Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new hpsofts.security.AES();
                return _instance;
            }
        }
        public string Encrypt(string planText, string secretKey, Bits bits, bool hash = false)
        {
            return Cryptography.Instance.Encrypt(planText, secretKey, bits, hash);
        }
        public string Decrypt(string planText, string secretKey, Bits bits)
        {
            return Cryptography.Instance.Decrypt(planText, secretKey, bits);
        }
    }
}
