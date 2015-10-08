﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace Microsoft.AspNet.Mvc
{
    public abstract partial class BaseController : Controller
    {
        [NonAction]
        public IActionResult AjaxPagedView<TView, TModel>(
            IEnumerable<TModel> Source,
            string ContentSelector,
            int PageSize = 50,
            AjaxPerformanceType AjaxPerformance = AjaxPerformanceType.WaterFallFlow,
            string PagerDomId = "pager-outer",
            string FormSelector = "form",
            string ViewPath = null)
            where TModel : IConvertible<TView>
        {
            if (Request.Query["raw"] == "info")
            {
                var info = Pager.GetPagerInfo(ref Source, PageSize, string.IsNullOrEmpty(Request.Query["p"]) ? 1 : Convert.ToInt32(Request.Query["p"]));
                return Json(info);
            }
            else if (Request.Query["raw"] == "true")
            {
                Pager.PlainDivide(ref Source, PageSize, string.IsNullOrEmpty(Request.Query["p"]) ? 1 : Convert.ToInt32(Request.Query["p"]));
                if (string.IsNullOrEmpty(ViewPath))
                    return View("~/Views/" + RouteData.Values["controller"].ToString() + "/_" + ActionContext.ActionDescriptor.Name, Source);
                else
                {
                    var last = ViewPath.LastIndexOf('/');
                    var tmp = ViewPath.Substring(0, last);
                    var tmp2 = ViewPath.Substring(last + 1, ViewPath.Length - 1 - last);
                    var ret = new List<TView>();
                    foreach (var item in Source)
                    {
                        var tmp3 = (item as IConvertible<TView>).ToType();
                        ret.Add(tmp3);
                    }
                    return View(tmp + "/_" + tmp2, ret);
                }
            }
            else
            {
                ViewData["__Performance"] = (int)AjaxPerformance;
                ViewData["__PagerDomId"] = PagerDomId;
                ViewData["__ContentSelector"] = ContentSelector;
                ViewData["__FormSelector"] = FormSelector;
                if (string.IsNullOrEmpty(ViewPath))
                    return View();
                else
                    return View(ViewPath);
            }
        }

        [NonAction]
        [Obsolete]
        public IActionResult AjaxTemplatedPagedView<TView, TModel>(
            IEnumerable<TModel> Source,
            string ContentSelector,
            int PageSize = 50,
            AjaxPerformanceType AjaxPerformance = AjaxPerformanceType.WaterFallFlow,
            string PagerDomId = "pager-outer",
            string FormSelector = "form",
            string ViewPath = null)
            where TModel : IConvertible<TView>
        {
            if (!string.IsNullOrEmpty(ViewPath) && CurrentTemplate != null)
                ViewPath = ViewPath.Replace("{template}", Cookies["_template"] ?? Template.DefaultTemplate);
            if (Request.Query["raw"] == "info")
            {
                var info = Pager.GetPagerInfo(ref Source, PageSize, string.IsNullOrEmpty(Request.Query["p"]) ? 1 : Convert.ToInt32(Request.Query["p"]));
                return Json(info);
            }
            else if (Request.Query["raw"] == "true")
            {
                Pager.PlainDivide(ref Source, PageSize, string.IsNullOrEmpty(Request.Query["p"]) ? 1 : Convert.ToInt32(Request.Query["p"]));
                if (string.IsNullOrEmpty(ViewPath))
                    return View("~/Views/" + (Cookies["_template"] ?? Template.DefaultTemplate) + "/" + RouteData.Values["controller"].ToString() + "/_" + ActionContext.ActionDescriptor.Name, Source);
                else
                {
                    var last = ViewPath.LastIndexOf('/');
                    var tmp = ViewPath.Substring(0, last);
                    var tmp2 = ViewPath.Substring(last + 1, ViewPath.Length - 1 - last);
                    var ret = new List<TView>();
                    foreach (var item in Source)
                    {
                        var tmp3 = (item as IConvertible<TView>).ToType();
                        ret.Add(tmp3);
                    }
                    return View(tmp + "/_" + tmp2, ret);
                }
            }
            else
            {
                ViewData["__Performance"] = (int)AjaxPerformance;
                ViewData["__PagerDomId"] = PagerDomId;
                ViewData["__ContentSelector"] = ContentSelector;
                ViewData["__FormSelector"] = FormSelector;
                if (string.IsNullOrEmpty(ViewPath))
                    return View(("~/Views/" + CurrentTemplate.Folder + "/" + ActionContext.RouteData.Values["controller"] + "/" + ActionContext.ActionDescriptor.Name));
                else
                    return View(ViewPath);
            }
        }
    }
}
