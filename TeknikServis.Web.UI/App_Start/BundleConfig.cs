﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace TeknikServis.Web.UI.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundle/js")
                .Include(
                    "~/assets/js/core/jquery.min.js",
                    "~/assets/js/core/bootstrap.min.js",
                    "~/assets/js/core/jquery.slimscroll.min.js",
                    "~/assets/js/core/jquery.scrollLock.min.js",
                    "~/assets/js/core/jquery.placeholder.min.js",
                    "~/assets/js/app.js",
                    "~/assets/js/app-custom.js"
                ));

            bundles.Add(new StyleBundle("~/bundle/css").Include(
                //"~/assets/css/ionicons.css",
                //"~/assets/css/font-awesome.css",
                "~/assets/css/bootstrap.css",
                "~/assets/css/app.css",
                "~/assets/css/app-custom.css"));

            //bundles.Add(new StyleBundle("~/bundle/css")
            //    .Include("~/assets/css/font-awesome.css", new CssRewriteUrlTransform()));
            BundleTable.EnableOptimizations = true;
        }
    }
}