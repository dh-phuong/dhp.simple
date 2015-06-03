using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using simple.helper;
using simple.bus.core.attribute;

namespace simple.bus.core.model
{
    public abstract class BModel<T>
        where T : class
    {
        #region Constructor

        public BModel()
        {
        }

        public BModel(DbDataReader dr)
        {
            var properties = typeof(T).GetProperties();
            foreach (var propInfo in properties)
            {
                if (propInfo.CanRead && propInfo.CanWrite)
                {
                    var name = propInfo.Name.Decamelize().ToLower();
                    if (dr[name] != DBNull.Value)
                        propInfo.SetValue(this, dr[name], null);
                }
            }
        }

        #endregion Constructor

        #region Clone

        /// <summary>
        /// Clone Object
        /// </summary>
        /// <typeparam name="T">Class to clone</typeparam>
        /// <returns></returns>
        public T Clone()
        {
            return (T)base.MemberwiseClone();
        }

        #endregion Clone

        public override string ToString()
        {
            return StringHelper.Me.ToJson<object>(this);
        }

        /// <summary>
        /// Gets the member names of properties.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetMemberNames()
        {
            return this.GetType().GetProperties()
                                              .Where(p => p.CanRead && p.CanWrite
                                                  && !p.CustomAttributes.Any(s => s.AttributeType.Equals(typeof(AutoColumnAttribute))))
                                              .Select(it => it.Name.Decamelize(true));
        }
    }
}
