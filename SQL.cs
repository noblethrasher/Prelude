using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml.Linq;
using System.Web;

namespace Prelude
{
    public abstract class Sp_Base
    {
        protected List<SqlParameter> param = new List<SqlParameter>();

        readonly string sql;

        public Sp_Base(string sql)
        {
            this.sql = sql;
        }

        protected virtual string GetConnectionString() 
        {
            return ConfigurationManager.ConnectionStrings["default"].ConnectionString;
        }

        public virtual int Execute(SqlConnection conn)
        {
            bool dispose = conn == null;

            conn = conn ?? new SqlConnection(GetConnectionString());

            var command = new SqlCommand(sql, conn) { CommandType = System.Data.CommandType.StoredProcedure };

            if (conn.State != ConnectionState.Open)
                conn.Open();            

            foreach (var p in param)
                command.Parameters.Add(p);

            var result = command.ExecuteNonQuery();

            if (dispose)
                conn.Dispose();

            return result;
        }

        public int Execute()
        {
            return Execute(null);
        }
    }

    public abstract class Sp_View_Base
    {

        protected class Connection : IDisposable
        {

            string sql;

            SqlConnection sc;
            bool dispose;

            List<SqlParameter> @params = new List<SqlParameter>();

            public Connection(string sql, SqlConnection sc)
            {
                this.sql = sql;

                if (sc == null)
                    dispose = true;

                this.sc = sc ?? new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["default"].ConnectionString);

                if (this.sc.State != System.Data.ConnectionState.Open)
                    this.sc.Open();
            }

            public void Dispose()
            {
                if (dispose)
                    sc.Dispose();
            }

            public void Add(SqlParameter param)
            {
                this.@params.Add(param);
            }

            public SqlDataReader ExecuteReader()
            {
                var command = new SqlCommand(sql, sc) { CommandType = System.Data.CommandType.StoredProcedure };

                command.Parameters.AddRange(@params.ToArray());

                return command.ExecuteReader();
                    
            }
        }        
        
        protected static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["default"].ConnectionString;
        }

        protected static XContainer GetXObject(string Name, XDocument Xml= null)
        {
            return (Xml != null) ? Xml.Root : new XElement(Name);
        }

        public abstract XContainer ToXML(XDocument xml = null);
    }    
    
    public sealed class DynamicDataReader : IDataReader, IDynamicMetaObjectProvider
    {
        IDataReader reader;

        public DynamicDataReader(IDataReader reader)
        {
            this.reader = reader;
            f = s => this[s];
        }

        public void Close()
        {
            reader.Close();
        }

        public int Depth
        {
            get { return reader.Depth; }
        }

        public DataTable GetSchemaTable()
        {
            return reader.GetSchemaTable();
        }

        public bool IsClosed
        {
            get { return reader.IsClosed; }
        }

        public bool NextResult()
        {
            return reader.NextResult();
        }

        public bool Read()
        {
            return reader.Read();
        }

        public int RecordsAffected
        {
            get { return reader.RecordsAffected; }
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        public int FieldCount
        {
            get { return reader.FieldCount; }
        }

        public bool GetBoolean(int i)
        {
            return reader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return reader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return reader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return reader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return reader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return reader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return reader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return reader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return reader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return reader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return reader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return reader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return reader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return reader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return reader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return reader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return reader.GetString(i);
        }

        public object GetValue(int i)
        {
            return reader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return reader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return reader.IsDBNull(i);
        }

        public object this[string name]
        {
            get { return reader[name]; }
        }

        public object this[int i]
        {
            get { return reader[i]; }
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new _dynamicReader(this, parameter);
        }

        Expression<Func<string, object>> f;

        class _dynamicReader : DynamicMetaObject
        {
            DynamicDataReader reader;

            public _dynamicReader(DynamicDataReader reader, Expression exp) : base(exp, BindingRestrictions.GetInstanceRestriction(Expression.Constant(reader), reader), reader )
            {
                this.reader = reader;
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                return new DynamicMetaObject(Expression.Invoke(reader.f, Expression.Constant(binder.Name)) , BindingRestrictions.GetInstanceRestriction(Expression.Constant(reader), reader));
            }            
        }        
    }

    public class SQL : IEnumerable<DynamicDataReader>, IDisposable, IEnumerable<SqlParameter>
    {
        string sql;
        string connectionString;

        Action dispose;

        List<SqlParameter> sql_params;

        public SQL(string sql) : this(sql, null) { }
        
        public SQL(string sql, string connectionString)
        {
            this.sql = sql;
            this.connectionString = connectionString;
        }

        public void Add(SqlParameter sql_param)
        {
            (sql_params = sql_params ?? new List<SqlParameter>()).Add(sql_param);
        }

        public void Add(IEnumerable<SqlParameter> @params)
        {
            (sql_params = sql_params ?? new List<SqlParameter>()).AddRange(@params);
        }

        protected virtual SqlConnection GetConnection(string connectionString = null)
        {
            connectionString = connectionString ?? ConfigurationManager.ConnectionStrings["default"].ConnectionString;

            return new SqlConnection(connectionString);
        }

        public SqlCommand GetSqlCommand()
        {
            var conn = GetConnection(connectionString);
            conn.Open();

            dispose += () => conn.Dispose();
                
            var command = new SqlCommand(sql, conn) { CommandType = GetCommandType() };

            if(sql_params != null)
                command.Parameters.AddRange(sql_params.ToArray());

            return command;
        }

        private CommandType GetCommandType()
        {
            if (sql.StartsWith("[") || !sql.Contains(' '))
                return CommandType.StoredProcedure;
            else
                return CommandType.Text;
        }

        public IEnumerator<DynamicDataReader> GetEnumerator()
        {
            try
            {
                var command = GetSqlCommand();
                
                var reader = new DynamicDataReader(command.ExecuteReader());

                while (reader.Read())
                     yield return reader;
                
            }
            finally
            {
                command.Connection.Dispose();
            }
            
            
           
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            if (dispose != null)
                dispose();
        }

        public static implicit operator SQL(string sql)
        {
            return new SQL(sql);
        }

        public static SQL operator +(SQL sql, SqlParams ps)
        {
            sql.Add(ps);

            return sql;
        }

        public static SQL operator +(SQL sql, Connection conn)
        {
            sql.connectionString = conn.connection;

            return sql;
        }

        IEnumerator<SqlParameter> IEnumerable<SqlParameter>.GetEnumerator()
        {
            return sql_params.GetEnumerator();
        }        
    }

    public sealed class SqlParams : IEnumerable<SqlParameter>, IEnumerable<KeyValuePair<string, object>>
    {
        List<SqlParameter> @params = new List<SqlParameter>();

        public void Add(string key, object value) { @params.Add(new SqlParameter(key, value)); }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            foreach (var p in @params)
                yield return new KeyValuePair<string, object>(p.ParameterName, p.Value);
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<SqlParameter> GetEnumerator()
        {
            return @params.GetEnumerator();
        }        
    }

    public sealed class Connection
    {
        public readonly string connection;

        public Connection(string connection)
        {
            this.connection = connection;
        }

        public static implicit operator Connection(string s)
        {
            return new Connection(s);
        }
    }
}
