using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using hpsofts.Extension;

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

        #endregion "クラス変数"

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

        #endregion "静的プロパティ"

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

        #endregion "BuildParameterName メソッド"

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

        #endregion "CreateParameter メソッド"

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

        #endregion "パラメータ キャッシュ関連メソッド"

        #endregion "静的メソッド"

#if DEBUG
        internal static void TraceLogCmd(SqlCommand cmd)
        {
            Trace.WriteLine("#[CommandText]: ");
            Trace.WriteLine(cmd.CommandText);
            Trace.WriteLine("#[Parameters]: ");
            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                var sqlParam = cmd.Parameters[i];
                Trace.WriteLine(string.Format("# {0}: {1} ", sqlParam.ParameterName, sqlParam.Value));
            }
        } 
#endif
    }
}