using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using simple.bus.core.model;
using simple.bus.core.service;
using simple.helper;

namespace simple.sql
{
    public static class Simple
    {
        public static ISimpleSelect<T> SimpleSelect<T>(this BService<T> service, SimpleWhere where
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

            return new SimpleSelect<T>()
            {
                Service = service,
                Where = where,
            };
        }

        public static ISimpleSelect<T> SimpleSelect<T>(this BService<T> service
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

            return new SimpleSelect<T>()
            {
                Service = service,
            };
        }

        /// <summary>
        /// Simples the select from SQL file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="fileUrl">The file URL.</param>
        /// <param name="reqDto">The req dto.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <returns></returns>
        public static ISimpleSelectFromSQLFile<T> SimpleSelectFromSQLFile<T>(this BService<T> service
                                                    , string fileUrl
                                                    , BReqDto reqDto
                                                    , [CallerMemberName]string callerMemberName = "")
            where T : BModel<T>
        {
            return new SimpleSelectFromSQLFile<T>()
            {
                Service = service,
            }.Load(fileUrl, reqDto);
        }

        /// <summary>
        /// Simples the select from stored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="service">The service.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="reqDto">The req dto.</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <returns></returns>
        public static SimpleSelect<T> SimpleSelectFromStored<T>(this BService<T> service
                                                    , string storedProcedureName
                                                    , BReqDto reqDto
                                                    , [CallerMemberName]string callerMemberName = "")
            where T : BModel<T>
        {
            return new SimpleSelect<T>()
            {
                Service = service,
            }.Execute(storedProcedureName, reqDto);
        }
    }
}
