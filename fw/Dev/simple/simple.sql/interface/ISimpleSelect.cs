using System;
using System.Collections.Generic;
using simple.core.model;

namespace simple.sql
{
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

        #endregion Querry

        #region Non-Querry

        decimal Insert(T t);

        decimal Insert();

        int Update(T t);

        int Update();

        int Delete();

        #endregion Non-Querry
    }
}