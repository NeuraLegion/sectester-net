using System;
using System.Threading.Tasks;

namespace SecTester.Core;

// Based on https://github.com/jbogard/MediatR/blob/master/src/MediatR.Contracts/Unit.cs
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
  private static readonly Unit _value = new();

  public static ref readonly Unit Value => ref _value;

  public static Task<Unit> Task { get; } = System.Threading.Tasks.Task.FromResult(_value);

  public int CompareTo(Unit other) => 0;

  int IComparable.CompareTo(object? obj) => 0;

  public override int GetHashCode() => 0;

  public bool Equals(Unit other) => true;

  public override bool Equals(object? obj) => obj is Unit;

  public override string ToString() => "()";

  public static bool operator ==(Unit first, Unit second) => true;

  public static bool operator !=(Unit first, Unit second) => false;

  public static bool operator <(Unit left, Unit right) => false;

  public static bool operator <=(Unit left, Unit right) => true;

  public static bool operator >(Unit left, Unit right) => false;

  public static bool operator >=(Unit left, Unit right) => true;
}
