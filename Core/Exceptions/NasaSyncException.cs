namespace Core.Exceptions;

public sealed class NasaSyncException(string message, Exception? inner = null) : Exception(message, inner);