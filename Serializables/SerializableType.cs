using System;
using UnityEngine;

namespace Omnix.Serializables
{
    [Serializable]
    public class SerializableType
    {
        [SerializeField] private string guid;
        
        private readonly int _hashCode;
        private Type _type;

        public Type Type
        {
            get
            {
                if (_type == null) _type = Type.GetType(guid);
                return _type;
            }
        }

        public string TypeName => Type.Name;
        public string Namespace => Type.Namespace;
        public string AssemblyName => Type.Assembly.FullName;
        public string DisplayName => $"{Type.Name} ({_type.Namespace})";
        public string Guid => guid;

        public SerializableType()
        {
            _hashCode = System.Guid.NewGuid().GetHashCode();
        }

        public SerializableType(Type systemType)
        {
            _type = systemType;
            guid = systemType.AssemblyQualifiedName;
            _hashCode = guid.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            Type otherType = other.GetType();

            if (otherType == typeof(SerializableType))
            {
                SerializableType st = (SerializableType)other;
                return st.guid == guid;
            }

            if (otherType == typeof(Type))
            {
                Type st = (Type)other;
                return st == _type;
            }

            return false;
        }

        public override int GetHashCode() => _hashCode;
        public static bool operator ==(SerializableType a, SerializableType b) => (a == null && b == null) || (a != null && b != null && a.guid == b.guid);
        public static bool operator !=(SerializableType a, SerializableType b) => !(a == b);
        public static explicit operator Type(SerializableType me) => Type.GetType(me.guid);
        public static explicit operator SerializableType(Type me) => new SerializableType(me);
    }
}