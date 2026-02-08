using System.Diagnostics.CodeAnalysis;

namespace Artimora.Kronos;

public readonly struct RequestMethod(string method) : IEquatable<RequestMethod>
{
    public readonly string Method = method;

    public static implicit operator string(RequestMethod method) => method.Method;

    public static implicit operator RequestMethod(string method) => new(method);

    public static readonly RequestMethod Get = new("GET");
    public static readonly RequestMethod Post = new("POST");
    public static readonly RequestMethod Patch = new("PATCH");
    public static readonly RequestMethod Put = new("PUT");
    public static readonly RequestMethod Delete = new("DELETE");

    public override int GetHashCode()
    {
        return Method.GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public static bool operator ==(RequestMethod left, RequestMethod right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RequestMethod left, RequestMethod right)
    {
        return !(left.Equals(right));
    }

    public bool Equals(RequestMethod other)
    {
        return Method == other.Method;
    }
}