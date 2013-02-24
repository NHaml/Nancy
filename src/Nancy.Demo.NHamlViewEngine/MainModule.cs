using System;
using Nancy.Demo.NHamlViewEngine.Models;
using System.Collections.Generic;

namespace Nancy.Demo.NHamlViewEngine
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = parameters =>
                           {
                               this.ViewBag["NancyVersion"] = this.GetType().Assembly.GetName().Version.ToString();
                               this.ViewBag["NHamlVersion"] = Nancy.ViewEngines.NHaml.NHamlViewEngine.NHamlVersion;

                               var productList = new List<Product>
                               {
                                   new Product { Name = "Acme Snargler", Sku = "ACM0001"},
                                   new Product { Name = "Spumko Fooferon", Sku = "SPM0002"}
                               };

                               return View["Index", productList];
                           };            
        }
    }
}