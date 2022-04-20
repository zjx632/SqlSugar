﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public static class CommonExtensions
    {
        public static IEnumerable<T> WhereIF<T>(this IEnumerable<T> thisValue, bool isOk, Func<T, bool> predicate) 
        {
            if (isOk)
            {
                return thisValue.Where(predicate);
            }
            else 
            {
                return thisValue;
            }
        }
    }
}
