using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using simple.bus.core.model;
using simple.bus.core.service;

namespace simple.sql
{
    public static class Simple
    {
        public static SimpleSelect<T> SimpleSelect<T>(this BService<T> service, SimpleWhere where
                                                    , [CallerMemberName]string callerMemberName = "")
            where T : BModel<T>
        {

#region Log
		#if DEBUG
            {
                Trace.Write(" - [CallerMemberName] :" + callerMemberName);
            }
#endif 
	#endregion

            return new SimpleSelect<T>(where);
        }

        public static SimpleSelect<T> SimpleSelect<T>(this BService<T> service
                                                    , [CallerMemberName]string callerMemberName = "")
            where T : BModel<T>
        {

            #region Log
#if DEBUG
            {
                Trace.Write(" - [CallerMemberName] :" + callerMemberName);
            }
#endif
            #endregion

            return new SimpleSelect<T>();
        }
    }

    public sealed class SimpleSelect<T>
        where T : BModel<T>
    {
        #region Enum

        /// <summary>
        /// Execute Type
        /// </summary>
        private enum ExecuteType
        {
            AutoSql = 0,
            Text = 1,
            Stored = 2,
            [ObsoleteAttribute("This option will ready on later vesion. thank you", false)]
            SqlFile = 3
        }

        /// <summary>
        /// Command
        /// </summary>
        private enum Command
        {
            /// <summary>
            /// The select
            /// </summary>
            Select = 0,
            /// <summary>
            /// The insert
            /// </summary>
            Insert = 1,
            /// <summary>
            /// The update
            /// </summary>
            Update = 2,
            /// <summary>
            /// The delete
            /// </summary>
            Delete = 3,
        }
        #endregion Enum
        public SimpleWhere Where { get; set; }
        internal SimpleSelect()
        {
        }
        internal SimpleSelect(SimpleWhere where)
        {
            this.Where = where;
        }

        #region Public Method
        /// <summary>
        /// Singles this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        public T Single()
        {
            T t = this.SingleOrDefault();
            if (t == null)
            {
                throw new NullReferenceException();
            }
            return t;
        }

        /// <summary>
        /// Singles the or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SingleOrDefault()
        {
            try
            {

                var sql = this.Where.ToSql();
                #region Log
#if DEBUG
                {
                    Trace.WriteLine(" - [WHERE] :" + sql);
                    Trace.WriteLine(" - [SqlParameters] :");
                    for (int next = 0; next < this.Where.SqlParameters.Count; next++)
                    {
                        Trace.WriteLine(string.Format("{0} : {1}", this.Where.SqlParameters[next], this.Where.SqlParameters[next].Value));
                    }
                }
#endif
                #endregion
                T ret = default(T);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Public Method
    }
}
