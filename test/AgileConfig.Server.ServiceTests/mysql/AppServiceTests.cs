﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgileConfig.Server.ServiceTests.sqlite;
using System.Collections.Generic;

namespace AgileConfig.Server.Service.Tests.mysql
{
    [TestClass()]
    public class AppServiceTests_mysql: AppServiceTests
    {
        string conn = "Database=agile_config_test;Data Source=192.168.0.125;User Id=root;Password=123456;port=13306";

        public override Dictionary<string, string> GetConfigurationData()
        {
            var dict = base.GetConfigurationData();
            dict["db:provider"] = "mysql";
            dict["db:conn"] = conn;

            return dict;
        }
     
    }
}