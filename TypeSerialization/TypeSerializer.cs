﻿using System;
using System.Collections.Generic;
using System.Linq;
using TypeSerialization._internal;

namespace TypeSerialization
{
    public static class TypeSerializer
    {
        /// <summary>Converts an object type to a string representation.</summary>
        /// <param name="type">An object type.</param>
        /// <returns>A string like: "String", "Array(Int32)", "Dictionary(Int32-String)", ...</returns>
        public static string Serialize(this Type type, Formats format = Formats.UriSafe)
        {
            if (type.IsArray)
                return string.Format(
                    SerializationFormat.Values[(int)format].Format,
                    nameof(Array),
                    Serialize(type.GetElementType()!, format));

            if (!type.IsPublic && Types.Type.IsAssignableFrom(type))
                return nameof(Type);

            if (!type.IsGenericType)
                return Types.Defaults.Value.Contains(type)? type.Name: AsString(type);

            var nameLength = type.Name.IndexOf('`');
            var name = nameLength < 0 ? type.Name : type.Name.Substring(0, nameLength);

            return string.Format(
                SerializationFormat.Values[(int)format].Format,
                name, Serialize(type.GetGenericArguments(), format));
        }

        /// <summary>Converts an array of object types to a string representation.</summary>
        /// <param name="types">Object types.</param>
        /// <returns>A string like: "Boolean-List(String)-Array(Nullable(Int32))".</returns>
        public static string Serialize(this IEnumerable<Type?> types, Formats format = Formats.UriSafe)
        {
            return string.Join(
#if NET6_0_OR_GREATER
                SerializationFormat.Values[(int)format].Sep,
#else
                SerializationFormat.Values[(int)format].SepStr,
#endif
                types.Select(x => x == null || x.IsGenericParameter ? string.Empty : Serialize(x, format)));
        }

        public static string AsString(string assemblyName, string nameSpace, string name)
        {
            return nameSpace + "." + name + ", " + assemblyName;
        }

        public static string AsString(this Type type)
        {
            return AsString(type.Assembly.ToString(), type.Namespace, type.Name);
        }


    }
}
