﻿using System;
using System.Collections.Generic;
using System.Reflection;
using WindowsAzure.Common;

namespace WindowsAzure.Table.Queryable.Base
{
    internal static class TypeSystem
    {
        private static readonly Type StringType = typeof (string);
        private static readonly Type ObjectType = typeof (object);
        private static readonly Type EnumerableType = typeof (IEnumerable<>);

        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == StringType)
            {
                return null;
            }

            if (seqType.IsArray)
            {
                return EnumerableType.MakeGenericType(seqType.GetElementType());
            }

            if (seqType.GetTypeInfo().IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = EnumerableType.MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                    {
                        return ienum;
                    }
                }
            }

            var baseType = seqType.GetTypeInfo().BaseType;
            if (baseType != null && baseType != ObjectType)
            {
                return FindIEnumerable(baseType);
            }

            return null;
        }
    }
}