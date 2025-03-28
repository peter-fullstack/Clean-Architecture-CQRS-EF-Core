﻿namespace Application.Exceptions;

public class ApplicationValidationException : Exception
{
    private readonly List<string> _errors = new();
    public ApplicationValidationException(string message, List<string> errors) : base(message)
    {
        _errors = errors;
    }

    public List<string> Errors
    {
        get
        {
            return _errors;
        }
    }
}
