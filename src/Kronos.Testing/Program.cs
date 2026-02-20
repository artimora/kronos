namespace Artimora.Kronos.Testing;

public static class Program
{
    private static readonly ResourceLoader Loader = new(typeof(ResourceLoader).Assembly, "Artimora.Kronos.Testing.Resources");

    private static void Main()
    {
        var builder = new Server.Builder
        {
            ["/"] =
            {
                [RequestMethod.Get] = d => d.Text("Hello World!"),
                [RequestMethod.Post] = d => d.Json(new Dictionary<string, string>()
                {
                    ["hello"] = "world!",
                    ["content"] = d.BodyTextContents ?? string.Empty
                })
            },
            ["/404"] =
            {
                [RequestMethod.Get] = d => d.Text("route not found lol"),
            },
            ["/exception"] =
            {
                [RequestMethod.Get] = d => throw new Exception()
            },
            ["/json"] =
            {
                [RequestMethod.Get] = d => d.Json(new Dictionary<string, string>()
                {
                    ["Hello"] = "World!"
                })
            },
            ["/html"] = { [RequestMethod.Get] = d => d.Html(Loader.LoadTextResource("Testing.html")) },
            ["/test/:id"] =
            {
                [RequestMethod.Get] = d =>
                {
                    var id = d.GetParam("id");

                    return d.Text($"{nameof(id)}: {id}");
                }
            },
            ["/test/:id/:value"] =
            {
                [RequestMethod.Get] = d =>
                {
                    var id = d.GetParam("id");
                    var value = d.GetParam("value");

                    return d.Text($"{nameof(id)}: {id} | {nameof(value)}: {value}");
                }
            },
            ["/:id/:value"] =
            {
                [RequestMethod.Get] = d =>
                {
                    var id = d.GetParam("id");
                    var value = d.GetParam("value");

                    return d.Text($"{nameof(id)}: {id} | {nameof(value)}: {value}");
                }
            }
        };

        var server = builder.Build();

        server.Listen(); // blocking
    }
}