using Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Utilities.Extentions;

namespace LearningSkill.TagHelpers
{
    [HtmlTargetElement("constant")]
    public class ConstantTagHelper : TagHelper
    {
        // نوع اینام ورودی
        public ModelExpression ConstantType { get; set; }

        // پروپرتی ورودی
        public ModelExpression ConstantFor { get; set; }

        // لیست ورودی برای دراپدون
        [HtmlAttributeName("constant-items")]
        public IEnumerable<SelectListItem> ConstantItems { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ConstantType == null || ConstantFor == null) return;

            // گرفتن نوع داده ورودی
            ConstantType type = (ConstantType)ConstantType.Model;
            var customDataType = type.GetEnumAttribute<CustomDataTypeAttribute>();

            #region برای نوع متنی، عددی یا بولین

            output.TagName = "input";
            output.TagMode = TagMode.SelfClosing;

            // مشخص کردن نوع اینپوت خروجی
            var inputType = "";

            #region برای نوع عددی و متنی

            if (customDataType.Type == CustomDataType.Number || customDataType.Type == CustomDataType.String)
            {
                inputType = customDataType.Type.ToString().ToLower();
                if (inputType == "string") inputType = "text";

                output.Attributes.Add("class", "form-control");
                output.Attributes.Add("value", ConstantFor.Model as string);
                output.Attributes.Add("placeholder", customDataType.Placeholder);
                if(customDataType.Min != 0)
                    output.Attributes.Add("min", customDataType.Min);
                if (customDataType.Max != 0)
                    output.Attributes.Add("max", customDataType.Max);

            }
            #endregion


            #region برای بولین

            if (customDataType.Type == CustomDataType.Bool)
            {
                inputType = "checkbox";

                var model = ConstantFor.Model as string;

                if (model == "true")
                    output.Attributes.Add("checked", model);

                output.Attributes.Add("data-val", model);

                output.PreElement.AppendHtml("<div class=\"togglebutton col-md-12\">");
                output.PreElement.AppendHtml("<label>");
                output.PostContent.SetHtmlContent("<span class=\"toggle\"></span>");
                output.PostElement.SetHtmlContent("<span style=\"padding-right: 10px\">گزینه فعال باشد</span>");
            }
            #endregion

            output.Attributes.Add("type", inputType);

            #endregion

            #region برای دراپدون
            if (customDataType.Type == CustomDataType.DropDown)
            {
                if (ConstantItems == null) return;
                output.TagName = "select";
                output.TagMode = TagMode.StartTagAndEndTag;

                output.Attributes.Add("class", "selectpicker valid");
                output.Attributes.Add("data-live-search", "true");
                output.Attributes.Add("data-size", "7");
                output.Attributes.Add("tabindex", "-98");
                output.Attributes.Add("data-style", "select-with-transition");
                output.Attributes.Add("aria-invalid", "false");
                output.Attributes.Add("tabindex", "-98");

                output.Attributes.Add("test", ConstantFor.Model as string);

                //var list = customDataType.Items;
                //foreach (var item in list)
                //{
                //    if (ConstantFor.Model as string == item.Value)
                //        output.Content.AppendHtml("<option selected=\"selected\" value=\"" + item.Value + "\">" + item.Text + "</option>");
                //    else
                //        output.Content.AppendHtml("<option value=\"" + item.Value + "\">" + item.Text + "</option>");
                //}
            }
            #endregion

            var _for = ConstantFor.Name;
            output.Attributes.Add("id", _for);
            output.Attributes.Add("name", _for);

            if (customDataType.Max > 0)
            {
                output.Attributes.Add("maxlength", customDataType.Max);
                output.Attributes.Add("data-val-length-max", customDataType.Max);
                output.Attributes.Add("data-val-length", "مقدار حداکثر " + customDataType.Max + " کاراکتر باشد.");
            }
        }
    }
}
