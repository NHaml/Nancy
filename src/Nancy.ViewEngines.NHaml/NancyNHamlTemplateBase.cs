namespace Nancy.ViewEngines.NHaml
{
    using System.Collections.Generic;
    using global::System.Web.NHaml.TemplateBase;

    public class NancyNHamlTemplateBase : Template
    {
        public dynamic Model { get; set; }

        public NancyContext Context
        {
            set
            {
                if (value != null && value.ViewBag != null)
                    base.ViewData = GetDictionary(value.ViewBag);
            }
        }

        private IDictionary<string, object> GetDictionary(DynamicDictionary dynamicDictionary)
        {
            var result = new Dictionary<string, object>();
            foreach (var item in dynamicDictionary)
            {
                result.Add(item, dynamicDictionary[item]);
            }
            return result;
        }
    }
}
