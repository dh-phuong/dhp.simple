using System;

namespace hpsofts.security
{
    /// <summary>
    /// Interface AES
    /// </summary>
    internal interface IAES
    {
        /// <summary>
        /// Mã hóa chuỗi dữ liệu
        /// </summary>
        /// <param name="planText">chuỗi cần mã hóa</param>
        /// <param name="secretKey">từ khóa</param>
        /// <param name="bits">loại mã hóa</param>
        /// <param name="hash">băm chuỗi trả về</param>
        /// <returns>chuỗi được mã hóa</returns>
        string Encrypt(string planText, string secretKey, Bits bits, bool hash = false);

        /// <summary>
        /// Dịch chuỗi mã hóa
        /// </summary>
        /// <param name="planText">chuỗi cần mã hóa</param>
        /// <param name="secretKey">từ khóa</param>
        /// <param name="bits">loại mã hóa</param>
        /// <returns>chuỗi được giải mã</returns>
        string Decrypt(string planText, string secretKey, Bits bits);
    }
}
