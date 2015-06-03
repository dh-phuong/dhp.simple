using System;
using System.Linq;
using System.Web.Script.Serialization;

namespace simple.helper
{
    /// <summary>
    /// String Util
    /// </summary>
    public class StringHelper
    {
        private static StringHelper _owner;

        public static StringHelper Me
        {
            get
            {
                if (_owner == null)
                {
                    _owner = new StringHelper();
                }
                return _owner;
            }
        }

        /// <summary>
        /// To the name of the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string ToTableName<T>()
        {
            return string.Format("dbo.{0}", Activator.CreateInstance<T>().GetType().Name.Decamelize(true));
        }

        /// <summary>
        /// JSON Serialization
        /// </summary>
        public string ToJson<T>(T t)
        {
            if (null == t)
            {
                return string.Empty;
            }
            return new JavaScriptSerializer().Serialize(t);
        }

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        public T JsonTo<T>(string json)
        {
            if (string.IsNullOrEmpty(json)
                || string.IsNullOrEmpty(json.Trim()))
            {
                return default(T);
            }
            return (T)new JavaScriptSerializer().Deserialize(json, typeof(T));
        }

        #region Doc so bang tieng viet

        /// <summary>
        /// doc so thanh mot chuoi
        /// </summary>
        /// <param name="numberStr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string DocSo(string numberStr, string moneyName, string decimalName, bool readNagative)
        {
            if (string.IsNullOrEmpty(numberStr) || string.IsNullOrEmpty(numberStr.Trim()))
                return string.Empty;
            bool isNegative = numberStr.IndexOf('-') == 0;

            numberStr = numberStr.Trim(new char[] { '-' });
            string result = "";
            string decimalStr = "";
            int decimalPlace = 0;
            int count = 0;
            string tempString = "";
            string[] place = new string[10];
            place[2] = "Nghìn ";
            place[3] = "Triệu ";
            place[4] = "Tỷ ";
            place[5] = "Nghìn ";
            place[6] = "Triệu ";
            place[7] = "Tỷ ";
            // Position of decimal place 0 if none.
            decimalPlace = numberStr.IndexOf(".");
            // Convert cents and set MyNumber to dollar amount.
            if (decimalPlace > 0)
            {
                var decStr = this.ReadNumber2Digit((numberStr.Mid(decimalPlace + 2) + "00").Left(2));
                if (!string.IsNullOrEmpty(decStr))
                {
                    decimalStr = " và " + decStr + decimalName;
                }
                numberStr = (numberStr.Left(decimalPlace)).Trim();
            }
            count = 1;
            while (!string.IsNullOrEmpty(numberStr))
            {
                tempString = this.ReadNumber3Digit(numberStr.Right(3));
                if (!string.IsNullOrEmpty(tempString))
                    result = tempString + place[count] + result;
                if (numberStr.Length > 3)
                {
                    numberStr = numberStr.Left(numberStr.Length - 3);
                }
                else
                {
                    numberStr = "";
                }
                count = count + 1;
            }
            switch (result)
            {
                case "":
                    result = "không " + moneyName;
                    break;

                default:
                    result = result + moneyName;
                    break;
            }
            result = ((isNegative && readNagative ? "Âm " : string.Empty) + result + decimalStr).ToLower().Trim();
            if (result.IndexOf(moneyName, StringComparison.OrdinalIgnoreCase) > 0)
            {
                result = result.Replace(moneyName.ToLower(), moneyName);
            }
            return string.Format("{0}{1}.", char.ToUpper(result[0]), result.Substring(1));
        }

        /// <summary>
        /// doc chuoi so voi 3 chu so
        /// </summary>
        /// <param name="numberStr">The number string.</param>
        /// <returns></returns>
        private string ReadNumber3Digit(string numberStr)
        {
            const string odd_term = "lẻ ";
            const string one_term = "mốt ";
            const string five_term = "lăm ";
            const string ten_term = "mười ";

            numberStr = numberStr.PadLeft(3, ' ');
            var result = string.Empty;
            var numbers = numberStr.ToArray();
            if (numbers.Any(s => s != '0'))
            {
                for (int i = 0; i < numbers.Length; i++)
                {
                    var num = numbers[i].ToString();
                    if (string.IsNullOrWhiteSpace(num))
                        continue;

                    if (i == 0)
                    {
                        result += string.Format("{0} trăm ", this.NumberToString(num)); continue;
                    }
                    if (i == 1)
                    {
                        if (num == "0")
                        {
                            result += odd_term;
                        }
                        else if (num == "1")
                        {
                            result += ten_term;
                        }
                        else
                        {
                            result += string.Format("{0} mươi ", this.NumberToString(num));
                        }
                        continue;
                    }
                    if (i == 2)
                    {
                        var numPre = numbers[i - 1].ToString();
                        if (num == "5" && !string.IsNullOrWhiteSpace(numPre) && int.Parse(numPre) > 0)
                        {
                            result += five_term;
                        }
                        else if (num == "1" && !string.IsNullOrWhiteSpace(numPre) && int.Parse(numPre) > 1)
                        {
                            result += one_term;
                        }
                        else if (num != "0")
                        {
                            result += string.Format("{0} ", this.NumberToString(num));
                        }
                        continue;
                    }
                }
                if (result.EndsWith(odd_term))
                {
                    result = result.Replace(odd_term, string.Empty);
                }
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// doc chuoi so voi 2 chu so
        /// </summary>
        /// <param name="numberStr">The number string.</param>
        /// <returns></returns>
        private string ReadNumber2Digit(string numberStr)
        {
            const string one_term = "mốt ";
            const string five_term = "lăm ";
            const string ten_term = "mười ";

            var result = string.Empty;
            var numbers = numberStr.ToArray();
            if (numbers.Any(s => s != '0'))
            {
                for (int i = 0; i < numbers.Length; i++)
                {
                    var num = numbers[i].ToString();
                    if (string.IsNullOrWhiteSpace(num))
                        continue;
                    if (i == 0)
                    {
                        if (num != "0")
                        {
                            if (num == "1")
                            {
                                result += ten_term;
                            }
                            else
                            {
                                result += string.Format("{0} mươi ", this.NumberToString(num));
                            }
                        }
                        continue;
                    }
                    if (i == 1)
                    {
                        var numPre = numbers[i - 1].ToString();
                        if (num == "5" && !string.IsNullOrWhiteSpace(numPre) && int.Parse(numPre) > 0)
                        {
                            result += five_term;
                        }
                        else if (num == "1" && !string.IsNullOrWhiteSpace(numPre) && int.Parse(numPre) > 1)
                        {
                            result += one_term;
                        }
                        else if (num != "0")
                        {
                            result += string.Format("{0} ", this.NumberToString(num));
                        }
                        continue;
                    }
                }
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// so kieu chuoi
        /// </summary>
        /// <param name="intNum">The int number.</param>
        /// <returns></returns>
        private string NumberToString(string number)
        {
            string[] numbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            return numbers[int.Parse(number)];
        }

        #endregion Doc so bang tieng viet

        #region Doc so bang tieng anh

        /// <summary>
        /// Read number by English
        /// </summary>
        /// <param name="numberStr"></param>
        /// <param name="moneyName"></param>
        /// <param name="decimalName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string ReadNumber(string numberStr, string moneyName, string decimalName, bool readNagative)
        {
            bool isNegative = numberStr.IndexOf('-') == 0;

            string dollars = "";
            string cents = "";
            string tempString = "";
            int decimalPlace = 0;
            int count = 0;
            string[] place = new string[10];
            place[2] = "Thousand ";
            place[3] = "Million ";
            place[4] = "Billion ";
            place[5] = "Trillion ";
            // String representation of amount.
            if (string.IsNullOrEmpty(numberStr))
            {
                return string.Empty;
            }
            numberStr = numberStr.Trim(new char[] { '-' });
            // Position of decimal place 0 if none.
            decimalPlace = numberStr.IndexOf(".");
            // Convert cents and set MyNumber to dollar amount.
            if (decimalPlace > 0)
            {
                cents = this.GetTens((numberStr.Mid(decimalPlace + 2) + "00").Left(2));
                numberStr = (numberStr.Left(decimalPlace)).Trim();
            }
            count = 1;
            while (!string.IsNullOrEmpty(numberStr))
            {
                tempString = this.GetHundreds(numberStr.Right(3));
                if (!string.IsNullOrEmpty(tempString))
                    dollars = tempString + place[count] + dollars;
                if (numberStr.Length > 3)
                {
                    numberStr = numberStr.Left(numberStr.Length - 3);
                }
                else
                {
                    numberStr = "";
                }
                count = count + 1;
            }
            switch (dollars)
            {
                case "":
                    dollars = "Zero " + moneyName;
                    break;

                case "One":
                    dollars = "One " + moneyName;
                    break;

                default:
                    dollars = dollars + moneyName;
                    break;
            }
            switch (cents)
            {
                case "":
                    break;
                //Cents = " Only"
                case "One":
                    cents = " and One " + decimalName;
                    break;

                default:
                    cents = " and " + cents + decimalName;
                    break;
            }

            dollars = (isNegative && readNagative ? "Negative " : string.Empty) + dollars + cents;
            dollars = dollars.ToLower().Trim();
            if (dollars.IndexOf(moneyName, StringComparison.OrdinalIgnoreCase) > 0)
            {
                dollars = dollars.Replace(moneyName.ToLower(), moneyName);
            }
            return string.Format("{0}{1}.", char.ToUpper(dollars[0]), dollars.Substring(1));
        }

        /// <summary>
        /// Get number Hundreds
        /// </summary>
        /// <param name="numberStr"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string GetHundreds(string numberStr)
        {
            string functionReturnValue = null;
            string Result = string.Empty;
            if (int.Parse(numberStr) == 0)
            {
                return "";
                //return functionReturnValue;
            }
            numberStr = ("000" + numberStr).Right(3);
            // Convert the hundreds place.
            if (numberStr.Mid(1, 1) != "0")
            {
                Result = this.GetDigit(numberStr.Mid(1, 1)) + "Hundred ";
            }
            // Convert the tens and ones place.
            if (numberStr.Mid(2, 1) != "0")
            {
                Result = Result + this.GetTens(numberStr.Mid(2));
            }
            else
            {
                Result = Result + this.GetDigit(numberStr.Mid(3));
            }
            functionReturnValue = Result;
            return functionReturnValue;
        }

        /// <summary>
        /// Get Number Tens
        /// </summary>
        /// <param name="tensText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string GetTens(string tensText)
        {
            string[] numbers = new string[]
            {
                "", "", "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety ",
                "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen "
            };
            // Null out the temporary function value.
            // If value between 10-19...
            var ten = int.Parse(tensText.Left(1));
            if (ten == 1)
            {
                return numbers[int.Parse(tensText)];
            }
            else
            {
                return numbers[ten] + GetDigit(tensText.Right(1));
            }
        }

        /// <summary>
        /// Get Number Digits
        /// </summary>
        /// <param name="digit"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string GetDigit(string digit)
        {
            string[] numbers = new string[] { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
            return numbers[int.Parse(digit)];
        }

        #endregion Doc so bang tieng anh
    }
}
