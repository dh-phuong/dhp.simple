using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using simple.core.model;
using simple.core.service;
using hpsofts.StringHelper;
using hpsofts.Extension;

namespace simple.sql
{
    public sealed class SimpleSelect<T> : ISimpleSelect<T>
        where T : BModel<T>
    {
        #region Property

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Queue<object> _container = new Queue<object>();

        /// <summary>
        /// The _order columns
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Queue<string> _orderColumns = new Queue<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Queue<string> _maxColumns = new Queue<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Queue<string> _minColumns = new Queue<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Queue<string> _sumColumns = new Queue<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Queue<string> _groupColumns = new Queue<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal BService<T> Service { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal SimpleWhere Where { get; set; }

        #endregion Property

        #region Contructor

        internal SimpleSelect()
        {
        }

        internal SimpleSelect(SimpleWhere where)
        {
            this.Where = where;
        }

        #endregion Contructor

        #region Querry

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ResultType">The type of the esult type.</typeparam>
        /// <param name="field">The column name.</param>
        /// <returns></returns>
        public ResultType Get<ResultType>(string field)
        {
            var resDto = this.Get(field);
            if (resDto.Count > 0)
            {
                return (ResultType)resDto[0][field];
            }
            return default(ResultType);
        }

        /// <summary>
        /// Gets the specified names.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields">The columns.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public IList<IDictionary<string, object>> Get(params string[] fields)
        {
            if (fields == null || fields.Length == 0)
                throw new ArgumentNullException();
            for (int i = 0; i < fields.Length; i++)
            {
                this._container.Enqueue(fields[i].Decamelize(true));
            }
            IList<IDictionary<string, object>> resDto = new List<IDictionary<string, object>>();
            var cmd = this.SelectCommand();
            using (DbDataReader dr = this.Service.Context.ExecuteReader(cmd))
            {
                while (dr.Read())
                {
                    IDictionary<string, object> item = new Dictionary<string, object>();
                    var cols = new object[dr.FieldCount];
                    for (int i = 0; i < cols.Length; i++)
                    {
                        var column = dr.GetName(i).Camelize();
                        var val = dr[dr.GetName(i)];
                        item.Add(new KeyValuePair<string, object>(column, val));
                    }
                    resDto.Add(item);
                }
            }
            #region Trace Log
		#if DEBUG
            SqlHelper.TraceLogCmd(cmd);
            #endif 
	#endregion
            return resDto;
        }

        /// <summary>
        /// Sets the specified field.
        /// </summary>
        /// <param name="column">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ISimpleSelect<T> Set(string column, object value)
        {
            this._container.Enqueue(new Tuple<string, object>(column.Decamelize(true), value));
            return this;
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ISimpleSelect<T> Min(params string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    this._minColumns.Enqueue(fields[i].Decamelize(true));
                }
            }
            return this;
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ISimpleSelect<T> Max(params string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    this._maxColumns.Enqueue(fields[i].Decamelize(true));
                }
            }
            return this;
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ISimpleSelect<T> Sum(params string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    this._sumColumns.Enqueue(fields[i].Decamelize(true));
                }
            }
            return this;
        }

        /// <summary>
        /// Orders the by.
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ISimpleSelect<T> OrderBy(params string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    this._orderColumns.Enqueue(fields[i].Decamelize(true));
                }
            }
            return this;
        }

        /// <summary>
        /// Group by
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public ISimpleSelect<T> GroupBy(params string[] fields)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    this._groupColumns.Enqueue(fields[i].Decamelize(true));
                }
            }
            return this;
        }

        public Int32 Count()
        {
            this._container.Enqueue("COUNT(*)");
            var cmd = this.SelectCommand();
            var count = this.Service.Context.ExecuteScalar(cmd);
            #region Trace Log
#if DEBUG
            SqlHelper.TraceLogCmd(cmd);
#endif
            #endregion
            return (Int32)count;
        }

        #endregion Querry

        #region Non-Querry

        /// <summary>
        /// Inserts the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public decimal Insert(T t)
        {
            this.ParseToParam(this._container, t);
            var entity = t as BEntity<T>;
            entity.SetMemberName("Id", this.Insert());
            return entity.Id;
        }

        /// <summary>
        /// Inserts this instance.
        /// </summary>
        /// <returns></returns>
        public decimal Insert()
        {
            SqlCommand cmd = new SqlCommand(this.GetSimpleInsertQuerry());
            while (this._container.Count > 0)
            {
                Tuple<string, object> param = (Tuple<string, object>)this._container.Dequeue();
                cmd.Parameters.Add(SqlHelper.CreateParameter(param.Item1, param.Item2));
            }
            var id = this.Service.Context.ExecuteScalar(cmd);
            #region Trace Log
#if DEBUG
            SqlHelper.TraceLogCmd(cmd);
#endif
            #endregion
            return (int)id;
        }

        /// <summary>
        /// Updates the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public int Update(T t)
        {
            this.ParseToParam(this._container, t);
            return this.Update();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <returns></returns>
        public int Update()
        {
            SqlCommand cmd = new SqlCommand();
            IList<SqlParameter> SqlParameters = new List<SqlParameter>();
            var sql = this.GetSimpleUpdateQuerry();
            while (this._container.Count > 0)
            {
                Tuple<string, object> param = (Tuple<string, object>)this._container.Dequeue();
                SqlParameters.Add(SqlHelper.CreateParameter(param.Item1, param.Item2));
            }
            if (this.Where != null)
            {
                sql += "WHERE \r\n\t";
                sql += this.Where.ToSql();
                SqlParameters = SqlParameters.AddRange(this.Where.SqlParameters).ToList();
            }
            else
            {
                // Update by default primary key
                T t = Activator.CreateInstance<T>();
                var PK = t.GetPK();
                if (PK.Keys.Count == 0)
                {
                    throw new MissingFieldException(typeof(T).Name, "Primary Key");
                    #region Trace Log
#if DEBUG
                    SqlHelper.TraceLogCmd(cmd);
#endif
                    #endregion
                }
                sql += "WHERE \r\n";
                for (int i = 0; i < PK.Keys.Count; i++)
                {
                    var column = PK.Keys.ElementAt(i).Decamelize(true);
                    var param = SqlHelper.BuildParameterName(column);
                    if (0 == i)
                    {
                        sql += "\t " + column + " = " + param;
                    }
                    else
                    {
                        sql += "\t AND " + column + " = " + param;
                    }
                }
            }
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(SqlParameters.ToArray());
            var result = this.Service.Context.ExecuteNonQuerySql(cmd);
            #region Trace Log
#if DEBUG
            SqlHelper.TraceLogCmd(cmd);
#endif
            #endregion
            return result;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns></returns>
        public int Delete()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = this.GetSimpleDeleteQuerry();
            if (this.Where != null)
            {
                sql += "WHERE \r\n\t";
                sql += this.Where.ToSql();
                cmd.Parameters.AddRange(this.Where.SqlParameters.ToArray());
            }
            cmd.CommandText = sql;
            var result = this.Service.Context.ExecuteNonQuerySql(cmd);
            #region Trace Log
#if DEBUG
            SqlHelper.TraceLogCmd(cmd);
#endif
            #endregion
            return result;
        }

        #endregion Non-Querry

        #region Public Method

        /// <summary>
        /// Singles this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        public T Single()
        {
            T t = this.SingleOrDefault();
            if (t == null)
            {
                throw new NullReferenceException();
            }
            return t;
        }

        /// <summary>
        /// Singles the or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SingleOrDefault()
        {
            try
            {
                return this.GetListResult().SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the list result.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetListResult()
        {
            try
            {
                var cmd = this.SelectCommand();
                #region Trace Log
#if DEBUG
                SqlHelper.TraceLogCmd(cmd);
#endif
                #endregion
                return this.Service.Context.GetData<T>(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Public Method

        #region SQL String

        /// <summary>
        /// Gets the simple select querry.
        /// </summary>
        /// <returns></returns>
        private string GetSimpleSelectQuerry()
        {
            T t = Activator.CreateInstance<T>();
            StringBuilder sql = new StringBuilder();
            var tableName = StringHelper.Me.ToSqlTable<T>();
            IList<string> columns = new List<string>();
            if (this._groupColumns.Count > 0)
            {
                columns = columns.AddRange(this._groupColumns.ToArray()).ToList();
            }
            else if (this._container.Count > 0)
            {
                columns = columns.AddRange(this._container.Cast<string>()).ToList();
            }
            else
            {
                columns = t.GetColumNames(true).ToArray();
            }
            if (this._minColumns.Count > 0 || this._maxColumns.Count > 0 || this._sumColumns.Count > 0)
            {
                foreach (var min in this._minColumns)
                {
                    columns.Add(string.Format("MIN({0}) {0}", min));
                }
                foreach (var max in this._maxColumns)
                {
                    columns.Add(string.Format("MAX({0}) {0}", max));
                }
                foreach (var sum in this._sumColumns)
                {
                    columns.Add(string.Format("SUM({0}) {0}", sum));
                }
            }
            string cols = string.Join("\r\n\t, ", columns);
            sql.Append("SELECT \r\n\t  ");
            sql.AppendLine(cols + " ");
            sql.AppendLine("FROM " + tableName + " ");
            return sql.ToString();
        }

        /// <summary>
        /// Gets the simple delete querry.
        /// </summary>
        /// <returns></returns>
        private string GetSimpleDeleteQuerry()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("DELETE FROM ");
            sql.AppendLine(StringHelper.Me.ToSqlTable<T>() + " ");
            return sql.ToString();
        }

        /// <summary>
        /// Gets the simple insert querry.
        /// </summary>
        /// <returns></returns>
        private string GetSimpleInsertQuerry()
        {
            T t = Activator.CreateInstance<T>();
            StringBuilder sql = new StringBuilder();
            var tableName = StringHelper.Me.ToSqlTable<T>();
            var columns = t.GetColumNames().ToArray();
            string cols = string.Join(", ", columns);
            sql.AppendFormat("SET NOCOUNT ON; ");
            sql.AppendFormat("DECLARE @out_id as ins_ids; ");
            sql.AppendFormat("INSERT INTO {0}(", tableName);
            sql.AppendLine(cols + ") ");
            sql.AppendLine("OUTPUT INSERTED.id into @out_id ");
            sql.Append("VALUES(");
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = SqlHelper.BuildParameterName(columns[i]);
            }
            string param = string.Join(", ", columns);
            sql.AppendLine(param + "); ");
            sql.AppendLine("SELECT * from @out_id ");
            return sql.ToString();
        }

        /// <summary>
        /// Gets the simple update querry.
        /// </summary>
        /// <returns></returns>
        private string GetSimpleUpdateQuerry()
        {
            StringBuilder sql = new StringBuilder();
            var tableName = StringHelper.Me.ToSqlTable<T>();
            sql.AppendLine("UPDATE " + tableName + " ");
            sql.AppendLine("SET ");
            sql.Append("\t  ");
            IList<string> columns = new List<string>();
            foreach (Tuple<string, object> setField in this._container)
            {
                columns.Add(string.Format("{0} = {1}\r\n", setField.Item1, SqlHelper.BuildParameterName(setField.Item1)));
            }
            string cols = string.Join("\t, ", columns);
            sql.AppendLine(cols);
            return sql.ToString();
        }

        #endregion SQL String

        #region Private

        /// <summary>
        /// Parses to parameter.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="t">The object model.</param>
        /// <exception cref="System.Exception">Duplicate parameter</exception>
        private void ParseToParam<E>(Queue<object> queue, E t)
            where E : BModel<E>
        {
            var columns = t.GetColumNames();
#if DEBUG
            foreach (var s in columns)
            {
                if (queue.Any(c => ((Tuple<string, object>)c).Item1.Equals(s)))
                {
                    throw new Exception("Duplicate parameter");
                }
                queue.Enqueue(new Tuple<string, object>(s, t.GetMemberValue(s)));
            }
#else
            columns.AsParallel<string>().ForAll(s =>
            {
                lock(this)
                {
                    if (queue.Any(c => ((Tuple<string, object>)c).Item1.Equals(s)))
                    {
                        throw new Exception("Duplicate parameter");
                    }
                    queue.Enqueue(new Tuple<string, object>(s, t.GetMemberValue(s)));
                }
            });
#endif
        }

        /// <summary>
        /// Selects the command.
        /// </summary>
        /// <returns></returns>
        private SqlCommand SelectCommand()
        {
            SqlCommand cmd = new SqlCommand();
            var sql = this.GetSimpleSelectQuerry();

            #region Where

            if (this.Where != null)
            {
                sql += "WHERE \r\n\t";
                sql += this.Where.ToSql();
                cmd.Parameters.AddRange(this.Where.SqlParameters.ToArray());
            }

            #endregion Where

            #region Group By

            IList<string> groups = new List<string>();
            while (this._groupColumns.Count > 0)
            {
                groups.Add(this._groupColumns.Dequeue());
            }
            if (groups.Count > 0)
            {
                sql += "GROUP BY \r\n\t  ";
                sql += string.Join("\r\n\t, ", groups);
                sql += "\r\n";
            }

            #endregion Group By

            #region OrderBy

            IList<string> orders = new List<string>();
            while (this._orderColumns.Count > 0)
            {
                orders.Add(this._orderColumns.Dequeue());
            }
            if (orders.Count > 0)
            {
                sql += "ORDER BY \r\n\t  ";
                sql += string.Join("\r\n\t, ", orders);
                sql += "\r\n";
            }

            #endregion OrderBy

            cmd.CommandText = sql;
            return cmd;
        }

        #endregion Private
    }

    public sealed class SimpleSelectFromSQLFile<T> : ISimpleSelectFromSQLFile<T>
         where T : BModel<T>
    {
        #region Contructor

        internal SimpleSelectFromSQLFile()
        {
        }

        #endregion Contructor

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Queue<object> _container = new Queue<object>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal BService<T> Service { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string URLFile { set; get; }

        #region FromFile

        internal ISimpleSelectFromSQLFile<T> Load(string urlFile, BReqDto reqDto)
        {
            this.ParseToParam(this._container, reqDto);
            this.URLFile = urlFile;
            return this;
        }

        #endregion FromFile

        #region Private

        /// <summary>
        /// Parses to parameter.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="t">The object model.</param>
        /// <exception cref="System.Exception">Duplicate parameter</exception>
        private void ParseToParam<E>(Queue<object> queue, E t)
            where E : BModel<E>
        {
            if (t != null)
            {
                var columns = t.GetColumNames();
#if DEBUG
                foreach (var s in columns)
                {
                    if (queue.Any(c => ((Tuple<string, object>)c).Item1.Equals(s)))
                    {
                        throw new Exception("Duplicate parameter");
                    }
                    queue.Enqueue(new Tuple<string, object>(s, t.GetMemberValue(s)));
                }
#else
            columns.AsParallel<string>().ForAll(s =>
            {
                lock(this)
                {
                    if (queue.Any(c => ((Tuple<string, object>)c).Item1.Equals(s)))
                    {
                        throw new Exception("Duplicate parameter");
                    }
                    queue.Enqueue(new Tuple<string, object>(s, t.GetMemberValue(s)));
                }
            });
#endif
            }
        }

        private SqlCommand SqlCommand()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = string.Empty;
            using (var stream = new System.IO.StreamReader(this.URLFile, Encoding.UTF8))
            {
                sql = stream.ReadToEnd();

                stream.Close();
            }
            while (this._container.Count > 0)
            {
                var param = (Tuple<string, object>)this._container.Dequeue();
                if (param.Item2.GetType().IsArray)
                {
                    object[] arr = (object[])param.Item2;
                    IList<string> arrParam = new List<string>();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var rename = string.Format("{0}_{1}", param.Item1, i + 1);
                        arrParam.Add(SqlHelper.BuildParameterName(rename));
                        cmd.Parameters.Add(SqlHelper.CreateParameter(rename, arr[i]));
                    }
                    sql = sql.Replace(SqlHelper.BuildParameterName(param.Item1), string.Join(", ", arrParam));
                }
                else
                {
                    cmd.Parameters.Add(SqlHelper.CreateParameter(param.Item1, param.Item2));
                }
            }
            cmd.CommandText = sql;
            return cmd;
        }

        #endregion Private

        #region Method

        public IList<IDictionary<string, object>> Get()
        {
            IList<IDictionary<string, object>> resDto = new List<IDictionary<string, object>>();
            var cmd = this.SqlCommand();
            using (DbDataReader dr = this.Service.Context.ExecuteReader(cmd))
            {
                while (dr.Read())
                {
                    IDictionary<string, object> item = new Dictionary<string, object>();
                    var cols = new object[dr.FieldCount];
                    for (int i = 0; i < cols.Length; i++)
                    {
                        var column = dr.GetName(i).Camelize();
                        var val = dr[dr.GetName(i)];
                        item.Add(new KeyValuePair<string, object>(column, val));
                    }
                    resDto.Add(item);
                }
            }
            #region Trace Log
#if DEBUG
            SqlHelper.TraceLogCmd(cmd);
#endif
            #endregion
            return resDto;
        }

        /// <summary>
        /// Singles this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        public T Single()
        {
            return this.Single<T>();
        }

        public T1 Single<T1>()
            where T1 : BModel<T1>
        {
            T1 t = this.SingleOrDefault<T1>();
            if (t == null)
            {
                throw new NullReferenceException();
            }
            return t;
        }

        /// <summary>
        /// Singles the or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SingleOrDefault()
        {
            return this.SingleOrDefault<T>();
        }

        /// <summary>
        /// Singles the or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T1 SingleOrDefault<T1>()
             where T1 : BModel<T1>
        {
            try
            {
                return this.GetListResult<T1>().SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the list result.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetListResult()
        {
            return this.GetListResult<T>();
        }

        /// <summary>
        /// Gets the list result.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T1> GetListResult<T1>()
            where T1 : BModel<T1>
        {
            try
            {
                var cmd = this.SqlCommand();
                #region Trace Log
#if DEBUG
                SqlHelper.TraceLogCmd(cmd);
#endif
                #endregion
                return this.Service.Context.GetData<T1>(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int Execute()
        {
            try
            {
                var cmd = this.SqlCommand();
                #region Trace Log
#if DEBUG
                SqlHelper.TraceLogCmd(cmd);
#endif
                #endregion
                return this.Service.Context.ExecuteNonQuerySql(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }

    public sealed class SimpleSelectFromStored<T> : ISimpleSelectFromStored<T>
         where T : BModel<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Queue<object> _container = new Queue<object>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal BService<T> Service { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string StoredName { set; get; }

        #region Contructor

        internal SimpleSelectFromStored()
        {
        }

        #endregion Contructor

        #region From Stored

        internal SimpleSelectFromStored<T> Load(string storedName, BReqDto reqDto)
        {
            this.StoredName = storedName;
            this.ParseToParam(this._container, reqDto);
            return this;
        }

        #endregion From Stored

        #region ISimple<T> Members

        /// <summary>
        /// Singles this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        public T Single()
        {
            T t = this.SingleOrDefault();
            if (t == null)
            {
                throw new NullReferenceException();
            }
            return t;
        }

        /// <summary>
        /// Singles the or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SingleOrDefault()
        {
            try
            {
                return this.GetListResult().SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the list result.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetListResult()
        {
            try
            {
                var cmd = this.SqlCommand();
                #region Trace Log
#if DEBUG
                SqlHelper.TraceLogCmd(cmd);
#endif
                #endregion
                return this.Service.Context.GetData<T>(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ISimple<T> Members

        #region Private

        /// <summary>
        /// Parses to parameter.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="t">The object model.</param>
        /// <exception cref="System.Exception">Duplicate parameter</exception>
        private void ParseToParam<E>(Queue<object> queue, E t)
            where E : BModel<E>
        {
            if (t != null)
            {
                var columns = t.GetColumNames();
#if DEBUG
                foreach (var s in columns)
                {
                    if (queue.Any(c => ((Tuple<string, object>)c).Item1.Equals(s)))
                    {
                        throw new Exception("Duplicate parameter");
                    }
                    queue.Enqueue(new Tuple<string, object>(s, t.GetMemberValue(s)));
                }
#else
            columns.AsParallel<string>().ForAll(s =>
            {
                lock(this)
                {
                    if (queue.Any(c => ((Tuple<string, object>)c).Item1.Equals(s)))
                    {
                        throw new Exception("Duplicate parameter");
                    }
                    queue.Enqueue(new Tuple<string, object>(s, t.GetMemberValue(s)));
                }
            });
#endif
            }
        }

        private SqlCommand SqlCommand()
        {
            SqlCommand cmd = new SqlCommand();
            string sql = this.StoredName;
            while (this._container.Count > 0)
            {
                var param = (Tuple<string, object>)this._container.Dequeue();
                cmd.Parameters.Add(SqlHelper.CreateParameter(param.Item1, param.Item2));
            }
            cmd.CommandText = sql;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }

        #endregion Private

        #region ISimpleSelectFromStored<T> Members

        public IList<IDictionary<string, object>> Get()
        {
            IList<IDictionary<string, object>> resDto = new List<IDictionary<string, object>>();
            var cmd = this.SqlCommand();
            using (DbDataReader dr = this.Service.Context.ExecuteReader(cmd))
            {
                while (dr.Read())
                {
                    IDictionary<string, object> item = new Dictionary<string, object>();
                    var cols = new object[dr.FieldCount];
                    for (int i = 0; i < cols.Length; i++)
                    {
                        var column = dr.GetName(i).Camelize();
                        var val = dr[dr.GetName(i)];
                        item.Add(new KeyValuePair<string, object>(column, val));
                    }
                    resDto.Add(item);
                }
            }
            #region Trace Log
#if DEBUG
            SqlHelper.TraceLogCmd(cmd);
#endif
            #endregion
            return resDto;
        }

        public int Execute()
        {
            try
            {
                var cmd = this.SqlCommand();
                #region Trace Log
#if DEBUG
                SqlHelper.TraceLogCmd(cmd);
#endif
                #endregion
                return this.Service.Context.ExecuteNonQuerySql(cmd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ISimpleSelectFromStored<T> Members
    }
}