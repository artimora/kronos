using System.Text.Json;
using Artimora.Kronos.Data;

namespace Artimora.Kronos.Testing;

public static class Program
{
    private static readonly ResourceLoader Loader = new(typeof(ResourceLoader).Assembly, "Artimora.Kronos.Testing.Resources");

    private static void Main()
    {
        var server = new Server
        {
            Get =
            {
                ["/"] = d => d.Text("Hello World!"),
                ["/json"] = d => d.Json(new Dictionary<string, string>()
                {
                    ["Hello"] = "World!"
                }),
                ["/html"] = d => d.Html(Loader.LoadTextResource("Testing.html")),
                ["/test/:id"] = d =>
                {
                    var id = d.GetParam("id");

                    return d.Text($"{nameof(id)}: {id}");
                },
                ["/test/:id/:value"] = d =>
                {
                    var id = d.GetParam("id");
                    var value = d.GetParam("value");

                    return d.Text($"{nameof(id)}: {id} | {nameof(value)}: {value}");
                },
                ["/:id/:value"] = d =>
                {
                    var id = d.GetParam("id");
                    var value = d.GetParam("value");

                    return d.Text($"{nameof(id)}: {id} | {nameof(value)}: {value}");
                }
            },
            Post =
            {
                ["/"] = d => d.Json(new Dictionary<string, string>()
                {
                    ["hello"] = "world!",
                    ["content"] = d.BodyTextContents ?? string.Empty
                })
            }
        };


        server.Listen(3000, 4321, 5172); // blocking
    }
}