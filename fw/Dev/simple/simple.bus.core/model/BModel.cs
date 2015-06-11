﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using simple.helper;
using simple.bus.core.attribute;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace simple.bus.core.model
{
    [Serializable]
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

        public void CopyFrom<T1>(T1 source)
            where T1 : BModel<T1>
        {
            var c1 = this.GetColumNames(true);
            var c2 = source.GetColumNames(true);
            var innerJoin = c1.Where(col => c2.Any(desCol => desCol.Equals(col))).Select(col => col);
#if DEBUG
            foreach (var column in innerJoin)
            {
                this.SetMemberName(column, source.GetMemberValue(column));
            }
#else
            innerJoin.AsParallel<string>().ForAll(s =>
            {
                this.SetMemberName(s, source.GetMemberValue(s));
            });
#endif
        }
        #endregion Clone

        public override string ToString()
        {
            return StringHelper.Me.ToJson<object>(this);
        }

        /// <summary>
        /// Gets the member name of columns.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetColumNames(bool getAll = false)
        {
            return this.GetType().GetProperties()
                                              .Where
                                              (p => p.CanRead && p.CanWrite
                                                  && 
                                                  (
                                                  getAll ||
                                                  !p.CustomAttributes.Any(s => s.AttributeType.Equals(typeof(AutoColumnAttribute))
                                                  )
                                              ))
                                              .Select(it =>  it.Name.Decamelize(true));
        }

        /// <summary>
        /// Gets the pk values.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> GetPK()
        {
            return this.GetType().GetProperties()
                                             .Where(p => p.CanRead && p.CanWrite
                                                 && p.CustomAttributes.Any(s => s.AttributeType.Equals(typeof(PrimaryKeyAttribute))))
                                             .ToDictionary(k => k.Name, v => v.GetValue(this, null));
            
        }
    }
}
