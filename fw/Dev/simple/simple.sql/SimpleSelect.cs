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
    public static class Simple
    {
        public static SimpleSelect<T> SimpleSelect<T>(this BService<T> service, SimpleWhere where
                                                    , [CallerMemberName]string callerMemberName = "")
            where T : BModel<T>
        {

            #region Log
#if DEBUG
            {
                Trace.Write(" - [CallerMemberName] :" + callerMemberName);
            }
#endif
            #endregion

            return new SimpleSelect<T>()
            {
                Service = service,
                Where = where,
            };
        }

        public static SimpleSelect<T> SimpleSelect<T>(this BService<T> service
                                                    , [CallerMemberName]string callerMemberName = "")
            where T : BModel<T>
        {

            #region Log
#if DEBUG
            {
                Trace.Write(" - [CallerMemberName] :" + callerMemberName);
            }
#endif
            #endregion

            return new SimpleSelect<T>()
            {
                Service = service,
            };
        }
    }

    public sealed class SimpleSelect<T>
        where T : BModel<T>
    {
        #region Enum

        /// <summary>
        /// Execute Type
        /// </summary>
        private enum ExecuteType
        {
            AutoSql = 0,
            Text = 1,
            Stored = 2,
            [ObsoleteAttribute("This option will ready on later vesion. thank you", false)]
            SqlFile = 3
        }

        /// <summary>
        /// Command
        /// </summary>
        private enum Command
        {
            /// <summary>
            /// The select
            /// </summary>
            Select = 0,
            /// <summary>
            /// The insert
            /// </summary>
            Insert = 1,
            /// <summary>
            /// The update
            /// </summary>
            Update = 2,
            /// <summary>
            /// The delete
            /// </summary>
            Delete = 3,
        }
        #endregion Enum

        #region Property
        private readonly Queue<object> _container = new Queue<object>();
        private readonly Queue<string> _orderColumns = new Queue<string>();
        private readonly Queue<string> _maxColumns = new Queue<string>();
        private readonly Queue<string> _minColumns = new Queue<string>();
        private readonly Queue<string> _sumColumns = new Queue<string>();
        private readonly Queue<string> _groupColumns = new Queue<string>();
        internal BService<T> Service { get; set; }
        internal SimpleWhere Where { get; set; }
        #endregion

        #region Contructor
        internal SimpleSelect()
        {
        }
        internal SimpleSelect(SimpleWhere where)
        {
            this.Where = where;
        }
        #endregion

        #region Querry
        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="ResultType">The type of the esult type.</typeparam>
        /// <param name="fields">The column name.</param>
        /// <returns></returns>
        public ResultType Get<ResultType>(string fields)
        {
            var resDto = this.Get(fields);
            if (resDto.Count > 0)
            {
                return (ResultType)resDto[0][fields];
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
            SqlCommand cmd = new SqlCommand();
            var sql = this.GetSimpleSelectQuerry();

            #region Where
            if (this.Where != null)
            {
                sql += "WHERE \r\n\t";
                sql += this.Where.ToSql();
                cmd.Parameters.AddRange(this.Where.SqlParameters.ToArray());
            }
            #endregion

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
            #endregion

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
            #endregion

            cmd.CommandText = sql;
            #region Log
#if DEBUG
            {
                Trace.WriteLine(" - [SQL] :\r\n" + sql);
                if (this.Where != null)
                {
                    Trace.WriteLine(" - [SqlParameters] :");
                    foreach (var item in this.Where.SqlParameters)
                    {
                        Trace.WriteLine(string.Format("{0} : {1}", item, item.Value));
                    }
                }

            }
#endif
            #endregion
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

                        //item[i] = dr[names[i].Decamelize().ToLower()];
                        item.Add(new KeyValuePair<string, object>(column, val));
                    }
                    resDto.Add(item);
                }
            }
            return resDto;
        }
        /// <summary>
        /// Sets the specified field.
        /// </summary>
        /// <param name="column">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleSelect<T> Set(string column, object value)
        {
            this._container.Enqueue(new Tuple<string, object>(column.Decamelize(true), value));
            return this;
        }
        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        public SimpleSelect<T> Min(params string[] fields)
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
        public SimpleSelect<T> Max(params string[] fields)
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
        public SimpleSelect<T> Sum(params string[] fields)
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
        public SimpleSelect<T> OrderBy(params string[] fields)
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
        public SimpleSelect<T> GroupBy(params string[] fields)
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
        #endregion Querry

        #region Non-Querry
        public int Insert(T t)
        {
            this.ParseToParam(this._container, t);
            return this.Insert();
        }
        public int Insert()
        {
            SqlCommand cmd = new SqlCommand(this.GetSimpleInsertQuerry());
            while (this._container.Count > 0)
            {
                Tuple<string, object> param = (Tuple<string, object>)this._container.Dequeue();
                cmd.Parameters.Add(SqlHelper.CreateParameter(param.Item1, param.Item2));
                Trace.WriteLine(param);
            }
            return this.Service.Context.ExecuteNonQuerySql(cmd);
        }
        public int Update(T t)
        {
            this.ParseToParam(this._container, t);
            return this.Update();
        }
        public int Update()
        {
            SqlCommand cmd = new SqlCommand();
            IList<SqlParameter> SqlParameters = new List<SqlParameter>();
            var sql = this.GetSimpleUpdateQuerry();
            while (this._container.Count > 0)
            {
                Tuple<string, object> param = (Tuple<string, object>)this._container.Dequeue();
                SqlParameters.Add(SqlHelper.CreateParameter(param.Item1, param.Item2));
                Trace.WriteLine(param);
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
            Trace.WriteLine("- [Querry] :");
            Trace.WriteLine(sql);
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(SqlParameters.ToArray());
            var result = this.Service.Context.ExecuteNonQuerySql(cmd);
            return result;
        }
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
            #region Log
#if DEBUG
            {
                Trace.WriteLine(" - [SQL]:");
                Trace.WriteLine(sql);
            }
#endif
            #endregion
            var result = this.Service.Context.ExecuteNonQuerySql(cmd);
            return result;
        }
        #endregion

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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetListResult()
        {
            try
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
                #endregion

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
                #endregion

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
                #endregion
                
                cmd.CommandText = sql;
                #region Log
#if DEBUG
                {
                    Trace.WriteLine(" - [SQL] :\r\n" + sql);
                    if (this.Where != null)
                    {
                        Trace.WriteLine(" - [SqlParameters] :");
                        foreach (var item in this.Where.SqlParameters)
                        {
                            Trace.WriteLine(string.Format("{0} : {1}", item, item.Value));
                        }
                    }
                    
                }
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
        private string GetSimpleSelectQuerry()
        {
            T t = Activator.CreateInstance<T>();
            StringBuilder sql = new StringBuilder();
            var tableName = StringHelper.Me.ToTableName<T>();
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
        private string GetSimpleDeleteQuerry()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("UPDATE ");
            sql.AppendLine(StringHelper.Me.ToTableName<T>() + " ");
            sql.AppendLine("SET ");
            sql.Append("\t  ");
            sql.AppendLine("delete_flag = 1 ");
            return sql.ToString();
        }
        private string GetSimpleInsertQuerry()
        {
            T t = Activator.CreateInstance<T>();
            StringBuilder sql = new StringBuilder();
            var tableName = StringHelper.Me.ToTableName<T>();
            var columns = t.GetColumNames().ToArray();
            string cols = string.Join(", ", columns);
            sql.AppendFormat("INSERT INTO {0}(", tableName);
            sql.AppendLine(cols + ") ");
            sql.Append("VALUES(");
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = SqlHelper.BuildParameterName(columns[i]);
            }
            string param = string.Join(", ", columns);
            sql.AppendLine(param + ") ");
            
            #region Log
#if DEBUG
            {
                Trace.WriteLine(" - [COL]:");
                Trace.WriteLine(sql);
            }
#endif
            #endregion
            return sql.ToString();
        }
        private string GetSimpleUpdateQuerry()
        {
            StringBuilder sql = new StringBuilder();
            var tableName = StringHelper.Me.ToTableName<T>();
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
        #endregion

        #region Private
        /// <summary>
        /// Parses to parameter.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="t">The object model.</param>
        private void ParseToParam(Queue<object> queue , T t)
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
        #endregion
    }
}
