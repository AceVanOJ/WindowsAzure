﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.Queryable.Base
{
    public abstract class QueryProviderBase : IQueryProvider
    {
        IQueryable<TS> IQueryProvider.CreateQuery<TS>(Expression expression)
        {
            return new Query<TS>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            Type queryType = typeof (Query<>).MakeGenericType(elementType);

            try
            {
                return (IQueryable) Activator.CreateInstance(queryType, new object[] {this, expression});
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        TS IQueryProvider.Execute<TS>(Expression expression)
        {
            return (TS) Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }

        public abstract object Execute(Expression expression);
    }
}