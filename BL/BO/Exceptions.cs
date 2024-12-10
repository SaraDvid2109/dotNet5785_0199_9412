﻿namespace BO;

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
/// Exception for violation of a requirement
/// </summary>
public class RequirementNotMetException : Exception
{
    public RequirementNotMetException(string? message) : base(message) { }
}
/// <summary>
/// Exception thrown when the operation is not allowed due to business rules or system constraints.
/// </summary>
public class OperationNotAllowedException : Exception
{
    public OperationNotAllowedException(string? message) : base(message) { }
}



