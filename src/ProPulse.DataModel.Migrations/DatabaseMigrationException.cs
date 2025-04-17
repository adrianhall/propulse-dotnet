using System.Diagnostics.CodeAnalysis;

namespace ProPulse.DataModel.Migrations;

/// <summary>
/// An exception thrown by the DatabaseMigrator when a migration fails.
/// </summary>
public class DatabaseMigrationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseMigrationException"/> class.
    /// </summary>
    public DatabaseMigrationException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseMigrationException"/> class with 
    /// a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DatabaseMigrationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseMigrationException"/> class with 
    /// a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public DatabaseMigrationException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
