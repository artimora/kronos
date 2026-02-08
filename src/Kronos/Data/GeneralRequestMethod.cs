using System.Diagnostics.CodeAnalysis;

namespace Artimora.Kronos.Data;

public readonly struct GeneralRequestMethod(string method) : IEquatable<GeneralRequestMethod>
{
    public readonly string Method = method;

    public static implicit operator string(GeneralRequestMethod method) => method.Method;

    public static implicit operator GeneralRequestMethod(string method) => new(method);

    public static implicit operator GeneralRequestMethod(RequestMethod method) => new(Server.GetMethod(method));

    public override int GetHashCode()
    {
        return Method.GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public static bool operator ==(GeneralRequestMethod left, GeneralRequestMethod right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GeneralRequestMethod left, GeneralRequestMethod right)
    {
        return !(left.Equals(right));
    }

    public bool Equals(GeneralRequestMethod other)
    {
        return Method == other.Method;
    }
}