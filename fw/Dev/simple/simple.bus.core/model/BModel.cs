using System;
using System.Data.Common;
using simple.helper;

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
    }
}
