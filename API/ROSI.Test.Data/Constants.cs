namespace ROSI.Test.Data;

public static class Constants {
    public static string AppveyorServer => @"(local)\SQL2017";
    public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
    public static string Server => LocalServer;
#endif

    public static readonly string CsROSI = @$"Data Source={Server};Initial Catalog={"ROSITests"};Integrated Security=True;Encrypt=False;";
    public static readonly string CsRATL = @$"Data Source={Server};Initial Catalog={"RATLTests"};Integrated Security=True;Encrypt=False;";
}