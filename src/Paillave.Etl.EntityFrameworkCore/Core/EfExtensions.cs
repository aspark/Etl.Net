using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Paillave.Etl.EntityFrameworkCore.Core
{
    public static class EfExtensions
    {
        public static EntityEntry<TEntity> EntryWithoutDetectChanges<TEntity>(this DbContext context, TEntity entity)
                    where TEntity : class
        {
            var entryWithoutDetectChangesMethodInfo = context.GetType().GetMethod("EntryWithoutDetectChanges", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(TEntity) }, null);
            return (EntityEntry<TEntity>)entryWithoutDetectChangesMethodInfo.Invoke(context, new object[] { entity });
        }
        public static void DeleteWhere<T>(this DbContext db, Expression<Func<T, bool>> filter) where T : class
        {
            string selectSql = db.Set<T>().Where(filter).ToString();
            string fromWhere = selectSql.Substring(selectSql.IndexOf("FROM"));
            string deleteSql = "DELETE [Extent1] " + fromWhere;
            db.Database.ExecuteSqlCommand(deleteSql);
        }



        public static Expression<Func<T1, TResult>> ApplyPartialRight<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, Expression expressionValue)
        {
            var parameterToBeReplaced = expression.Parameters[1];
            var visitor = new ReplacementVisitor(parameterToBeReplaced, expressionValue);
            var newBody = visitor.Visit(expression.Body);
            return Expression.Lambda<Func<T1, TResult>>(newBody, expression.Parameters[0]);
        }

        public static Expression<Func<T1, TResult>> ApplyPartialRight<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T2 value)
        {
            var parameterToBeReplaced = expression.Parameters[1];
            var constant = Expression.Constant(value, parameterToBeReplaced.Type);
            return ApplyPartialRight(expression, constant);
        }
        public static Expression<Func<T2, TResult>> ApplyPartialLeft<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, Expression expressionValue)
        {
            var parameterToBeReplaced = expression.Parameters[0];
            var visitor = new ReplacementVisitor(parameterToBeReplaced, expressionValue);
            var newBody = visitor.Visit(expression.Body);
            return Expression.Lambda<Func<T2, TResult>>(newBody, expression.Parameters[1]);
        }
        public static Expression<Func<T2, TResult>> ApplyPartialLeft<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T1 value)
        {
            var parameterToBeReplaced = expression.Parameters[0];
            var constant = Expression.Constant(value, parameterToBeReplaced.Type);
            return ApplyPartialLeft(expression, constant);
        }
        public static Expression<Func<TResult>> ApplyPartial<T1, TResult>(this Expression<Func<T1, TResult>> expression, Expression expressionValue)
        {
            var parameterToBeReplaced = expression.Parameters[0];
            var visitor = new ReplacementVisitor(parameterToBeReplaced, expressionValue);
            var newBody = visitor.Visit(expression.Body);
            return Expression.Lambda<Func<TResult>>(newBody, expression.Parameters[1]);
        }
        public static Expression<Func<TResult>> ApplyPartial<T1, TResult>(this Expression<Func<T1, TResult>> expression, T1 value)
        {
            var parameterToBeReplaced = expression.Parameters[0];
            var constant = Expression.Constant(value, parameterToBeReplaced.Type);
            return ApplyPartial(expression, constant);
        }
    }
}