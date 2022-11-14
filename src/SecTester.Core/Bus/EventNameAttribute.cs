namespace SecTester.Core.Bus;

[System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct)]
public class EventNameAttribute : System.Attribute
{
  public string Name { get; }

  public EventNameAttribute(string name)
  {
    Name = name;
  }
}
