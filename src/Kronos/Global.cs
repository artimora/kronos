// type aliases

global using UserRequestMethod = System.Func<CopperDevs.Kronos.Data.RequestData, string>;
global using UserRequestMethodDataless = System.Func<string>;
global using UserRequestMethodData = System.Tuple<System.Func<CopperDevs.Kronos.Data.RequestData, string>, CopperDevs.Kronos.Data.ReturnType>;