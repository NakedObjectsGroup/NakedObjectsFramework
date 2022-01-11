namespace AdventureWorksLegacy.AppLib;

public static class ServiceLocatorFactory {
    public static ServiceLocator ServiceLocator => new(ThreadLocals.Container);
}