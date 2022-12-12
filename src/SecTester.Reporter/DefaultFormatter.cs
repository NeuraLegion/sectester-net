using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SecTester.Scan.Models;

namespace SecTester.Reporter;

public class DefaultFormatter : Formatter
{
  private const char BulletPoint = '●';
  private const char NewLine = '\n';
  private const char Tabulation = '\t';
  private const string TemplateBody = @"Issue in Bright UI:   {0}
Name:                 {1}
Severity:             {2}
Remediation:
{3}
Details:
{4}";
  private const string TemplateExtraDetails = "\nExtra Details:\n{5}";
  private const string TemplateReferences = "\nReferences:\n{6}";

  private static readonly IEnumerable<Comment> EmptyComments = new List<Comment>();
  private static readonly IEnumerable<string> EmptyResources = new List<string>();

  public string Format(Issue issue)
  {
    var comments = issue.Comments ?? EmptyComments;
    var resources = issue.Resources ?? EmptyResources;

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

    return message.Trim();
  }

  private static string GenerateTemplate(bool extraInfo, bool references)
  {
    var stringBuilder = new StringBuilder(TemplateBody);

    if (extraInfo)
    {
      stringBuilder.Append(TemplateExtraDetails);
    }

    if (references)
    {
      stringBuilder.Append(TemplateReferences);
    }

    return stringBuilder.ToString();
  }

  private static string FormatExtraInfo(Comment comment)
  {
    var footer = comment.Links is not null && comment.Links.Any() ? CombineList(comment.Links.Prepend("Links:")) : String.Empty;
    var blocks = new List<string> { comment.Text ?? string.Empty, footer }.Select(x => Indent(x));
    var document = CombineList(blocks);

    return CombineList(new List<string> { comment.Headline, document });
  }

  private static string Indent(string x, int length = 1)
  {
    var lines = x.Split(NewLine);

    return CombineList(
      lines.Select(line => $"{new string(Tabulation, length)}{line}")
    );
  }

  private static string FormatList<T>(IEnumerable<T> list, Func<T, string>? map = default)
    where T : class
  {
    var items = list.Select(
      x => $"{BulletPoint} {map?.Invoke(x) ?? x.ToString()}"
    );

    return CombineList(items);
  }

  private static string CombineList(IEnumerable<string> list, string? sep = default)
  {
    return string.Join(sep ?? NewLine.ToString(), list);
  }
}
