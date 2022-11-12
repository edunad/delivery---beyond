
using System;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class EditorButtonAttribute : Attribute {
    public readonly string name;
    public EditorButtonAttribute(string name) {
        this.name = name;
    }
}
