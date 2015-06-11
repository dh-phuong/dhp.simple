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
    public interface ISimpleSelectFromStored<T> : ISimple<T>
       where T : BModel<T>
    {
        IList<IDictionary<string, object>> Get();
        int Execute();
    }
}
