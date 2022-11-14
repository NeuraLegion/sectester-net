namespace SecTester.Core.Bus;

[System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct)]
public class MessageTypeAttribute : System.Attribute
{
  public string Name { get; }

  public MessageTypeAttribute(string name)
  {
    Name = name;
  }
}
