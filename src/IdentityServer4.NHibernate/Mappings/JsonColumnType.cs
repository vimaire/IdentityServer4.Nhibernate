using System;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace IdentityServer4.NHibernate.Mappings
{
    public class JsonColumnType<T> : IUserType where T : class
    {
        public Type ReturnedType => typeof(T);

        public object Assemble(object cached, object owner) => cached;

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            var column = value as T;
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(cmd, "{}", index, session);
                return;
            }
            value = Serialise(column);
            NHibernateUtil.String.NullSafeSet(cmd, value, index, session);
        }

        public object DeepCopy(object value)
        {
            if (!(value is T source))
                return null;
            return Deserialise(Serialise(source));
        }

        public object Disassemble(object value) => value;

        public new bool Equals(object x, object y)
        {
            var left = x as T;
            var right = y as T;

            if (left == null && right == null)
                return true;

            if (left == null || right == null)
                return false;

            return Serialise(left).Equals(Serialise(right));
        }

        public int GetHashCode(object x) => x.GetHashCode();

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var returnValue = NHibernateUtil.String.NullSafeGet(rs, names[0], session, owner);
            var json = returnValue?.ToString() ?? "{}";
            return Deserialise(json);
        }

        public bool IsMutable => false;

        public object Replace(object original, object target, object owner) => original;

        public SqlType[] SqlTypes => new SqlType[] { SqlTypeFactory.GetString(8000) };

        private T Deserialise(string jsonString)
        {
            return string.IsNullOrWhiteSpace(jsonString) 
                ? CreateObject(typeof(T)) 
                : JsonConvert.DeserializeObject<T>(jsonString);
        }

        private string Serialise(T obj)
        {
            return obj == null 
                ? "{}" 
                : JsonConvert.SerializeObject(obj);
        }

        private static T CreateObject(Type jsonType)
        {
            object result;
            try
            {
                result = Activator.CreateInstance(jsonType, true);
            }
            catch (Exception)
            {
                result = FormatterServices.GetUninitializedObject(jsonType);
            }

            return (T)result;
        }
    }
}