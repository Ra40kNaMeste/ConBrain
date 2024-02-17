using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Encodings.Web;

namespace ConBrain.TagHelpers
{
    internal enum InputTypes
    {
        Radio,
        Text,
        Checkbox,
        Hidden,
        Submit,
        Color,
        Time,
        Week,
        Date,
        DateTime,
        Email,
        Mounth,
        Number,
        Range,
        Tel
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayAttribute : Attribute
    {
        public DisplayAttribute() { }
        public DisplayAttribute(string name)
        {
            Name = name;
        }
        public string? Name { get; }
        public string? Type { get; set; }
        public string[]? Classes { get; set; }
    }


    public class TableFormTagHelper : FormTagHelper
    {
        public TableFormTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public object Target { get; set; }
        public string TableName { get; set; }

        public string RowClassName { get; set; }
        public string PropertyClassName { get; set; }
        public string ValueClassName { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            output.TagName = "form";
            var type = Target.GetType();
            var properties = type.GetProperties();
            TagBuilder table = new("table");
            List<TagBuilder> rows = new();
            var name = GenerateTableName(output);
            if (name != null)
                rows.Add(name);

            foreach (var property in properties)
            {
                var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                {
                    var attrs = property.GetCustomAttributes()
                        .Select(i => ConvertAttribute(i, property))
                        .SelectMany(i => i)
                        .ToDictionary(i => i.Item1, i => i.Item2);
                    attrs.Add("value", property.GetValue(Target).ToString());
                    rows.Add(GenerateRow(property.Name, attrs));
                }
            }
            table.SetChild(rows.ToArray());
            using var sw = new StringWriter();
            table.WriteTo(sw, HtmlEncoder.Default);

            output.Content.AppendHtml(sw.ToString());
        }
        private TagBuilder? GenerateTableName(TagHelperOutput output)
        {
            if (TableName != null)
            {
                TagBuilder nameBuilder = new("caption");
                nameBuilder.InnerHtml.Append(TableName);
                return nameBuilder;
            }
            return null;
        }
        private TagBuilder GenerateRow(string nameProperty, Dictionary<string, string?> attrs)
        {
            TagBuilder row = new("tr");
            if (RowClassName != null)
                row.AddCssClass(RowClassName);

            TagBuilder property = new("td");
            property.InnerHtml.Append(nameProperty);
            if (PropertyClassName != null)
                property.AddCssClass(PropertyClassName);

            TagBuilder value = new("td");
            if (ValueClassName != null)
                value.AddCssClass(ValueClassName);

            TagBuilder input = new("input");
            foreach (var attr in attrs)
                input.MergeAttribute(attr.Key, attr.Value);
            input.MergeAttribute("name", nameProperty.ToLower());

            value.SetChild(input);
            row.SetChild(property, value);
            return row;
        }

        private IEnumerable<(string, string?)> ConvertAttribute(Attribute attribute, PropertyInfo property)
        {
            List<(string, string?)> res = new();
            if (attribute is DisplayAttribute display)
            {
                res.Add(("type", (display.Type ?? ConvertPropertyToInputType(property).ToString()).ToLower()));
                if(display.Classes != null)
                    res.Add(("class", string.Join(' ', display.Classes)));
            }
            else if (attribute is StringLengthAttribute length)
            {
                res.Add(("minlength", length.MinimumLength.ToString()));
                res.Add(("maxlength", length.MaximumLength.ToString()));
            }
            else if (attribute is RequiredAttribute required)
            {
                res.Add(("required", null));
            }
            return res;
        }
        private InputTypes ConvertPropertyToInputType(PropertyInfo property)
        {
            var type = property.PropertyType;
            if (type == typeof(int))
                return InputTypes.Number;
            if (type == typeof(DateTime))
                return InputTypes.DateTime;
            if (type == typeof(DateOnly))
                return InputTypes.Date;
            if (type == typeof(TimeOnly))
                return InputTypes.Time;
            return InputTypes.Text;
        }

    }
    public static class TagBuilderExtension
    {
        public static TagBuilder SetChild(this TagBuilder parent, params TagBuilder[] childs)
        {
            using var sw = new StringWriter();
            foreach (var child in childs)
                child.WriteTo(sw, HtmlEncoder.Default);
            parent.InnerHtml.AppendHtml(sw.ToString());
            return parent;
        }
    }
}
