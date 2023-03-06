using System.Net.Http.Headers;

namespace ROSI.Interfaces;

public interface IInvokeOptions
{
    string? Token { get; }
    EntityTagHeaderValue? Tag { get; }
    HttpClient HttpClient { get; }
    IDictionary<string, object>? ReservedArguments { get; set; }
}