// type aliases

global using UserRequestMethod = System.Func<Artimora.Kronos.Data.RequestData, string>;
global using UserRequestMethodDataless = System.Func<string>;
global using UserRequestMethodData = System.Tuple<System.Func<Artimora.Kronos.Data.RequestData, string>, Artimora.Kronos.Data.ReturnType>;
