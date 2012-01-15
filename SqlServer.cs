using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Prelude
{
    public abstract class ProcBase<T>
    {
        protected readonly SqlCommand command;
        
        protected abstract T _Execute();

        protected ProcBase(string sql, System.Data.CommandType commandType = System.Data.CommandType.StoredProcedure)
        {
            command = new SqlCommand (sql) { CommandType = commandType };
        }

        protected SqlDataReader ExecuteReader()
        {
            return command.ExecuteReader ();
        }

        protected int ExecuteNonQuery()
        {
            return command.ExecuteNonQuery ();
        }

        protected void AddParam(SqlParameter param)
        {
            command.Parameters.Add (param);
        }

        protected void AddParams(SqlParameter param1, SqlParameter param2)
        {
            command.Parameters.Add (param1);
            command.Parameters.Add (param2);
        }

        protected void AddParams(SqlParameter param1, SqlParameter param2, SqlParameter param3)
        {
            command.Parameters.Add (param1);
            command.Parameters.Add (param2);
            command.Parameters.Add (param3);
        }

        protected void AddWithValue(string name, object value)
        {
            command.Parameters.AddWithValue (name, value);
        }
       
        protected virtual string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["default"].ConnectionString;
        }

        protected void Read(Action<SqlDataReader> act)
        {
            var rdr = ExecuteReader ();

            while (rdr.Read ())
                act (rdr);
        }

        public virtual T Execute(SqlConnection sc = null)
        {
            var should_dispose = sc == null;
            
            sc = sc ?? new SqlConnection (GetConnectionString());

            try
            {
                if (sc.State != System.Data.ConnectionState.Open)
                    sc.Open ();

                command.Connection = sc;

                return _Execute ();
            }
            finally
            {
                if (should_dispose)
                    sc.Dispose ();
            }            
        }

        protected static SqlParameter Int32IOParam(string name)
        {
            return new SqlParameter () { ParameterName = name, Direction = System.Data.ParameterDirection.InputOutput, DbType = System.Data.DbType.Int32 };
        }

        protected static SqlParameter DateTimeIOParam(string name)
        {
            return new SqlParameter () { ParameterName = name, Direction = System.Data.ParameterDirection.InputOutput, DbType = System.Data.DbType.DateTime };
        }

        protected static SqlParameter GuidIOParam(string name)
        {
            return new SqlParameter () { ParameterName = name, Direction = System.Data.ParameterDirection.InputOutput, DbType = System.Data.DbType.Guid };
        }
    }

    public abstract class Hydration<T>
    {
        readonly T obj;

        bool init = false;

        public Hydration(T obj)
        {
            this.obj = obj;
        }

        protected abstract void _Hydrate(SqlDataReader rdr);

        public void Hydrate(SqlDataReader rdr)
        {
            _Hydrate (rdr);

            init = true;
        }

        public static implicit operator T(Hydration<T> obj)
        {
            return obj.init ? obj : default (T);
        }
    }

    public interface DataInstantiable<T>
    {
        T Hydrate(SqlDataReader rdr);
    }

    public static class SqlHelper
    {
        public static T Hydrate<T>(this SqlDataReader rdr, Hydration<T> obj)
        {
            obj.Hydrate (rdr);

            return obj;
        }

        public static T Hydrate<T>(this SqlDataReader rdr, DataInstantiable<T> obj)
        {
            return obj.Hydrate (rdr);
        }


    }
}
