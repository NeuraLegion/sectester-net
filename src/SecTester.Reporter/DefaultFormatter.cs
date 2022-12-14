using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SecTester.Scan.Models;

namespace SecTester.Reporter;

public class DefaultFormatter : IFormatter
{
  private const string CrLf = "\r\n";
  private const char NewLine = '\n';
  private const char BulletPoint = '●';
  private const char Tabulation = '\t';
  private const string TemplateBody = @"Issue in Bright UI:   {0}
Name:                 {1}
Severity:             {2}
Remediation:
{3}
Details:
{4}";
  private const string TemplateExtraDetails = "Extra Details:\n{5}";
  private const string TemplateReferences = "References:\n{6}";

  public string Format(Issue issue)
  {
    var comments = issue.Comments ?? Array.Empty<Comment>();
    var resources = issue.Resources ?? Array.Empty<string>();

    var template = GenerateTemplate(comments.Any(), resources.Any());

    var message = string.Format(CultureInfo.InvariantCulture,
      template,
      issue.Link,
      issue.Name,
      issue.Severity,
      issue.Remedy,
      issue.Details,
      FormatList(comments, FormatExtraInfo),
      FormatList(resources)
    );

    // ADHOC: adjust line endings respectively for OS environment
    return message.Trim().Replace(NewLine.ToString(), Environment.NewLine);
  }

  private static string GenerateTemplate(bool extraInfo, bool references)
  {
    IEnumerable<string> templates = new List<string>
    {
      TemplateBody
    };

    if (extraInfo)
    {
      templates = templates.Append(TemplateExtraDetails);
    }

    if (references)
    {
      templates = templates.Append(TemplateReferences);
    }

    // ADHOC: `DefaultFormatter` works with LF, replace possible CRLF in verbatim strings
    templates = templates.Select(x => x.Replace(CrLf, NewLine.ToString()));

    return string.Join(NewLine.ToString(), templates);
  }

  private static string FormatExtraInfo(Comment comment)
  {
    var footer = comment.Links is not null && comment.Links.Any() ? CombineList(comment.Links.Prepend("Links:")) : "";
    var blocks = new List<string> { comment.Text ?? "", footer }.Select(x => Indent(x));
    var document = CombineList(blocks);

    return CombineList(new List<string> { comment.Headline, document });
  }

  private static string Indent(string x, int length = 1)
  {
    // ADHOC: `DefaultFormatter` works with LF, replace possible CRLF for consistent parsing
    var lines = x.Replace(CrLf, NewLine.ToString()).Split(NewLine);

    return CombineList(
      lines.Select(line => $"{new string(Tabulation, length)}{line}")
    );
  }

  private static string FormatList<T>(IEnumerable<T> list, Func<T, string>? map = default)
    where T : class
  {
    var formatItem = map ?? (x => x.ToString());

    var items = list.Select(
      x => $"{BulletPoint} {formatItem(x)}"
    );

    return CombineList(items);
  }

  private static string CombineList(IEnumerable<string> list, char sep = NewLine)
  {
    return string.Join(sep.ToString(), list);
  }
}
