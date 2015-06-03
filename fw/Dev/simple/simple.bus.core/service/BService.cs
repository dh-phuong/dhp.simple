using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using simple.bus.core.model;
using simple.bus.core.context;

namespace simple.bus.core.service
{
    /// <summary>
    /// Base Service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BService<T>
         where T : BModel<T>
    {
        public IBContext Context { get; set; }

        public BService()
        {
            this.Context = new DBContext();
        }

        public DateTime NowDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        public void SetUpdateInfo<E>(E entity)
            where E: BEntity<E>
        {
            var nowDate = this.NowDate;
            entity.CreateDate = nowDate;
            entity.UpdateDate = nowDate;
            entity.VersionNumber = 1;
            entity.DeleteFlag = false;
            entity.UpdateUCd = "10";
            entity.CreateUCd = "10";
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
