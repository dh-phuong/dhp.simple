using System;
using simple.bus.core.context;
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
        public IBContext Context { get; set; }

        public BService(IBContext ctx)
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
            where E : BEntity<E>
        {
            var nowDate = this.NowDate;
            entity.CreateDate = nowDate;
            entity.UpdateDate = nowDate;
            entity.DeleteFlag = false;
            entity.UpdateUId = 10;
            entity.CreateUId = 10;
        }

        public void SetContext(IBContext ctx)
        {
            this.Context = ctx;
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