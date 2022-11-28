using System;
namespace SecTester.Scan.Models;

public record Identifiable<T>(T Id)
{
  public T Id { get; init; } = Id ?? throw new ArgumentNullException(nameof(Id));
};
