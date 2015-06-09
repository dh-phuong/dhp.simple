using System;
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
    public interface ISimple<T>
       where T : BModel<T>
    {
        #region Querry
        #endregion

        #region Public Method
        T Single();
        T SingleOrDefault();
        IEnumerable<T> GetListResult();
        #endregion
    }
    public interface ISimpleSelect<T> : ISimple<T>
       where T : BModel<T>
    {
        #region Querry
        ResultType Get<ResultType>(string fields);
        Int32 Count();
        IList<IDictionary<string, object>> Get(params string[] fields);
        ISimpleSelect<T> Set(string column, object value);
        ISimpleSelect<T> Min(params string[] fields);
        ISimpleSelect<T> Max(params string[] fields);
        ISimpleSelect<T> Sum(params string[] fields);
        ISimpleSelect<T> OrderBy(params string[] fields);
        ISimpleSelect<T> GroupBy(params string[] fields);
        #endregion

        #region Non-Querry
        decimal Insert(T t);
        decimal Insert();
        int Update(T t);
        int Update();
        int Delete();
        #endregion
    }
}
