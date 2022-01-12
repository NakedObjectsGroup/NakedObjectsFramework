
namespace AdventureWorksLegacy.AppLib;

public class ServiceLocator {
    private readonly IContainer container;

    public ServiceLocator(IContainer container) => this.container = container;

    public object Repository(Type ofType) => container.Repository(ofType);
}