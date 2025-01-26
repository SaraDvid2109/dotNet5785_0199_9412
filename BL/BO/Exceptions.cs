namespace BO;

[Serializable]

/// <summary>
/// Exception thrown when the business logic entity (Bl) does not exist.
/// </summary>
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when the business logic entity (Bl) already exists.
/// </summary>
public class BllAlreadyExistException : Exception
{
    public BllAlreadyExistException(string? message) : base(message) { }
    public BllAlreadyExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when it is impossible to delete a business logic entity (Bl).
/// </summary>
public class BllDeletionImpossible : Exception
{
    public BllDeletionImpossible(string? message) : base(message) { }
    public BllDeletionImpossible(string message, Exception innerException)
               : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a null reference is encountered in the business logic layer.
/// </summary>
public class BlNullReferenceException : Exception
{
    public BlNullReferenceException(string? message) : base(message) { }
    public BlNullReferenceException(string message, Exception innerException)
              : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when it is impossible to delete a business logic entity (Bl).
/// </summary>
public class BlFormatException : Exception
{
    public BlFormatException(string? message) : base(message) { }
    public BlFormatException(string message, Exception innerException)
             : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when a file loading or creation operation fails in the business logic layer.
/// </summary>
public class BlFileLoadCreateException : Exception////////////////////////////////////////////
{
    public BlFileLoadCreateException(string? message) : base(message) { }
    public BlFileLoadCreateException(string message, Exception innerException)
            : base(message, innerException) { }
}


/////////////////////Exceptions for an internal fault within the logical layer/////////////////////

/// <summary>
/// Exception thrown when using a null attribute value.
/// </summary>
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

/// <summary>
/// Exception to the problem with unauthorized access.
/// </summary>
public class UnauthorizedAccessException: Exception
{
    public UnauthorizedAccessException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the operation is not allowed due to business rules or system constraints.
/// </summary>
public class BlOperationNotAllowedException : Exception
{
    public BlOperationNotAllowedException(string? message) : base(message) { }
}

/// <summary>
/// Exception thrown when the data in the business logic layer is invalid.
/// </summary>
public class BlInvalidData : Exception
{
    public BlInvalidData(string? message) : base(message) { }
}

public class BlDeletionImpossible : Exception
{
    public BlDeletionImpossible(string? message) : base(message) { }
}

public class BlDistance : Exception
{
    public BlDistance(string? message) : base(message) { }
}

public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string? message) : base(message) { }
}






