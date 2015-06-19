using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using hpsofts.Extension;

namespace simple.sql
{
    /// <summary>
    /// Simple where
    /// </summary>
    public sealed class SimpleWhere
    {
        private const string AND_TERM = "AND";

        private const string OR_TERM = "OR";

        private readonly Queue<KeyValuePair<string, KeyValuePair<operators, object>>> _where;

        public SimpleWhere()
            : base()
        {
            this._where = new Queue<KeyValuePair<string, KeyValuePair<operators, object>>>();
            this.SqlParameters = new List<SqlParameter>();
        }

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

        internal IEnumerable<SqlParameter> SqlParameters { private set; get; }

        public SimpleWhere Btw(string field, object value1, object value2)
        {
            this.AddOpr(operators.Btw, field, new object[] { value1, value2 });
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
        /// Greater or Equal the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public SimpleWhere Ge(string field, object value)
        {
            this.AddOpr(operators.Ge, field, value);
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
            this.AddOpr(operators.Gt, field, value);
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
            this.AddOpr(operators.In, field, values);
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
            this.AddOpr(operators.Le, field, value);
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
        /// Not in array the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public SimpleWhere Nin(string field, params object[] values)
        {
            this.AddOpr(operators.Nin, field, values);
            return this;
        }

        /// <summary>
        /// Or the specified where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public SimpleWhere Or(SimpleWhere simpleWhere)
        {
            foreach (var keyPair in simpleWhere._where)
            {
                //KeyValuePair<string, KeyValuePair<operators, object>> keyPair = simpleWhere._where.Dequeue();
                KeyValuePair<operators, object> valuePair = keyPair.Value;
                if (valuePair.Key == operators.Or)
                {
                    //this.Or((SimpleWhere)valuePair.Value);
                }
                else
                {
                    SqlParameter[] sqlParams = (SqlParameter[])valuePair.Value;
                    string reg = @"_(\d+)$";
                    foreach (var s in sqlParams)
                    {
                        var orgName = Regex.Replace(s.ParameterName, reg, "");
                        var count = this.SqlParameters.Count(prm => prm.ParameterName.StartsWith(orgName));
                        count = count + 1;
                        s.ParameterName = string.Format("{0}_{1}", orgName, count);
                        this.SqlParameters = this.SqlParameters.Add(s);
                    }
                }
            }
            this._where.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(string.Empty,
                    new KeyValuePair<operators, object>(operators.Or, simpleWhere)));
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
        /// To the SQL.
        /// </summary>
        /// <returns></returns>
        internal string ToSql()
        {
            return this.ToSql(AND_TERM);
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
            SqlParameter[] param;
            if (value == null || value.GetType() == typeof(DBNull))
            {
                value = DBNull.Value;
                param = new SqlParameter[1];
                param[0] = SqlHelper.CreateParameter(parameterName, dbType, value);
            }
            else
            {
                if (!value.GetType().IsArray)
                {
                    param = new SqlParameter[1];
                    param[0] = SqlHelper.CreateParameter(parameterName, dbType, value);
                }
                else
                {
                    object[] arr = (object[])value;
                    param = new SqlParameter[arr.Length];

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
                        param[i] = SqlHelper.CreateParameter(parameterName, dbType, arr[i]);
                    }
                }
            }

            this.SqlParameters = this.SqlParameters.AddRange(param);
            this._where.Enqueue(new KeyValuePair<string, KeyValuePair<operators, object>>(field.Decamelize(true),
                new KeyValuePair<operators, object>(opr, param)));
        }
        /// <summary>
        /// To the SQL.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
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
                        where = string.Format("{0} = {1}", item.Key, ((SqlParameter[])item.Value.Value)[0].ParameterName);
                        i++;
                        break;

                    case operators.Neq:
                        where = string.Format("{0} <> {1}", item.Key, ((SqlParameter[])item.Value.Value)[0].ParameterName);
                        i++;
                        break;

                    case operators.Lt:
                        where = string.Format("{0} < {1}", item.Key, ((SqlParameter[])item.Value.Value)[0].ParameterName);
                        i++;
                        break;

                    case operators.Le:
                        where = string.Format("{0} <= {1}", item.Key, ((SqlParameter[])item.Value.Value)[0].ParameterName);
                        i++;
                        break;

                    case operators.Gt:
                        where = string.Format("{0} > {1}", item.Key, ((SqlParameter[])item.Value.Value)[0].ParameterName);
                        i++;
                        break;

                    case operators.Ge:
                        where = string.Format("{0} >= {1}", item.Key, ((SqlParameter[])item.Value.Value)[0].ParameterName);
                        i++;
                        break;

                    case operators.Stw:
                    case operators.EnW:
                    case operators.Ctn:
                        where = string.Format("{0} LIKE {1}", item.Key, ((SqlParameter[])item.Value.Value)[0].ParameterName);
                        i++;
                        break;

                    case operators.In:
                    case operators.Nin:
                        var inrange = (SqlParameter[])item.Value.Value;
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
                                where += string.Format(", {0}", inrange[next].ParameterName);
                            }
                            else
                            {
                                where += string.Format("{0}{1}", opt, inrange[next].ParameterName);
                            }
                            i++;
                        }
                        where += ")";
                        break;

                    case operators.Btw:
                        var from = ((SqlParameter[])item.Value.Value)[0];
                        var to = ((SqlParameter[])item.Value.Value)[1];
                        where = string.Format("{0} BETWEEN ", item.Key);
                        where += from.ParameterName;
                        i++;
                        where += " AND ";
                        where += to.ParameterName;
                        i++;
                        break;

                    case operators.Or:
                        var sub = (SimpleWhere)item.Value.Value;
                        if (sub.SqlParameters.Count() == 1)
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
    }
}