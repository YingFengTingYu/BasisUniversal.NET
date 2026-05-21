using System;

namespace BasisUniversal;

public sealed class BasisUniversalException : Exception
{
    public BasisUniversalException(string message)
        : base(message)
    {
    }
}
