﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using AgileConfig.Server.Data.Entity;
using System.Threading.Tasks;
using AgileConfig.Server.IService;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Linq;

namespace AgileConfig.Server.ServiceTests.sqlite
{
    [TestClass()]
    public class ServerNodeServiceTests: BasicTestService
    {
        IServerNodeService _serverNodeService = null;
        public override Dictionary<string, string> GetConfigurationData()
        {
            return
                new Dictionary<string, string>
                {
                {"db:provider","sqlite" },
                {"db:conn","Data Source=agile_config.db" }
            };
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _serverNodeService = _serviceProvider.GetService<IServerNodeService>();
            _fsq.Delete<ServerNode>().Where("1=1");
        }

        [TestMethod()]
        public async Task AddAsyncTest()
        {
            _fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;

            var result = await _serverNodeService.AddAsync(source);
            Assert.IsTrue(result);

            var node = _fsq.Select<ServerNode>(new
            {
                Address = "1"
            }).ToOne();
            Assert.IsNotNull(node);

            Assert.AreEqual(source.Id, node.Id);
            Assert.AreEqual(source.CreateTime.ToString("yyyyMMddHHmmss"), node.CreateTime.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual(source.LastEchoTime.Value.ToString("yyyyMMddHHmmss"), node.LastEchoTime.Value.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual(source.Remark, node.Remark);
            Assert.AreEqual(source.Status, node.Status);
        }

        [TestMethod()]
        public async Task DeleteAsyncTest()
        {
            _fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;

            var result = await _serverNodeService.AddAsync(source);
            Assert.IsTrue(result);

            var result1 = await _serverNodeService.DeleteAsync(source);
            Assert.IsTrue(result1);

            var node = _fsq.Select<ServerNode>(new
            {
                Address = "1"
            }).ToOne();
            Assert.IsNull(node);
        }

        [TestMethod()]
        public async Task DeleteAsyncTest1()
        {
            _fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;

            var result = await _serverNodeService.AddAsync(source);
            Assert.IsTrue(result);

            var result1 = await _serverNodeService.DeleteAsync(source.Id);
            Assert.IsTrue(result1);

            var node = _fsq.Select<ServerNode>(new
            {
                Address = "1"
            }).ToOne();
            Assert.IsNull(node);
        }

        [TestMethod()]
        public async Task GetAllNodesAsyncTest()
        {
            _fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;

            var result = await _serverNodeService.AddAsync(source);
            Assert.IsTrue(result);

            var nodes = await _serverNodeService.GetAllNodesAsync();
            Assert.IsNotNull(nodes);

            Assert.AreEqual(1, nodes.Count);
        }

        [TestMethod()]
        public async Task GetAsyncTest()
        {
            _fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;
            var result = await _serverNodeService.AddAsync(source);
            Assert.IsTrue(result);

            var node = await _serverNodeService.GetAsync(source.Id);
            Assert.IsNotNull(node);

            Assert.AreEqual(source.Id, node.Id);
            Assert.AreEqual(source.CreateTime.ToString("yyyyMMddHHmmss"), node.CreateTime.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual(source.LastEchoTime.Value.ToString("yyyyMMddHHmmss"), node.LastEchoTime.Value.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual(source.Remark, node.Remark);
            Assert.AreEqual(source.Status, node.Status);
        }

        [TestMethod()]
        public async Task UpdateAsyncTest()
        {
            _fsq.Delete<ServerNode>().Where("1=1").ExecuteAffrows();

            var source = new ServerNode();
            source.Id = "1";
            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "2";
            source.Status = NodeStatus.Offline;
            var result = await _serverNodeService.AddAsync(source);
            Assert.IsTrue(result);

            source.CreateTime = DateTime.Now;
            source.LastEchoTime = DateTime.Now;
            source.Remark = "3";
            source.Status = NodeStatus.Online;
            var result1 = await _serverNodeService.UpdateAsync(source);
            Assert.IsTrue(result);

            var node = await _serverNodeService.GetAsync(source.Id);
            Assert.IsNotNull(node);

            Assert.AreEqual(source.Id, node.Id);
            Assert.AreEqual(source.CreateTime.ToString("yyyyMMddHHmmss"), node.CreateTime.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual(source.LastEchoTime.Value.ToString("yyyyMMddHHmmss"), node.LastEchoTime.Value.ToString("yyyyMMddHHmmss"));
            Assert.AreEqual(source.Remark, node.Remark);
            Assert.AreEqual(source.Status, node.Status);
        }
    }
}