using System.Collections.Generic;
using System.Linq;

namespace EasyLauncher
{
    public interface ITemplateSubstitutor
    {
        string Substitute(string value);
    }

    public sealed class TemplateSubstitutor : ITemplateSubstitutor
    {
        private readonly IDictionary<string, string> templates;

        public TemplateSubstitutor(IDictionary<string, string> templates)
        {
            this.templates = templates;
        }

        public string Substitute(string value)
        {
            return templates.Aggregate(value, (current, template) => current.Replace(template.Key, template.Value));
        }
    }
}