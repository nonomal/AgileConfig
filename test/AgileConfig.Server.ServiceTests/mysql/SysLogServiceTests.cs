﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgileConfig.Server.ServiceTests.sqlite;
using System.Collections.Generic;

namespace AgileConfig.Server.ServiceTests.mysql
{
    [TestClass()]
    public class SysLogServiceTests_mysql : SysLogServiceTests
    {
        string conn = "Database=agile_config_test;Data Source=192.168.0.125;User Id=root;Password=mimi756310;port=13306";

        public override Dictionary<string, string> GetConfigurationData()
        {
            var dict = base.GetConfigurationData();
            dict["db:provider"] = "mysql";
            dict["db:conn"] = conn;

            return dict;
        }
    }
}