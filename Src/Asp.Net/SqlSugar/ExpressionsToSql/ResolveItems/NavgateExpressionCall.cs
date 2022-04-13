﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class OneToManyNavgateExpression
    {
        private SqlSugarProvider context;
        private EntityInfo EntityInfo;
        private EntityInfo ProPertyEntity;
        private Navigat Navigat;
        public string ShorName;
        private string MemberName;
        private string MethodName;
        public OneToManyNavgateExpression(SqlSugarProvider context)
        {
            this.context = context;
        }

        internal bool IsNavgate(Expression expression)
        {
            var result = false;
            var exp = expression;
            if (exp is UnaryExpression) 
            {
                exp = (exp as UnaryExpression).Operand;
            }
            if (exp is MethodCallExpression) 
            {
                var memberExp=exp as MethodCallExpression;
                MethodName = memberExp.Method.Name;
                if (memberExp.Method.Name.IsIn("Any","Count") &&  memberExp.Arguments.Count>0 && memberExp.Arguments[0] is MemberExpression ) 
                {
                    result = ValidateNav(result, memberExp.Arguments[0] as MemberExpression, memberExp.Arguments[0]);
                }
            }
            return result;
        }

        private bool ValidateNav(bool result, MemberExpression memberExp, Expression childExpression)
        {
            if (childExpression != null && childExpression is MemberExpression)
            {
                var child2Expression = (childExpression as MemberExpression).Expression;
                if (child2Expression.Type.IsClass() && child2Expression is ParameterExpression)
                {
                    var rootType = child2Expression.Type;
                    var rootEntity = this.context.EntityMaintenance.GetEntityInfo(rootType);
                    var type= childExpression.Type.GetGenericArguments()[0];
                    var entity = this.context.EntityMaintenance.GetEntityInfo(type);
                    if (rootEntity.Columns.Any(x => x.PropertyName == (childExpression as MemberExpression).Member.Name && x.Navigat != null))
                    {
                        EntityInfo = rootEntity;
                        ShorName = child2Expression.ToString();
                        MemberName = memberExp.Member.Name;
                        ProPertyEntity = entity;
                        Navigat = rootEntity.Columns.FirstOrDefault(x => x.PropertyName == (childExpression as MemberExpression).Member.Name).Navigat;
                        result = true;
                    }
                }
            }

            return result;
        }
        internal MapperSql GetSql()
        {
            if (Navigat.NavigatType == NavigatType.OneToMany)
            {
                return GetOneToManySql();
            }
            else 
            {
                return GetManyToManySql();
            }
     
        }
        private MapperSql GetManyToManySql()
        {
            var pk = this.ProPertyEntity.Columns.First(it => it.IsPrimarykey == true).DbColumnName;
            var name = this.EntityInfo.Columns.First(it => it.PropertyName == Navigat.Name).DbColumnName;
            var selectName = this.ProPertyEntity.Columns.First(it => it.PropertyName == MemberName).DbColumnName;
            MapperSql mapper = new MapperSql();
            var queryable = this.context.Queryable<object>();
            pk = queryable.QueryBuilder.Builder.GetTranslationColumnName(pk);
            name = queryable.QueryBuilder.Builder.GetTranslationColumnName(name);
            selectName = queryable.QueryBuilder.Builder.GetTranslationColumnName(selectName);
            mapper.Sql = queryable
                .AS(this.ProPertyEntity.DbTableName)
                .Where($" {ShorName}.{name}={pk} ").Select(selectName).ToSql().Key;
            mapper.Sql = $" ({mapper.Sql}) ";
            mapper.Sql = GetMethodSql(mapper.Sql);
            return mapper;
        }
        private MapperSql GetOneToManySql()
        {
            var pk = this.EntityInfo.Columns.First(it => it.IsPrimarykey == true).DbColumnName;
            var name = this.ProPertyEntity.Columns.First(it => it.PropertyName == Navigat.Name).DbColumnName;
            //var selectName = this.ProPertyEntity.Columns.First(it => it.PropertyName == MemberName).DbColumnName;
            MapperSql mapper = new MapperSql();
            var queryable = this.context.Queryable<object>();
            pk = queryable.QueryBuilder.Builder.GetTranslationColumnName(pk);
            name = queryable.QueryBuilder.Builder.GetTranslationColumnName(name);
            //selectName = queryable.QueryBuilder.Builder.GetTranslationColumnName(selectName);
            mapper.Sql = queryable
                .AS(this.ProPertyEntity.DbTableName)
                .Where($" {name}={ShorName}.{pk} ").Select(" COUNT(1) ").ToSql().Key;
            mapper.Sql = $" ({mapper.Sql}) ";
            mapper.Sql = GetMethodSql(mapper.Sql);
            return mapper;
        }

        private string GetMethodSql(string sql)
        {
            if (MethodName == "Any") 
            {
                return $" ({sql}>0 ) ";
            }
            return sql;
        }

    }
}
