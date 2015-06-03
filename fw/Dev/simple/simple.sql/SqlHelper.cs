using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;


using System.Data.SqlClient;
using System.Text;
using simple.helper;

namespace simple.sql
{

    /// <summary> 
    /// SQL Server データベース接続に必要な機能を提供します。このクラスはインスタンス化できません。 
    /// </summary> 
    public sealed class SqlHelper
    {
        private SqlHelper()
        {
        }

        #region "クラス変数"

        /// <summary> 
        /// パラメータ文字列の記号を表します。 
        /// </summary> 

        public const char ParameterToken = '@';
        /// <summary> 
        /// SqlParameter 配列をキャッシュする容量の初期値を表します。 
        /// </summary> 

        private const int cacheParametersCapacity = 10;
        /// <summary> 
        /// SqlParameter 配列を格納する辞書 (キーと値のコレクション) を表します。 
        /// </summary> 

        private static Dictionary<string, SqlParameter[]> cachedPrms = new Dictionary<string, SqlParameter[]>(cacheParametersCapacity);
        /// <summary> 
        /// 接続タイムアウトを表します。 
        /// </summary> 

        private static int m_timeout = 15;
        /// <summary> 
        /// パケット サイズを表します。 
        /// </summary> 

        private const int packetSize = 8000;
        #endregion

        #region "静的プロパティ"

        /// <summary> 
        /// 規定の接続タイムアウトを取得または設定します。 
        /// </summary> 
        public static int Timeout
        {
            get { return m_timeout; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "value が有効範囲外です。0 以上の値を指定してください。");
                }

                m_timeout = value;
            }
        }

        #endregion

        #region "静的メソッド"

        #region "BuildParameterName メソッド"

        /// <summary> 
        /// 指定された名称のパラメータ文字列を生成し返します。 
        /// </summary> 
        /// <param name="name">パラメータ名</param> 
        /// <returns>パラメータ文字列</returns> 
        public static string BuildParameterName(string name)
        {

            if (name == null)
            {
                throw new ArgumentNullException("name", "name パラメータが null (Visual Basic in Nothing) 参照です。");
            }
            else if (name.Length == 0)
            {
                throw new ArgumentException("name パラメータが未指定です。");
            }

            // 先頭がパラメータ文字以外の場合は先頭に付加 
            if (name[0] != ParameterToken)
            {
                name = string.Format("{0}in_{1}", ParameterToken, name);
            }
            return name;
        }

        #endregion

        #region "CreateConnectionString メソッド"

        /// <summary> 
        /// 指定された値を使用して接続文字列を生成し、返します (Windows 認証を使用する場合は userId と password に null を指定してください) 。 
        /// </summary> 
        /// <param name="instanceName">インスタンス名</param> 
        /// <param name="databaseName">データベース名</param> 
        /// <param name="userId">ユーザー ID</param> 
        /// <param name="password">パスワード</param> 
        /// <param name="connectTimeout">接続タイムアウト</param> 
        /// <returns>接続文字列</returns> 
        /// <exception cref="ArgumentNullException">instanceName, databaseName パラメータのいずれかが null 参照の場合に発生します。</exception> 
        /// <remarks>userId パラメータと password パラメータが共に null 参照の場合は Windows 認証を使用した接続文字列を返します。</remarks> 
        public static string CreateConnectionString(string instanceName, string databaseName, string userId, string password, int connectTimeout)
        {
            SqlConnectionStringBuilder scsb = null;
            System.Reflection.Assembly asm = null;

            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName パラメータが null 参照です。");
            }
            else if (databaseName == null)
            {
                throw new ArgumentNullException("databaseName パラメータが null 参照です。");
            }

            try
            {
                // userId と password が共に null 参照の場合は Windows 認証 (true) を使用する。それ以外は SQL Server 認証 (false) を使用する 
                bool integratedSecurity = (userId == null && password == null);

                // リフレクタからエントリ アセンブリを取得 
                asm = System.Reflection.Assembly.GetEntryAssembly();

                // 接続文字列生成 
                scsb = new SqlConnectionStringBuilder();
                scsb.DataSource = instanceName;
                scsb.InitialCatalog = databaseName;
                scsb.IntegratedSecurity = integratedSecurity;

                if (!integratedSecurity)
                {
                    scsb.UserID = userId;
                    scsb.Password = password;
                }

                scsb.ConnectTimeout = connectTimeout;
                scsb.PersistSecurityInfo = true;
                scsb.PacketSize = packetSize;
                scsb.AsynchronousProcessing = true;

                scsb.ApplicationName = asm.ManifestModule.Name;

                return scsb.ToString();
            }
            finally
            {
                asm = null;
                scsb = null;
            }
        }

        /// <summary> 
        /// 指定された値と規定の接続試行時間を使用して接続文字列を生成し、返します (Windows 認証を使用する場合は userId と password に null を指定してください) 。 
        /// </summary> 
        /// <param name="instanceName">インスタンス名</param> 
        /// <param name="databaseName">データベース名</param> 
        /// <param name="userId">ユーザー ID</param> 
        /// <param name="password">パスワード</param> 
        /// <returns>接続文字列</returns> 
        public static string CreateConnectionString(string instanceName, string databaseName, string userId, string password)
        {
            return CreateConnectionString(instanceName, databaseName, userId, password, Timeout);
        }

        #endregion

        #region "CreateParameter メソッド"
        /// <summary> 
        /// SqlParameter インスタンスを生成します。 
        /// </summary> 
        /// <param name="parameterName">パラメータ名</param> 
        /// <param name="dbType">パラメータの種類</param> 
        /// <param name="value">パラメータの値</param> 
        /// <returns>SqlParameter インスタンス</returns> 
        public static SqlParameter CreateParameter(string parameterName, object value)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName", "parameterName パラメータが null (Nothing in Visual Basic) 参照です。");
            }
            else if (parameterName.Length == 0)
            {
                throw new ArgumentException("parameterName パラメータが未指定です。", "parameterName");
            }
            SqlDbType dbType = SqlDbType.NVarChar;
            if (value == null || value.GetType() == typeof(DBNull))
            {
                value = DBNull.Value;
            }
            else
            {
                dbType = value.GetType().GetDBType();
            }
            SqlParameter prm = new SqlParameter(SqlHelper.BuildParameterName(parameterName), dbType);
            prm.IsNullable = true;
            prm.Value = value;
            return prm;
        }

        /// <summary> 
        /// SqlParameter インスタンスを生成します。 
        /// </summary> 
        /// <param name="parameterName">パラメータ名</param> 
        /// <param name="dbType">パラメータの種類</param> 
        /// <param name="value">パラメータの値</param> 
        /// <returns>SqlParameter インスタンス</returns> 
        public static SqlParameter CreateParameter(string parameterName, SqlDbType dbType, object value)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName", "parameterName パラメータが null (Nothing in Visual Basic) 参照です。");
            }
            else if (parameterName.Length == 0)
            {
                throw new ArgumentException("parameterName パラメータが未指定です。", "parameterName");
            }
            SqlParameter prm = new SqlParameter(BuildParameterName(parameterName), dbType);
            prm.IsNullable = true;
            prm.Value = value;
            return prm;
        }

        /// <summary> 
        /// SqlParameter インスタンスを生成します。 
        /// </summary> 
        /// <param name="parameterName">パラメータ名</param> 
        /// <param name="dbType">パラメータの種類</param> 
        /// <param name="size">パラメータのサイズ</param> 
        /// <param name="direction">パラメータの方向</param> 
        /// <param name="value">パラメータの値</param> 
        /// <returns>SqlParameter インスタンス</returns> 
        public static SqlParameter CreateParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction, object value)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName", "parameterName パラメータが null (Nothing in Visual Basic) 参照です。");
            }
            else if (parameterName.Length == 0)
            {
                throw new ArgumentException("parameterName パラメータが未指定です。", "parameterName");
            }

            if (direction == ParameterDirection.Input || direction == ParameterDirection.InputOutput)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "value パラメータが null (Nothing in Visual Basic) 参照です。");
                }
            }
            
            SqlParameter prm = new SqlParameter(BuildParameterName(parameterName), dbType, size);
            prm.Direction = direction;
            prm.IsNullable = true;
            prm.Value = value;

            return prm;
        }

        /// <summary> 
        /// SqlParameter インスタンスを生成します。パラメータの値には DBNull を規定値とします。 
        /// </summary> 
        /// <param name="parameterName">パラメータ名</param> 
        /// <param name="dbType">パラメータの種類</param> 
        /// <param name="size">パラメータのサイズ</param> 
        /// <param name="direction">パラメータの方向</param> 
        /// <returns>SqlParameter インスタンス</returns> 
        public static SqlParameter CreateParameter(string parameterName, SqlDbType dbType, int size, ParameterDirection direction)
        {
            return CreateParameter(parameterName, dbType, size, direction, DBNull.Value);
        }

        /// <summary> 
        /// SqlParameter インスタンスを生成します。 
        /// </summary> 
        /// <param name="parameterName">パラメータ名</param> 
        /// <param name="dbType">パラメータの種類</param> 
        /// <param name="size">パラメータのサイズ</param> 
        /// <param name="precision">桁数 (整数部＋小数部)</param> 
        /// <param name="scale">小数部桁数</param> 
        /// <param name="direction">パラメータの方向</param> 
        /// <param name="value">パラメータの値</param> 
        /// <returns>SqlParameter インスタンス</returns> 
        /// <remarks>このメソッドは dbType パラメータが SqlDbType.Decimal の場合に使用します。これ以外の場合は prevision パラメータと scale パラメータは無視されます。</remarks> 
        public static SqlParameter CreateParameter(string parameterName, SqlDbType dbType, int size, byte precision, byte scale, ParameterDirection direction, object value)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName", "parameterName パラメータが null (Nothing in Visual Basic) 参照です。");
            }
            else if (parameterName.Length == 0)
            {
                throw new ArgumentException("parameterName パラメータが未指定です。", "parameterName");
            }

            if (direction == ParameterDirection.Input || direction == ParameterDirection.InputOutput)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "value パラメータが null (Nothing in Visual Basic) 参照です。");
                }
            }

            SqlParameter prm = new SqlParameter(BuildParameterName(parameterName), dbType, size);
            prm.Direction = direction;
            prm.IsNullable = true;
            prm.Value = value;

            if (dbType == SqlDbType.Decimal)
            {
                prm.Precision = precision;
                prm.Scale = scale;
            }

            return prm;
        }

        /// <summary> 
        /// SqlParameter インスタンスを生成します。パラメータの値には DBNull を規定値とします。 
        /// </summary> 
        /// <param name="parameterName">パラメータ名</param> 
        /// <param name="dbType">パラメータの種類</param> 
        /// <param name="size">パラメータのサイズ</param> 
        /// <param name="precision">桁数 (整数部＋小数部)</param> 
        /// <param name="scale">小数部桁数</param> 
        /// <param name="direction">パラメータの方向</param> 
        /// <returns>SqlParameter インスタンス</returns> 
        /// <remarks>このメソッドは dbType パラメータが SqlDbType.Decimal の場合に使用します。これ以外の場合は prevision パラメータと scale パラメータは無視されます。</remarks> 
        public static SqlParameter CreateParameter(string parameterName, SqlDbType dbType, int size, byte precision, byte scale, ParameterDirection direction)
        {
            return CreateParameter(parameterName, dbType, size, precision, scale, direction, DBNull.Value);
        }

        #endregion

        #region "ExecuteNonQuery メソッド"

        /// <summary> 
        /// コマンドを実行し、影響を受けた行数を返します。 
        /// </summary> 
        /// <param name="conStr">接続文字列</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>影響を受けた行数</returns> 
        public static int ExecuteNonQuery(string conStr, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand cmd = CreateCommand(con, null, cmdType, cmdText, cmdPrms);
                    int val = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    return val;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// コマンドを実行し、影響を受けた行数を返します。 
        /// </summary> 
        /// <param name="con">接続</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>影響を受けた行数</returns> 
        public static int ExecuteNonQuery(SqlConnection con, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            bool closed = true;

            try
            {
                if (con == null)
                {
                    throw new ArgumentNullException("con", "con パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                closed = (con.State == ConnectionState.Closed);

                SqlCommand cmd = CreateCommand(con, null, cmdType, cmdText, cmdPrms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                return val;
            }
            catch (Exception ex)
            {
                if (closed)
                {
                    con.Close();
                }
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// コマンドを実行し、影響を受けた行数を返します。 
        /// </summary> 
        /// <param name="trans">トランザクション</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>影響を受けた行数</returns> 
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            try
            {
                if (trans == null)
                {
                    throw new ArgumentNullException("trans", "trans パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                SqlCommand cmd = CreateCommand(null, trans, cmdType, cmdText, cmdPrms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                return val;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        #endregion

        #region "ExecuteReader メソッド"

        /// <summary> 
        /// コマンドを実行し、SqlDataReader を返します。 
        /// </summary> 
        /// <param name="conStr">接続文字列</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>SqlDataReader インスタンス</returns> 
        public static SqlDataReader ExecuteReader(string conStr, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            SqlConnection con = null;
            SqlCommand cmd = null;

            try
            {
                if (conStr == null)
                {
                    throw new ArgumentNullException("conStr", "conStr パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                con = new SqlConnection(conStr);
                cmd = CreateCommand(con, null, cmdType, cmdText, cmdPrms);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();

                return dr;
            }
            catch (Exception ex)
            {
                if (con != null)
                {
                    con.Close();
                    con = null;
                }
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// コマンドを実行し、SqlDataReader を返します。 
        /// </summary> 
        /// <param name="con">接続</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>SqlDataReader インスタンス</returns> 
        public static SqlDataReader ExecuteReader(SqlConnection con, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            bool closed = true;

            try
            {
                if (con == null)
                {
                    throw new ArgumentNullException("con", "con パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                closed = (con.State == ConnectionState.Closed);

                SqlCommand cmd = CreateCommand(con, null, cmdType, cmdText, cmdPrms);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default);
                cmd.Parameters.Clear();

                return dr;
            }
            catch (Exception ex)
            {
                if (closed)
                {
                    con.Close();
                }
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// コマンドを実行し、SqlDataReader を返します。 
        /// </summary> 
        /// <param name="trans">トランザクション</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>SqlDataReader インスタンス</returns> 
        public static SqlDataReader ExecuteReader(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            try
            {
                if (trans == null)
                {
                    throw new ArgumentNullException("trans", "trans パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                SqlCommand cmd = CreateCommand(null, trans, cmdType, cmdText, cmdPrms);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default);
                cmd.Parameters.Clear();

                return dr;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        #endregion

        #region "ExecuteScalar メソッド"

        /// <summary> 
        /// コマンドを実行し、結果セットの最初の行の最初の列の値を返します。 
        /// </summary> 
        /// <param name="conStr">接続文字列</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>結果セットの最初の行の最初の列の値インスタンス</returns> 
        public static object ExecuteScalar(string conStr, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand cmd = CreateCommand(con, null, cmdType, cmdText, cmdPrms);
                    object val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();

                    return val;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// コマンドを実行し、結果セットの最初の行の最初の列の値を返します。 
        /// </summary> 
        /// <param name="con">接続文字列</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>結果セットの最初の行の最初の列の値インスタンス</returns> 
        public static object ExecuteScalar(SqlConnection con, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            bool closed = true;

            try
            {
                if (con == null)
                {
                    throw new ArgumentNullException("con", "con パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                closed = (con.State == ConnectionState.Closed);

                SqlCommand cmd = CreateCommand(con, null, cmdType, cmdText, cmdPrms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();

                return val;
            }
            catch (Exception ex)
            {
                if (closed)
                {
                    con.Close();
                }
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// コマンドを実行し、結果セットの最初の行の最初の列の値を返します。 
        /// </summary> 
        /// <param name="trans">トランザクション</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>結果セットの最初の行の最初の列の値インスタンス</returns> 
        public static object ExecuteScalar(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdPrms)
        {
            try
            {
                if (trans == null)
                {
                    throw new ArgumentNullException("trans", "trans パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                SqlCommand cmd = CreateCommand(null, trans, cmdType, cmdText, cmdPrms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();

                return val;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        #endregion

        #region "FillDataSet メソッド"

        /// <summary> 
        /// 指定された接続文字列を使用してコマンドを実行し、結果を <see cref="DataSet"/> で返します。 
        /// </summary> 
        /// <param name="conStr">接続文字列</param> 
        /// <param name="selectCmdType">コマンド文字列の解釈方法</param> 
        /// <param name="selectCmdText">コマンド文字列</param> 
        /// <param name="selectCmdPrms">コマンド パラメータ</param> 
        /// <returns>コマンドを実行した結果を格納した DataSet インスタンス</returns> 
        public static DataSet FillDataSet(string conStr, CommandType selectCmdType, string selectCmdText, params SqlParameter[] selectCmdPrms)
        {
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand selectCmd = CreateCommand(con, null, selectCmdType, selectCmdText, selectCmdPrms);
                    da = new SqlDataAdapter(selectCmd);

                    ds = new DataSet();
                    da.Fill(ds);
                    selectCmd.Parameters.Clear();

                    con.Close();
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// 指定されたコネクションを使用してコマンドを実行し、結果を <see cref="DataSet"/> で返します。 
        /// </summary> 
        /// <param name="con">接続</param> 
        /// <param name="selectCmdType">コマンド文字列の解釈方法</param> 
        /// <param name="selectCmdText">コマンド文字列</param> 
        /// <param name="selectCmdPrms">コマンド パラメータ</param> 
        /// <returns>コマンドを実行した結果を格納した DataSet インスタンス</returns> 
        public static DataSet FillDataSet(SqlConnection con, CommandType selectCmdType, string selectCmdText, params SqlParameter[] selectCmdPrms)
        {
            bool closed = (con.State == ConnectionState.Closed);
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                SqlCommand selectCmd = CreateCommand(con, null, selectCmdType, selectCmdText, selectCmdPrms);
                da = new SqlDataAdapter(selectCmd);

                ds = new DataSet();
                da.Fill(ds);
                selectCmd.Parameters.Clear();

                if (closed)
                {
                    con.Close();
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// 指定されたトランザクションを使用してコマンドを実行し、結果を <see cref="DataSet"/> で返します。 
        /// </summary> 
        /// <param name="trans">トランザクション</param> 
        /// <param name="selectCmdType">コマンド文字列の解釈方法</param> 
        /// <param name="selectCmdText">コマンド文字列</param> 
        /// <param name="selectCmdPrms">コマンド パラメータ</param> 
        /// <returns>コマンドを実行した結果を格納した DataSet インスタンス</returns> 
        public static DataSet FillDataSet(SqlTransaction trans, CommandType selectCmdType, string selectCmdText, params SqlParameter[] selectCmdPrms)
        {
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                if (trans == null)
                {
                    throw new ArgumentNullException("trans", "trans パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                SqlCommand selectCmd = CreateCommand(null, trans, selectCmdType, selectCmdText, selectCmdPrms);
                da = new SqlDataAdapter(selectCmd);

                ds = new DataSet();
                da.Fill(ds);
                selectCmd.Parameters.Clear();

                return ds;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        #endregion

        #region "FillSchemaDataSet メソッド"

        /// <summary> 
        /// 指定された接続文字列を使用してコマンドを実行し、スキーマの設定された <see cref="DataSet"/> を返します。 
        /// </summary> 
        /// <param name="conStr">接続文字列</param> 
        /// <param name="schemaType">スキーマ マップ処理方法</param> 
        /// <param name="selectCmdType">コマンド文字列の解釈方法</param> 
        /// <param name="selectCmdText">コマンド文字列</param> 
        /// <param name="selectCmdPrms">コマンド パラメータ</param> 
        /// <returns>コマンドを実行した結果を格納した DataSet インスタンス</returns> 
        public static DataSet FillSchemaDataSet(string conStr, SchemaType schemaType, CommandType selectCmdType, string selectCmdText, params SqlParameter[] selectCmdPrms)
        {
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                using (SqlConnection con = new SqlConnection(conStr))
                {
                    SqlCommand selectCmd = CreateCommand(con, null, selectCmdType, selectCmdText, selectCmdPrms);
                    da = new SqlDataAdapter(selectCmd);

                    ds = new DataSet();
                    da.FillSchema(ds, schemaType);
                    selectCmd.Parameters.Clear();

                    con.Close();
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// 指定されたコネクションを使用してコマンドを実行し、スキーマの設定された <see cref="DataSet"/> を返します。 
        /// </summary> 
        /// <param name="con">コネクション</param> 
        /// <param name="schemaType">スキーマ マップ処理方法</param> 
        /// <param name="selectCmdType">コマンド文字列の解釈方法</param> 
        /// <param name="selectCmdText">コマンド文字列</param> 
        /// <param name="selectCmdPrms">コマンド パラメータ</param> 
        /// <returns>コマンドを実行した結果を格納した DataSet インスタンス</returns> 
        public static DataSet FillSchemaDataSet(SqlConnection con, SchemaType schemaType, CommandType selectCmdType, string selectCmdText, params SqlParameter[] selectCmdPrms)
        {
            bool closed = (con.State == ConnectionState.Closed);
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                SqlCommand selectCmd = CreateCommand(con, null, selectCmdType, selectCmdText, selectCmdPrms);
                da = new SqlDataAdapter(selectCmd);

                ds = new DataSet();
                da.FillSchema(ds, schemaType);
                selectCmd.Parameters.Clear();

                if (closed)
                {
                    con.Close();
                }

                return ds;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        /// <summary> 
        /// 指定されたトランザクションを使用してコマンドを実行し、スキーマの設定された <see cref="DataSet"/> を返します。 
        /// </summary> 
        /// <param name="trans">トランザクション</param> 
        /// <param name="schemaType">スキーマ マップ処理方法</param> 
        /// <param name="selectCmdType">コマンド文字列の解釈方法</param> 
        /// <param name="selectCmdText">コマンド文字列</param> 
        /// <param name="selectCmdPrms">コマンド パラメータ</param> 
        /// <returns>コマンドを実行した結果を格納した DataSet インスタンス</returns> 
        public static DataSet FillSchemaDataSet(SqlTransaction trans, SchemaType schemaType, CommandType selectCmdType, string selectCmdText, params SqlParameter[] selectCmdPrms)
        {
            SqlDataAdapter da = null;
            DataSet ds = null;

            try
            {
                if (trans == null)
                {
                    throw new ArgumentNullException("trans", "trans パラメータが null (Nothing in Visual Basic) 参照です。");
                }

                SqlCommand selectCmd = CreateCommand(null, trans, selectCmdType, selectCmdText, selectCmdPrms);
                da = new SqlDataAdapter(selectCmd);

                ds = new DataSet();
                da.FillSchema(ds, schemaType);
                selectCmd.Parameters.Clear();

                return ds;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("コマンドの実行時にエラーが発生しました。", ex);
            }
        }

        #endregion

        #region "CreateCommand メソッド"

        /// <summary> 
        /// 指定されたパラメータで <see cref="SqlCommand"/> を生成し返します。 
        /// </summary> 
        /// <param name="con">コネクション</param> 
        /// <param name="trans">トランザクション</param> 
        /// <param name="cmdType">コマンド文字列の解釈方法</param> 
        /// <param name="cmdText">コマンド文字列</param> 
        /// <param name="cmdPrms">コマンド パラメータ</param> 
        /// <returns>SqlCommand インスタンス</returns> 
        /// <exception cref="System.ArgumentNullException">con パラメータと trans パラメータが共に null (Nothing in Visual Basic) 参照の場合に発生します。</exception> 
        /// <exception cref="System.ArgumentException">trans パラメータのトランザクションが開始されていないか、終了している場合に発生します。</exception> 
        static internal SqlCommand CreateCommand(SqlConnection con, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdPrms)
        {
            SqlCommand cmd = null;
            bool closed = false;

            try
            {
                // 引数チェック 
                if (con == null && trans == null)
                {
                    throw new ArgumentNullException("con or trans", "con パラメータと trans パラメータが共に null (Nothing in Visual Basic) 参照です。");
                }
                else if (con == null && trans != null && trans.Connection == null)
                {
                    throw new ArgumentException("trans パラメータのトランザクション処理は開始されてないか、既に終了しています。", "trans");
                }
                else if (cmdText == null)
                {
                    throw new ArgumentNullException("cmdText", "cmdText パラメータが null (Nothing in Visual Basic) 参照です。");
                }
                else if (cmdText.Length == 0)
                {
                    throw new ArgumentException("cmdText パラメータが未指定です。", "cmdText");
                }

                // 接続開始 
                if (con != null && con.State == ConnectionState.Closed)
                {
                    closed = true;
                    con.Open();
                }

                // コマンド生成 
                if (con != null)
                {
                    cmd = con.CreateCommand();
                }
                else
                {
                    cmd = trans.Connection.CreateCommand();
                }
                cmd.CommandType = cmdType;
                cmd.CommandText = cmdText;
                cmd.CommandTimeout = Timeout;

                // トランザクション設定 
                if (trans != null)
                {
                    cmd.Transaction = trans;
                }

                // パラメータ設定 
                if (cmdPrms != null)
                {
                    foreach (SqlParameter prm in cmdPrms)
                    {
                        if (prm != null)
                        {
                            cmd.Parameters.Add(prm);
                        }
                    }
                }

                return cmd;
            }
            catch (Exception ex)
            {
                cmd = null;
                if (closed)
                {
                    con.Close();
                }
                throw new ApplicationException("データベース接続に失敗しました。", ex);
            }
        }

        #endregion

        #region "パラメータ キャッシュ関連メソッド"

        /// <summary> 
        /// 指定されたキーで SqlParameter 配列をキャッシュします。 
        /// </summary> 
        /// <param name="key">キー</param> 
        /// <param name="cmdPrms">SqlParameter 配列</param> 
        /// <remarks>一般的に key パラメータには実行するコマンド文字列を指定します。</remarks> 
        public static void CacheParameters(string key, params SqlParameter[] cmdPrms)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "key パラメータが null (Nothing in Visual Basic) 参照です。");
            }

            cachedPrms[key] = cmdPrms;
        }

        /// <summary> 
        /// 指定されたキーでキャッシュされた SqlParameter 配列を返します。 
        /// </summary> 
        /// <param name="key">キー</param> 
        /// <returns>SqlParameter 配列</returns> 
        /// <remarks> 
        /// <p>指定されたキーでキャッシュされていない場合は null (Nothing in Visual Basic) を返します。</p> 
        /// <p>一般的に key パラメータには実行するコマンド文字列を指定します。</p> 
        /// </remarks> 
        public static SqlParameter[] GetCacheParameters(string key)
        {
            try
            {
                // キーが存在しない場合 
                if (!cachedPrms.ContainsKey(key))
                {
                    return null;
                }

                // キーから値を取得 
                SqlParameter[] cp = cachedPrms[key];

                if (cp == null)
                {
                    return null;
                }

                // クローンを生成 
                SqlParameter[] clonePrms = new SqlParameter[cp.Length];
                int i = 0;
                int j = cp.Length;
                while (i < j)
                {
                    clonePrms[i] = (SqlParameter)((ICloneable)cp[i]).Clone();
                    i += 1;
                }

                return clonePrms;
            }
            catch
            {
                throw;
            }
        }

        /// <summary> 
        /// キャッシュされた SqlParameter 配列をクリアします。 
        /// </summary> 
        public static void ClearCacheParameters()
        {
            cachedPrms.Clear();
        }

        /// <summary> 
        /// 指定されたキーでキャッシュされた SqlParameter 配列をキャッシュから削除します。 
        /// </summary> 
        /// <param name="key">キー</param> 
        public static void RemoveCacheParameters(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "key パラメータが null (Nothing in Visual Basic) 参照です。");
            }

            cachedPrms.Remove(key);
        }

        #endregion

        #endregion

    }
}