using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Template.Models.ViewModels;

namespace Template.MVC.TagHelpers
{
    [HtmlTargetElement("multiselect")]
    [HtmlTargetElement(Attributes = "asp-for,asp-items,asp-groupby")]
    public class MultiSelectTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private string ElementId { get { return SelectedValues.Name.Replace(".", "_"); } } // tag ids are for some reason generated with underscores instead of dots

        private string ElementName { get { return SelectedValues.Name; } }

        public MultiSelectTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets or sets the items that are bound to this multiselect list
        /// </summary>
        [HtmlAttributeName("asp-items")]
        public List<SelectListItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the selected values for the list
        /// </summary>
        [HtmlAttributeName("asp-for")]
        public ModelExpression SelectedValues { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("id", ElementId);
            output.Attributes.SetAttribute("class", "selectpicker");
            output.Attributes.SetAttribute(new TagHelperAttribute("multiple"));

            var sb = new StringBuilder();

            // configure select element
            bool mustGroup = Items.Any(i => !string.IsNullOrEmpty(i.Group));
            if (mustGroup)
            {
                foreach (var groupedItems in Items.GroupBy(i => i.Group))
                {
                    sb.AppendLine($"<optgroup label='{groupedItems.Key}'>");
                    foreach (var item in groupedItems)
                    {
                        var selectedAttribute = ((List<int>)SelectedValues.Model).Any(c => c == item.Value) ? "selected" : string.Empty;
                        sb.AppendLine($"<option value='{item.Value}' {selectedAttribute }>{item.Name}</option>");
                    }
                    sb.AppendLine($"</optgroup>");
                }
            }
            else
            {
                foreach (var item in Items)
                {
                    var selectedAttribute = ((List<int>)SelectedValues.Model).Any(c => c == item.Value) ? "selected" : string.Empty;
                    sb.AppendLine($"<option value='{item.Value}' {selectedAttribute }>{item.Name}</option>");
                }
            }
            output.PreContent.SetHtmlContent(sb.ToString());

            // configure selected inputs container
            sb = new StringBuilder();
            sb.AppendLine($"<div id='{ElementId}-container'>");
            foreach (var value in SelectedValues.Model as List<int>)
            {
                sb.AppendLine($"<input type='hidden' name='{ElementName}' value='{value}' />");
            }
            sb.AppendLine($"</div>");
            output.PostElement.AppendHtml(sb.ToString());

        }
    }
}
