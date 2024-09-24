﻿using AgileConfig.Server.Apisite.Controllers.api.Models;
using AgileConfig.Server.Apisite.Filters;
using AgileConfig.Server.Apisite.Models;
using AgileConfig.Server.Common.EventBus;
using AgileConfig.Server.Data.Entity;
using AgileConfig.Server.IService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileConfig.Server.Apisite.Controllers.api
{
    /// <summary>
    /// 应用操作接口
    /// </summary>
    [TypeFilter(typeof(AdmBasicAuthenticationAttribute))]
    [Route("api/[controller]")]
    public class AppController : Controller
    {
        private readonly IConfigService _configService;
        private readonly ITinyEventBus _tinyEventBus;
        private readonly IAppService _appService;
        private readonly IPremissionService _premissionService;
        private readonly IUserService _userService;

        private readonly Controllers.AppController _appController;
        private readonly Controllers.ConfigController _configController;

        public AppController(IAppService appService,
            IPremissionService premissionService,
            IUserService userService,
            IConfigService configService,
            ITinyEventBus tinyEventBus,
            
            Controllers.AppController _appController,
            Controllers.ConfigController _configController)
        {
            _appService = appService;
            _premissionService = premissionService;
            _userService = userService;
            _configService = configService;
            _tinyEventBus = tinyEventBus;

            this._appController = _appController;
            this._configController = _configController;
        }

        /// <summary>
        /// 获取所有应用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiAppVM>>> GetAll()
        {
            var apps = await _appService.GetAllAppsAsync();
            var vms = apps.Select(x =>
            {
                return new ApiAppVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Secret = x.Secret,
                    Inheritanced = x.Type == AppType.Inheritance,
                    Enabled = x.Enabled,
                };
            });

            return Json(vms);
        }

        /// <summary>
        /// 根据id获取应用
        /// </summary>
        /// <param name="id">应用id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiAppVM>> GetById(string id)
        {
            var result = (await _appController.Get(id)) as JsonResult;
            dynamic obj = result.Value;

            if (obj.success)
            {
                AppVM appVM = obj.data;
                return Json(new ApiAppVM
                {
                    Id = appVM.Id,
                    Name = appVM.Name,
                    Secret = appVM.Secret,
                    Inheritanced = appVM.Inheritanced,
                    Enabled = appVM.Enabled,
                    InheritancedApps = appVM.inheritancedApps,
                    AppAdmin = appVM.AppAdmin
                });
            }

            Response.StatusCode = 400;
            return Json(new
            {
                obj.message
            });
        }

        /// <summary>
        /// 添加应用
        /// </summary>
        /// <param name="model">应用模型</param>
        /// <returns></returns>
        [ProducesResponseType(201)]
        [TypeFilter(typeof(PremissionCheckByBasicAttribute), Arguments = new object[] { "App.Add", Functions.App_Add })]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ApiAppVM model)
        {
            var requiredResult = CheckRequired(model);

            if (!requiredResult.Item1)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    message = requiredResult.Item2
                });
            }

            _appController.ControllerContext.HttpContext = HttpContext;

            var result = (await _appController.Add(new AppVM
            {
                Id = model.Id,
                Name = model.Name,
                Secret = model.Secret,
                AppAdmin = model.AppAdmin,
                Inheritanced = model.Inheritanced
            })) as JsonResult;

            dynamic obj = result.Value;

            if (obj.success == true)
            {
                return Created("/api/app/" + obj.data.Id, "");
            }

            Response.StatusCode = 400;
            return Json(new
            {
                obj.message
            });
        }

        /// <summary>
        /// 编辑应用
        /// </summary>
        /// <param name="id">应用id</param>
        /// <param name="model">编辑后的应用模型</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [TypeFilter(typeof(PremissionCheckByBasicAttribute), Arguments = new object[] { "App.Edit", Functions.App_Edit })]
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] ApiAppVM model)
        {
            var requiredResult = CheckRequired(model);

            if (!requiredResult.Item1)
            {
                Response.StatusCode = 400;
                return Json(new
                {
                    message = requiredResult.Item2
                });
            }

            _appController.ControllerContext.HttpContext = HttpContext;

            model.Id = id;
            var result = (await _appController.Edit(new AppVM
            {
                Id = model.Id,
                Name = model.Name,
                Secret = model.Secret,
                AppAdmin = model.AppAdmin,
                Inheritanced = model.Inheritanced
            })) as JsonResult;

            dynamic obj = result.Value;
            if (obj.success == true)
            {
                return Ok();
            }

            Response.StatusCode = 400;
            return Json(new
            {
                obj.message
            });
        }

        /// <summary>
        /// 删除应用
        /// </summary>
        /// <param name="id">应用id</param>
        /// <returns></returns>
        [ProducesResponseType(204)]
        [TypeFilter(typeof(PremissionCheckByBasicAttribute), Arguments = new object[] { "App.Delete", Functions.App_Delete })]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            _appController.ControllerContext.HttpContext = HttpContext;

            var result = (await _appController.Delete(id)) as JsonResult;

            dynamic obj = result.Value;
            if (obj.success == true)
            {
                return NoContent();
            }

            Response.StatusCode = 400;
            return Json(new
            {
                obj.message
            });
        }

        /// <summary>
        /// 发布某个应用的待发布配置项
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="env">环境</param>
        /// <returns></returns>
        [TypeFilter(typeof(AdmBasicAuthenticationAttribute))]
        [TypeFilter(typeof(PremissionCheckByBasicAttribute), Arguments = new object[] { "Config.Publish_API", Functions.Config_Publish })]
        [HttpPost("publish")]
        public async Task<IActionResult> Publish(string appId, EnvString env)
        {
            _configController.ControllerContext.HttpContext = HttpContext;

            var result = (await _configController.Publish(new PublishLogVM()
            {
                AppId = appId
            }, env)) as JsonResult;

            dynamic obj = result.Value;
            if (obj.success == true)
            {
                return Ok();
            }

            Response.StatusCode = 400;
            return Json(new
            {
                obj.message
            });
        }

        /// <summary>
        /// 查询某个应用的发布历史
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="env">环境</param>
        /// <returns></returns>
        [TypeFilter(typeof(AdmBasicAuthenticationAttribute))]
        [HttpGet("Publish_History")]
        public async Task<ActionResult<IEnumerable<ApiPublishTimelineVM>>> PublishHistory(string appId, string env)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(appId);

            ISettingService.IfEnvEmptySetDefault(ref env);

            var history = await _configService.GetPublishTimelineHistoryAsync(appId, env);

            history = history.OrderByDescending(x => x.Version).ToList();

            var vms = history.Select(x => new ApiPublishTimelineVM
            {
                Id = x.Id,
                Version = x.Version,
                Log = x.Log,
                PublishTime = x.PublishTime,
                PublishUserId = x.PublishUserId,
                Env = x.Env
            });

            return Json(vms);
        }

        /// <summary>
        /// 回滚某个应用的发布版本，回滚到 historyId 指定的时刻
        /// </summary>
        /// <param name="historyId">发布历史</param>
        /// <param name="env">环境</param>
        /// <returns></returns>
        [TypeFilter(typeof(AdmBasicAuthenticationAttribute))]
        [TypeFilter(typeof(PremissionCheckByBasicAttribute), Arguments = new object[] { "Config.Rollback_API", Functions.Config_Publish })]
        [HttpPost("rollback")]
        public async Task<IActionResult> Rollback(string historyId, EnvString env)
        {
            _configController.ControllerContext.HttpContext = HttpContext;

            var result = (await _configController.Rollback(historyId, env)) as JsonResult;

            dynamic obj = result.Value;
            if (obj.success == true)
            {
                return Ok();
            }

            Response.StatusCode = 400;
            return Json(new
            {
                obj.message
            });
        }

        private (bool, string) CheckRequired(ApiAppVM model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                return (false, "Id不能为空");
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                return (false, "Name不能为空");
            }

            return (true, "");
        }
    }
}
