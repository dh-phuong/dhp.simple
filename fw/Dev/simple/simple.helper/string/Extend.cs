using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace simple.helper
{
    /// <summary>
    /// Extend of method
    /// </summary>
    public static class Extend
    {
        /// <summary>
        /// Customs function Left VB
        /// </summary>
        /// <param name="Original">The original.</param>
        /// <param name="Count">The count.</param>
        /// <returns></returns>
        public static string Left(this string Original, int Count)
        {
            // Can't remember if the Left function throws an exception in this case,but for
            // this method, we will just return the original string.
            if (Original == null || Original == string.Empty
                || Original.Length < Count)
            {
                return Original;
            }
            else
            {
                // Return a sub-string of the original string, starting at index 0.
                return Original.Substring(0, Count);
            }
        }

        /// <summary>
        /// Customs function Right VB
        /// </summary>
        /// <param name="Original">The original.</param>
        /// <param name="Count">The count.</param>
        /// <returns></returns>
        public static string Right(this string Original, int Count)
        {
            // same thing as above.
            if (Original == null || Original == string.Empty
                || Original.Length < Count)
            {
                return Original;
            }
            else
            {
                // blah blah blah
                return Original.Substring(Original.Length - (Count));
            }
        }

        /// <summary>
        /// Customs function Mid VB
        /// </summary>
        /// <param name="param"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Mid(this string param, int startIndex, int length)
        {
            return param.Substring(startIndex - 1, length);
        }

        /// <summary>
        /// Customs function Mid VB
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public static string Mid(this string param, int startIndex)
        {
            return param.Substring(startIndex - 1);
        }

        /// <summary>
        /// Numbers to currency string.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <param name="moneyName">Name of the money.</param>
        /// <param name="decimalName">Name of the decimal.</param>
        /// <returns></returns>
        public static string NumberToCurrencyString(this decimal number, CultureInfo cultureInfo, string moneyName, string decimalName = "", bool readNagative = false)
        {
            var enCulture = CultureInfo.GetCultureInfo("en-US");
            var viCulture = CultureInfo.GetCultureInfo("vi-VN");

            if (cultureInfo == enCulture)
            {
                return StringHelper.Me.ReadNumber(string.Format("{0}", number), moneyName, decimalName, readNagative);
            }
            else
            {
                return StringHelper.Me.DocSo(string.Format("{0}", number), moneyName, decimalName, readNagative);
            }
        }

        /// <summary>
        /// Decamelizes the specified string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public static string Decamelize(this string s, bool toLower = false)
        {
            string ret = s;
            if (!string.IsNullOrEmpty(s))
            {
                if (s.Length == 1)
                {
                    ret = s.ToUpperInvariant();
                }
                else
                {
                    StringBuilder buf = new StringBuilder(255);
                    int pos = 0;
                    for (int i = 1; i < s.Length; ++i)
                    {
                        if (Char.IsUpper(s[i]))
                        {
                            if (buf.Length != 0)
                            {
                                buf.Append('_');
                            }
                            buf.Append(s.Substring(pos, (i - pos)).ToUpperInvariant());
                            pos = i;
                        }
                    }

                    if (buf.Length != 0)
                    {
                        buf.Append('_');
                    }
                    ret = buf.Append(s.Substring(pos).ToUpperInvariant()).ToString();
                }
                
                if (!toLower)
                    return ret;
                return ret.ToLowerInvariant();
            }
            return s;
        }

        /// <summary>
        /// Camelizes the specified string.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public static string Camelize(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            s = s.ToLowerInvariant();
            string[] array = s.Split('_');
            if (array.Length == 1)
            {
                return s.Capitalize();
            }
            StringBuilder buf = new StringBuilder(255);
            for (int i = 0; i < array.Length; ++i)
            {
                buf.Append(array[i].Capitalize());
            }
            return buf.ToString();
        }

        /// <summary>
        /// Capitalizes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static string Capitalize(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            char[] chars = name.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            return new String(chars);
        }

        /// <summary>
        /// To the entity class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public static T ToEntityClass<T>(this string className)
        {
            var entityName = className.Camelize();
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var paths = from path in Directory.GetFiles(assemblyFolder, "*.dll", SearchOption.AllDirectories)
                        select path;
            foreach (var path in paths)
            {
                Assembly assembly = Assembly.LoadFrom(path);
                Type findClass = (Type)(from type in assembly.GetTypes()
                                        where type.IsClass == true
                                           && type.Name.Equals(entityName)
                                        select type);
                if (findClass != null)
                {
                    return (T)Activator.CreateInstance(findClass);
                }
            }

            return default(T);
        }

        ///// <summary>
        ///// Gets the member names of properties. Not all IDynamicMetaObjectProvider have support for this.
        ///// </summary>
        ///// <param name="target">The target.</param>
        ///// <param name="dynamicOnly">if set to <c>true</c> [dynamic only]. Won't add reflected properties</param>
        ///// <returns></returns>
        //public static IEnumerable<string> GetMemberNames(this object target, bool dynamicOnly = false)
        //{
        //    var tList = new List<string>();
        //    if (!dynamicOnly)
        //    {
        //        tList.AddRange(target.GetType().GetProperties()
        //                                       .Where(p => p.CanRead && p.CanWrite
        //                                           && !p.CustomAttributes.Any(s=> s.GetType().Equals(typeof(AcceptRejectRule)))

        //                                      )
        //                                       .Select(it => it.Name.Decamelize(true)));
        //    }
        //    var tTarget = target as IDynamicMetaObjectProvider;
        //    if (tTarget != null)
        //    {
        //        tList.AddRange(tTarget.GetMetaObject(Expression.Constant(tTarget))
        //                              .GetDynamicMemberNames()
        //                              .Select(it => it.Decamelize().ToLower()));
        //    }
        //    return tList;
        //}

        /// <summary>
        /// Sets the name of the member.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="value">The value.</param>
        public static void SetMemberName(this object target, string memberName, object value)
        {
            target.GetType().GetProperty(memberName.Camelize()).SetValue(target, value, null);
        }
        public static object GetMemberValue(this object target, string memberName)
        {
            return target.GetType().GetProperty(memberName.Camelize()).GetValue(target);
        }
        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        /// <param name="theType">The type.</param>
        /// <returns></returns>
        public static SqlDbType GetDBType(this System.Type theType)
        {
            System.Data.SqlClient.SqlParameter p1 = default(System.Data.SqlClient.SqlParameter);
            System.ComponentModel.TypeConverter tc = default(System.ComponentModel.TypeConverter);
            p1 = new System.Data.SqlClient.SqlParameter();
            tc = System.ComponentModel.TypeDescriptor.GetConverter(p1.DbType);
            if (tc.CanConvertFrom(theType))
            {
                p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
            }
            else
            {
                //Try brute force
                p1.DbType = (DbType)tc.ConvertFrom(theType.Name);
            }
            return p1.SqlDbType;
        }
    }
}
