﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public partial class NewUnitTest
    {
       public static  SqlSugarClient Db=> new SqlSugarClient(new ConnectionConfig()
        {
            DbType = DbType.SqlServer,
            ConnectionString = Config.ConnectionString,
            InitKeyType = InitKeyType.Attribute,
            IsAutoCloseConnection = true,
            AopEvents = new AopEvents
            {
                OnLogExecuting = (sql, p) =>
                {
                    Console.WriteLine(sql);
                    Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                }
            }
        });

        public static void RestData()
        {
            Db.DbMaintenance.TruncateTable<Order>();
            Db.DbMaintenance.TruncateTable<OrderItem>();
        }
        public static void Init()
        {
            UCustom018.Init();
            UCustom017.Init();
            UCustom016.Init();
            UCustom015.Init();
            UCustom014.Init();
            UCustom013.Init();
            UCustom012.Init();
            UCustom09.Init();
            UCustom011.Init();
            UCustom010.Init();
            UCustom08.Init();
            UCustom07.Init();
            UCustom01.Init();
            UCustom02.Init();
            UCustom03.Init();
            UCustom04.Init();
            UCustom05.Init();
            UCustom06.Init();
            SubQueryTest();
            UConfig();
            DeleteTest();
            Fastest2();
            SplitTest();
            Filter();
            Insert();
            Insert2();
            Enum();
            Tran();
            Queue();
            CodeFirst();
            Updateable();
            Json();
            Ado();
            Queryable();
            Queryable2();
            QueryableAsync();
            AopTest();
            //Thread();
            //Thread2();
            //Thread3();
        }
    }
}
