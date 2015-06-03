using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simple.bus.core.context
{
    /// <summary>
    /// Business context
    /// </summary>
    public interface IBContext
    {
        void BeginTransaction();
        void BeginTransaction(IsolationLevel isolationLevel);
        Boolean Commit();
        Boolean Rollback();
        int ExecuteNonQuerySql(SqlCommand cmd);
        object ExecuteScalar(SqlCommand cmd);
        DbDataReader ExecuteReader(SqlCommand cmd);
        DataTable GetDataTable(SqlCommand cmd);
        IEnumerable<T> GetData<T>(SqlCommand cmd);
    }

    internal class DBContext : IBContext, IDisposable
    {
        #region 内部変数

        private string _connectionString;
        private SqlConnection sqlCon;
        private SqlTransaction sqlTran;

        #endregion 内部変数

        #region 外部公開メンバ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DBContext()
        {
            this._connectionString = @"Data Source=192.168.1.36\SQLEXPRESS;Initial Catalog=IM20130724;User ID=sa;Password=123456;Connection Timeout=90";
            this.Connect();
        }

        #endregion 外部公開メンバ

        #region メソッド

        /// <summary>
        /// データベースへ接続する
        /// </summary>
        /// <returns></returns>
        private Boolean Connect()
        {
            return dbConnect(this._connectionString);
        }

        /// <summary>
        /// データベースを切断
        /// </summary>
        private void DisConnect()
        {
            this.sqlCon.Close();
            this.sqlTran = null;
        }

        /// <summary>
        /// データベースへ接続する
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private Boolean Connect(string connectionString)
        {
            return dbConnect(connectionString);
        }        
        #endregion メソッド

        #region 内部処理

        private Boolean dbConnect(string connectionString)
        {
            this.sqlCon = new SqlConnection(connectionString);
            this.sqlCon.Open();
            return true;
        }

        #endregion 内部処理

        #region IDisposable メンバー

        public void Dispose()
        {
            try
            {
                if (sqlTran != null)
                {
                    this.sqlTran.Rollback();
                    this.sqlTran = null;
                }
            }
            catch (Exception ex)
            {
                this.sqlTran = null;
                throw ex;
            }
        }

        #endregion IDisposable メンバー

        #region IBContext Members

        /// <summary>
        /// トランザクション開始
        /// </summary>
        void IBContext.BeginTransaction()
        {
            this.sqlTran = this.sqlCon.BeginTransaction();
        }

        /// <summary>
        /// トランザクション開始
        /// </summary>
        /// <param name="isolationLevel"></param>
        void IBContext.BeginTransaction(IsolationLevel isolationLevel)
        {
            this.sqlTran = this.sqlCon.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// トランザクションコミット
        /// </summary>
        /// <returns></returns>
        Boolean IBContext.Commit()
        {
            try
            {
                if (sqlTran != null)
                {
                    this.sqlTran.Commit();
                    this.sqlTran = null;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                this.sqlTran = null;
                throw ex;
            }
        }

        /// <summary>
        /// トランザクションロールバック
        /// </summary>
        /// <returns></returns>
        Boolean IBContext.Rollback()
        {
            try
            {
                if (this.sqlTran != null)
                {
                    this.sqlTran.Rollback();
                    this.sqlTran = null;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                this.sqlTran = null;
                throw ex;
            }
        }

        /// <summary>
        /// SELECT結果をデータテーブルで返す
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>DataTable</returns>
        DataTable IBContext.GetDataTable(SqlCommand cmd)
        {
            SqlDataAdapter da = new SqlDataAdapter();
            cmd.Connection = this.sqlCon;
            if (this.sqlTran != null) cmd.Transaction = this.sqlTran;
            da.SelectCommand = cmd;

            DataSet ds = new DataSet();
            da.Fill(ds, "sqlout");

            return ds.Tables[0];
        }

        /// <summary>
        /// 戻り値の無いＳＱＬを実行
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        int IBContext.ExecuteNonQuerySql(SqlCommand cmd)
        {
            cmd.Connection = this.sqlCon;
            if (this.sqlTran != null) cmd.Transaction = this.sqlTran;
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// コマンドを実行し、結果セットの最初の行の最初の列の値を返します。
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        object IBContext.ExecuteScalar(SqlCommand cmd)
        {
            cmd.Connection = this.sqlCon;
            if (this.sqlTran != null) cmd.Transaction = this.sqlTran;
            return cmd.ExecuteScalar();
        }


        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns></returns>
        DbDataReader IBContext.ExecuteReader(SqlCommand cmd)
        {
            cmd.Connection = this.sqlCon;
            if (this.sqlTran != null) cmd.Transaction = this.sqlTran;
            return cmd.ExecuteReader();
        }

        /// <summary>
        /// SELECT結果をデータテーブルで返す
        /// </summary>
        /// <param name="cmd"SqlCommand></param>
        /// <returns>IEnumerable Of T</returns>
        IEnumerable<T> IBContext.GetData<T>(SqlCommand cmd)
        {
            IList<T> results = new List<T>();
            cmd.Connection = this.sqlCon;
            if (this.sqlTran != null) cmd.Transaction = this.sqlTran;
            lock (this)
            {
                using (DbDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        yield return (T)typeof(T).GetConstructor(new System.Type[] { typeof(DbDataReader) }).Invoke(new object[] { dr });
                    }
                }
            }
            //return results;
        }

        #endregion
    }
}
