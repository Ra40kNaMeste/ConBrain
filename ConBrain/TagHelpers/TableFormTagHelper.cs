using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Encodings.Web;

namespace ConBrain.TagHelpers
{
    //Здесь определён tag-helper, который позволяет генерировать форму в виде таблицы по некоторым данным
    //Пример формы
    //<form asp-controller="" asp-action="">
    //    <table>
    //        <caption>name</caption>
    //        <tr class="row-class-name">
    //            <td class="property-class-name">Nick</td>
    //            <td class="value-class-name">
    //                <input name = "name" value="value" required maxlength="50" minlength="5" />
    //            </td>
    //        </tr>
    //    </table>
    //    <button name="send">save-button-content</button>
    //</form>
    //При его использовании необходимо пометить все свойства класса аттрибутом DisplayAttribute
    //Поддерживает такие аттрибуты валидации, как Required, MaxLength, MinLength, StringLength

    /// <summary>
    /// Аттрибут для TableFormTagHelper
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayAttribute : Attribute
    {
        public DisplayAttribute() { }
        public DisplayAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Имя поля в форме
        /// </summary>
        public string? Name { get; }
        /// <summary>
        /// Тип input-элемента
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// Классы input-элемента
        /// </summary>
        public string[]? Classes { get; set; }
    }

    /// <summary>
    /// Генерирует форму в виде таблицы
    /// Пример:
    /// name
    /// property value
    /// property value
    /// ...
    /// [request]
    /// </summary>
    public class TableFormTagHelper : FormTagHelper
    {
        public TableFormTagHelper(IHtmlGenerator generator) : base(generator)
        {
            RequestButtonContent = "Request";
        }

        /// <summary>
        /// Объект по которому генерируется форма
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// Имя таблицы
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Класс для каждой строки таблицы
        /// </summary>
        public string RowClassName { get; set; }

        /// <summary>
        /// Класс для ячейки с названием свойства
        /// </summary>
        public string PropertyClassName { get; set; }

        /// <summary>
        /// Класс для ячейки с input-элементом (значением)
        /// </summary>
        public string ValueClassName { get; set; }

        /// <summary>
        /// Название кнопки отправки формы
        /// </summary>
        public string RequestButtonContent { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            //Переименновываем форму
            output.TagName = "form";

            //Генерируем таблицу
            TagBuilder table = new("table");
            List<TagBuilder> rows = new(); //Массив строк таблицы
            
            //Добавляем название таблицы
            var name = GenerateTableName();
            if (name != null)
                rows.Add(name);



            //Ищем все свойства объекта
            var type = Target.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                //Если есть среди аттрибутов свойства аттрибут DisplayAttribute, то добавляем свойство в таблицу
                var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                {
                    //Находим все аттрибуты блока input
                    var attrs = property.GetCustomAttributes()
                        .Select(i => ConvertAttribute(i, property))
                        .SelectMany(i => i)
                        .DistinctBy(i=>i.Key)
                        .ToDictionary(i => i.Key, i => i.Value);

                    //Добавляем начальное значение аттрибута - такое же что и у объекта
                    attrs.Add("value", property.GetValue(Target)?.ToString());

                    //Генерируем строку
                    rows.Add(GenerateRow(displayAttr.Name, property.Name, attrs));
                }
            }

            //Добавляем кнопку отправки формы
            TagBuilder button = new("button");
            button.InnerHtml.Append(RequestButtonContent);
            button.Attributes.Add("name", "send");

            //Записываем сгенерированный html-код в элемент
            table.SetChild(rows.ToArray());
            using var sw = new StringWriter();
            table.WriteTo(sw, HtmlEncoder.Default);
            button.WriteTo(sw, HtmlEncoder.Default);
            output.Content.AppendHtml(sw.ToString());
        }

        /// <summary>
        /// Генерирует имя таблицы
        /// </summary>
        /// <returns>Имя таблицы</returns>
        private TagBuilder? GenerateTableName()
        {
            if (TableName != null)
            {
                TagBuilder nameBuilder = new("caption");
                nameBuilder.InnerHtml.Append(TableName);
                return nameBuilder;
            }
            return null;
        }

        /// <summary>
        /// Генерирует строку таблицы
        /// </summary>
        /// <param name="nameProperty">Название свойства в форме</param>
        /// <param name="originalName">Название свойства</param>
        /// <param name="attrs">аттрибуты для input-элемента</param>
        /// <returns></returns>
        private TagBuilder GenerateRow(string nameProperty, string originalName, Dictionary<string, string?> attrs)
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
            input.MergeAttribute("name", originalName.ToLower());

            value.SetChild(input);
            row.SetChild(property, value);
            return row;
        }

        /// <summary>
        /// Конвертирует аттрибут свойства в аттрибут input-элемента 
        /// При добавлении поддержки новых аттрибутов функцианальность добавляется сюда
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private Dictionary<string, string?> ConvertAttribute(Attribute attribute, PropertyInfo property)
        {
            Dictionary<string, string?> res = new();
            if (attribute is DisplayAttribute display)
            {
                res.Add("type", (display.Type ?? ConvertPropertyToInputType(property).ToString()).ToLower());
                if(display.Classes != null)
                    res.Add("class", string.Join(' ', display.Classes));
            }
            else if(attribute is MaxLengthAttribute maxlength)
            {
                res.Add("maxlength", maxlength.Length.ToString());
            }
            else if (attribute is MinLengthAttribute minlength)
            {
                res.Add("minlength", minlength.Length.ToString());
            }
            else if (attribute is StringLengthAttribute length)
            {
                res.Add("minlength", length.MinimumLength.ToString());
                res.Add("maxlength", length.MaximumLength.ToString());
            }
            else if (attribute is RequiredAttribute required)
            {
                res.Add("required", null);
            }
            return res;
        }

        /// <summary>
        /// Конвертер типа свойства в тип input-элемента. Используется при неуказании такого в DisplayAttribute
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private string ConvertPropertyToInputType(PropertyInfo property)
        {
            var type = property.PropertyType;
            if (type == typeof(int))
                return "number";
            if (type == typeof(DateTime))
                return "datetime";
            if (type == typeof(DateOnly))
                return "date";
            if (type == typeof(TimeOnly))
                return "time";
            return "text";
        }

    }
    public static class TagBuilderExtension
    {
        /// <summary>
        /// Добавляет к тэгу наследников в виде текста и возвращает его.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childs"></param>
        /// <returns></returns>
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
