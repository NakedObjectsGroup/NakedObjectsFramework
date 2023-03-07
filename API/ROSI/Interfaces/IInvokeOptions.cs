using System.Net.Http.Headers;
using System.Reflection;

namespace ROSI.Interfaces;

public interface IInvokeOptions {
    string? Token { get; }
    EntityTagHeaderValue? Tag { get; }
    HttpClient HttpClient { get; }
    IDictionary<string, object> ReservedArguments { get; }

    IInvokeOptions Copy();
}