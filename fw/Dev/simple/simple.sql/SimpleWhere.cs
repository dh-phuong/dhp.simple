using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using simple.helper;

namespace simple.sql
{

    public sealed class SimpleWhere
    {
        public enum operators
        {
            none = 0,
            Eq = 1,
            Neq = 2,
            Lt = 3,
            Gt = 4,
            Le = 5,
            Ge = 6,
            Stw = 8,
            EnW = 9,
            Ctn = 10,
            In = 11,
            Nin = 12,
            Btw = 13,
            Or = 14,
        }
        private readonly Queue<KeyValuePair<string, KeyValuePair<operators, object>>> _where;
        internal readonly IList<SqlParameter> SqlParameters;
        private const string AND_TERM = "AND";
        private const string OR_TERM = "OR";
        public SimpleWhere()
            : base()
        {
            this._where = new Queue<KeyValuePair<string, KeyValuePair<operators, object>>>();
            this.SqlParameters = new List<SqlParameter>();
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere Eq(string field, object value)
        {
            this.AddOpr(operators.Eq, field, value);
            return this;
        }
        /// <summary>
        /// Not Equals the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere Neq(string field, object value)
        {
            this.AddOpr(operators.Neq, field, value);
            return this;
        }
        /// <summary>
        /// Less then the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere Lt(string field, object value)
        {
            this.AddOpr(operators.Lt, field, value);
            return this;
        }
        /// <summary>
        /// Greater the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere Gt(string field, object value)
        {
            //this.where_.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(field.Decamelize().ToLower(), new KeyValuePair<operators, object>(operators.Gt, value)));
            this.AddOpr(operators.Gt, field, value);
            return this;
        }
        /// <summary>
        /// Less then or Equal the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere Le(string field, object value)
        {
            //this.where_.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(field.Decamelize().ToLower(), new KeyValuePair<operators, object>(operators.Le, value)));
            this.AddOpr(operators.Le, field, value);
            return this;
        }
        /// <summary>
        /// Greater or Equal the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere Ge(string field, object value)
        {
            //this.where_.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(field.Decamelize().ToLower(), new KeyValuePair<operators, object>(operators.Ge, value)));
            this.AddOpr(operators.Ge, field, value);
            return this;
        }
        /// <summary>
        /// Start with the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere StartWith(string field, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = string.Format("%{0}", value);
            }
            this.AddOpr(operators.Stw, field, value);
            return this;
        }
        /// <summary>
        /// End with the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere EndWith(string field, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = string.Format("{0}%", value);
            }
            this.AddOpr(operators.EnW, field, value);
            return this;
        }
        /// <summary>
        /// Contains the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere Contains(string field, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = string.Format("%{0}%", value);
            }
            this.AddOpr(operators.Ctn, field, value);
            return this;
        }

        /// <summary>
        /// In array the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public SimpleWhere In(string field, params object[] values)
        {
            //this.where_.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(field.Decamelize().ToLower(), new KeyValuePair<operators, object>(operators.In, values)));
            this.AddOpr(operators.In, field, values);
            return this;
        }
        /// <summary>
        /// Not in array the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public SimpleWhere Nin(string field, params object[] values)
        {
            //this.where_.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(field.Decamelize().ToLower(), new KeyValuePair<operators, object>(operators.Nin, values)));
            this.AddOpr(operators.Nin, field, values);
            return this;
        }
        public SimpleWhere Btw(string field, object value1, object value2)
        {
            //this.where_.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(field.Decamelize().ToLower(), new KeyValuePair<operators, object>(operators.Btw, new object[] { value1, value2 })));
            this.AddOpr(operators.Btw, field, new object[] { value1, value2 });
            return this;
        }
        /// <summary>
        /// Or the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public SimpleWhere Or(SimpleWhere where)
        {
            this._where.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>
                               (string.Empty, new KeyValuePair<operators, object>(operators.Or, where)));
            where.SqlParameters.AsParallel<SqlParameter>().ForAll(s => 
            {
                string reg = @"_(\d+)$";
                var orgName = Regex.Replace(s.ParameterName, reg, "");
                lock (this)
                {
                    var count = this.SqlParameters.Count(prm => prm.ParameterName.StartsWith(orgName));
                    if (count != 0)
                    {
                        s.ParameterName = string.Format("{0}_{1}", orgName, count + 1);
                    }
                    this.SqlParameters.Add(s);
                }
            });
            return this;
        }

        /// <summary>
        /// Adds the operators
        /// </summary>
        /// <param name="opr">The opr.</param>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        private void AddOpr(operators opr, string field, object value)
        {
            SqlDbType dbType = SqlDbType.NVarChar;
            var parameterName = SqlHelper.BuildParameterName(field.Decamelize(true));
            var count = this.SqlParameters.Count(s => s.ParameterName.StartsWith(parameterName));
            count = count + 1;
            parameterName = string.Format("{0}_{1}", parameterName, count);

            if (value == null || value.GetType() == typeof(DBNull))
            {
                value = DBNull.Value;
                this.SqlParameters.Add(SqlHelper.CreateParameter(parameterName, dbType, value));
            }
            else
            {
                if (!value.GetType().IsArray)
                {
                    this.SqlParameters.Add(SqlHelper.CreateParameter(parameterName, value.GetType().GetDBType(), value));
                }
                else
                {
                    object[] arr = (object[])value;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        parameterName = string.Format("{0}_{1}", SqlHelper.BuildParameterName(field.Decamelize(true)), count + i);
                        if (arr[i] == null || arr[i].GetType() == typeof(DBNull))
                        {
                            arr[i] = DBNull.Value;
                        }
                        else
                        {
                            dbType = arr[i].GetType().GetDBType();
                        }
                        this.SqlParameters.Add(SqlHelper.CreateParameter(parameterName, dbType, arr[i]));
                    }
                }
            }
            this._where.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(field.Decamelize(true), new KeyValuePair<operators, object>(opr, value)));
        }
        private string ToSql(string term)
        {
            StringBuilder sql = new StringBuilder();
            int i = 0;
            foreach (var item in this._where)
            {
                string where = string.Empty;
                string fieldName = item.Key;
                if (item.Key.IndexOf('.') != -1)
                {
                    fieldName = item.Key.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                switch (item.Value.Key)
                {
                    case operators.Eq:
                        where = string.Format("{0} = {1}", item.Key, (SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i))));
                        i++;
                        break;
                    case operators.Neq:
                        where = string.Format("{0} <> {1}", item.Key, (SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i))));
                        i++;
                        break;
                    case operators.Lt:
                        where = string.Format("{0} < {1}", item.Key, (SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i))));
                        i++;
                        break;
                    case operators.Le:
                        where = string.Format("{0} <= {1}", item.Key, (SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i))));
                        i++;
                        break;
                    case operators.Gt:
                        where = string.Format("{0} > {1}", item.Key, (SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i))));
                        i++;
                        break;
                    case operators.Ge:
                        where = string.Format("{0} >= {1}", item.Key, (SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i))));
                        i++;
                        break;
                    case operators.Stw:
                    case operators.EnW:
                    case operators.Ctn:
                        where = string.Format("{0} LIKE {1}", item.Key, (SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i))));
                        i++;
                        break;
                    case operators.In:
                    case operators.Nin:
                        var inrange = (object[])item.Value.Value;
                        string opt = string.Empty;

                        if (item.Value.Key == operators.In)
                        {
                            opt = string.Format("{0} IN(", item.Key);
                        }
                        else
                        {
                            opt = string.Format("{0} NOT IN(", item.Key);
                        }

                        for (int next = 0; next < inrange.Length; next++)
                        {
                            if (next != 0)
                            {
                                where += string.Format(", {0}", SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i)));
                            }
                            else
                            {
                                where += string.Format("{0}{1}", opt, SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i)));
                            }
                            i++;
                        }
                        where += ")";
                        break;
                    case operators.Btw:
                        var from = ((object[])item.Value.Value)[0];
                        var to = ((object[])item.Value.Value)[1];
                        where = string.Format("{0} BETWEEN ", item.Key);
                        where += SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i));
                        i++;
                        where += " AND ";
                        where += SqlHelper.BuildParameterName(string.Format("{0}_{1}", fieldName, i));
                        i++;
                        break;
                    case operators.Or:
                        var sub = (SimpleWhere)item.Value.Value;
                        if (sub.SqlParameters.Count == 1)
                        {
                            sql.AppendLine(string.Format("OR {0}", sub.ToSql()));
                        }
                        else
                        {
                            sql.AppendLine(string.Format("{0} ({1})", term, sub.ToSql(OR_TERM)));
                        }

                        break;
                    default:
                        break;
                }
                if (sql.Length == 0)
                {
                    sql.AppendLine(where);
                }
                else if (!string.IsNullOrEmpty(where))
                {
                    sql.AppendLine(string.Format("{0} {1}", term, where));
                }
            }
           
            return sql.ToString();
        }
        
        internal string ToSql()
        {
            return this.ToSql(AND_TERM);
        }
    }
}
