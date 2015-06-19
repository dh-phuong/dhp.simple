using System.Collections.Generic;
using simple.core.model;

namespace simple.sql
{
    public interface ISimpleSelectFromSQLFile<T> : ISimple<T>
       where T : BModel<T>
    {
        IList<IDictionary<string, object>> Get();

        int Execute();
    }
}