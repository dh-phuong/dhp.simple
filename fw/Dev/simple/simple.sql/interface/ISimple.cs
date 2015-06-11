using System.Collections.Generic;
using simple.bus.core.model;

namespace simple.sql
{
    public interface ISimple<T>
     where T : BModel<T>
    {
        #region Public Method

        T Single();

        T SingleOrDefault();

        IEnumerable<T> GetListResult();

        #endregion Public Method
    }
}
