using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using simple.bus.core.model;

namespace simple.bus.core.service
{
    /// <summary>
    /// Base Service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BService<T>
         where T : BModel<T>
    {

        public DateTime NowDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="Entity">The entity.</param>
        /// <returns></returns>
        public int Delete(T Entity)
        {
            var entity = Entity as BEntity<T>;
            //var sel = new SimpleSelect();
            return 0;
        }
    }
}
