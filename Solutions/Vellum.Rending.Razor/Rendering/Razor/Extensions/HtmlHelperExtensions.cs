using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Vellum.Cli.Rendering.Razor.Extensions;

public static class HtmlHelperExtensions
{
    private const string _partialViewScriptItemPrefix = "scripts_";

    public static IHtmlContent PartialSectionScripts(this IHtmlHelper htmlHelper, Func<object, HelperResult> template)
    {
        htmlHelper.ViewContext.HttpContext.Items[_partialViewScriptItemPrefix + Guid.NewGuid()] = template;
        return new HtmlContentBuilder();
    }

    public static IHtmlContent RenderPartialSectionScripts(this IHtmlHelper htmlHelper)
    {
        IEnumerable<object> partialSectionScripts = htmlHelper.ViewContext.HttpContext.Items.Keys
            .Where(k => Regex.IsMatch(
                k.ToString() ?? string.Empty,
                "^" + _partialViewScriptItemPrefix +
                "([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$"));

        HtmlContentBuilder contentBuilder = new();

        foreach (object key in partialSectionScripts)
        {
            if (htmlHelper.ViewContext.HttpContext.Items[key] is Func<object, HelperResult> template)
            {
                StringWriter writer = new();
                template(null!).WriteTo(writer, HtmlEncoder.Default);
                contentBuilder.AppendHtml(writer.ToString());
            }
        }

        return contentBuilder;
    }
}