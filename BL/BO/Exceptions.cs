namespace BO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}


public class BllAlreadyExistException : Exception
{
    public BllAlreadyExistException(string? message) : base(message) { }
    public BllAlreadyExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

public class BllDeletionImpossible : Exception
{
    public BllDeletionImpossible(string? message) : base(message) { }
    public BllDeletionImpossible(string message, Exception innerException)
               : base(message, innerException) { }
}

public class BlNullReferenceException : Exception
{
    public BlNullReferenceException(string? message) : base(message) { }
    public BlNullReferenceException(string message, Exception innerException)
              : base(message, innerException) { }
}

public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
    public BlFormatException(string message, Exception innerException)
             : base(message, innerException) { }
}

public class BlFileLoadCreateException : Exception////////////////////////////////////////////
{
    public BlFileLoadCreateException(string? message) : base(message) { }
    public BlFileLoadCreateException(string message, Exception innerException)
            : base(message, innerException) { }
}


///////////////////////////////////////////////

[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

