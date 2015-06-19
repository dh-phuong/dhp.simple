using System;
using System.IO;
using System.Security.Cryptography;

namespace hpsofts.security
{
    /// <summary>
    /// Lớp mã hóa AES
    /// </summary>
    internal sealed class AES : IAES
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AES"/> class.
        /// </summary>
        internal AES()
        {
        }

        #endregion Constructor

        #region Deconstructor

        ~AES()
        {
        }

        #endregion Deconstructor

        #region IAES メンバー

        /// <summary>
        /// Mã hóa chuỗi dữ liệu
        /// </summary>
        /// <param name="planText">chuỗi cần mã hóa</param>
        /// <param name="secretKey">từ khóa</param>
        /// <param name="bits">loại mã hóa</param>
        /// <param name="hash">băm chuỗi trả về</param>
        /// <returns></returns>
        string IAES.Encrypt(string planText, string secretKey, Bits bits, bool hash)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(planText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(secretKey,
                                                                         new byte[] {
                                                                            0x09, 0x25, 0x02, 0x1C,
                                                                            0x1D, 0x1E, 0x03, 0x04,
                                                                            0x05, 0x0F, 0x20, 0x21,
                                                                            0xAD, 0xAF, 0x12, 0x06 });

            string decryptedString = string.Concat(bits);
            if (bits == Bits.Bit128)
            {
                byte[] encryptedData = this.Encrypt(clearBytes, pdb.GetBytes(16), pdb.GetBytes(16));
                decryptedString = Convert.ToBase64String(encryptedData);
            }
            else if (bits == Bits.Bit192)
            {
                byte[] encryptedData = this.Encrypt(clearBytes, pdb.GetBytes(24), pdb.GetBytes(16));
                decryptedString = Convert.ToBase64String(encryptedData);
            }
            else if (bits == Bits.Bit256)
            {
                byte[] encryptedData = this.Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                decryptedString = Convert.ToBase64String(encryptedData);
            }
            return hash ? HashString(decryptedString) : decryptedString;
        }

        /// <summary>
        /// Dịch chuỗi mã hóa
        /// </summary>
        /// <param name="planText">chuỗi cần mã hóa</param>
        /// <param name="secretKey">từ khóa</param>
        /// <param name="bits">loại mã hóa</param>
        /// <returns>chuỗi được giải mã</returns>
        string IAES.Decrypt(string planText, string secretKey, Bits bits)
        {
            byte[] cipherBytes = Convert.FromBase64String(planText);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(secretKey,
                                                                        new byte[] {
                                                                            0x09, 0x25, 0x02, 0x1C,
                                                                            0x1D, 0x1E, 0x03, 0x04,
                                                                            0x05, 0x0F, 0x20, 0x21,
                                                                            0xAD, 0xAF, 0x12, 0x06 });
            if (bits == Bits.Bit128)
            {
                byte[] decryptedData = this.Decrypt(cipherBytes, pdb.GetBytes(16), pdb.GetBytes(16));
                return System.Text.Encoding.Unicode.GetString(decryptedData);
            }
            else if (bits == Bits.Bit192)
            {
                byte[] decryptedData = this.Decrypt(cipherBytes, pdb.GetBytes(24), pdb.GetBytes(16));
                return System.Text.Encoding.Unicode.GetString(decryptedData);
            }
            else if (bits == Bits.Bit256)
            {
                byte[] decryptedData = this.Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                return System.Text.Encoding.Unicode.GetString(decryptedData);
            }
            else
            {
                return string.Concat(bits);
            }
        }

        #endregion IAES メンバー

        #region IDisposable メンバー

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
        }

        #endregion IDisposable メンバー

        #region Private メンバー

        /// <summary>
        ///
        /// </summary>
        /// <param name="clearData"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        private byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream that is going to accept the encrypted bytes
            MemoryStream ms = new MemoryStream();

            Rijndael alg = Rijndael.Create();
            alg.Key = Key;

            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cipherData"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        private byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }

        /// <summary>
        /// Hash String
        /// </summary>
        /// <param name="stringInput"></param>
        /// <returns>Hex String hashed</returns>
        public string HashString(string stringInput)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(stringInput);
            byte[] hash = md5.ComputeHash(inputBytes);
            hash = md5.ComputeHash(hash);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        #endregion Private メンバー
    }
}
