using System;
using simple.core.context;
using simple.core.model;

namespace simple.core.service
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
            this.Context = ctx;
        }

        public DateTime NowDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Sets the update information.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="entity">The entity.</param>
        public void SetUpdateInfo<E>(E entity)
            where E : BEntity<E>
        {
            var nowDate = this.NowDate;
            entity.CrtTime = nowDate;
            entity.UpdTime = nowDate;
            entity.UpdUId = 10;
            entity.CrtUId = 10;
        }

        public abstract string NextCSeq();
    }
}