using System.Text.Json;
using Artimora.Kronos.Data;

namespace Artimora.Kronos.Testing;

public static class Program
{
    private static readonly ResourceLoader loader = new(typeof(ResourceLoader).Assembly, "Artimora.Kronos.Testing.Resources");

    private static void Main()
    {
        var server = new Server()
            .Get("/", () => "Hello, World!", ReturnType.Text)
            .Get("/json", () => JsonSerializer.Serialize(new Dictionary<string, string>()
            {
                ["Hello"] = "World!"
            }), ReturnType.Json)
            .Get("/html", () => loader.LoadTextResource("Testing.html"), ReturnType.Html)
            .Post("/", (d) => JsonSerializer.Serialize(new Dictionary<string, string>()
            {
                ["hello"] = "world!",
                ["content"] = d.BodyTextContents ?? string.Empty
            }), ReturnType.Json)
            .Get("/test/:id", (d) =>
            {
                var id = d.GetParam("id");

                return $"{nameof(id)}: {id}";
            }, ReturnType.Text)
            .Get("/test/:id/:value", (d) =>
            {

                var id = d.GetParam("id");
                var value = d.GetParam("value");

                return $"{nameof(id)}: {id} | {nameof(value)}: {value}";
            }, ReturnType.Text)
            .Get("/:id/:value", (d) =>
            {

                var id = d.GetParam("id");
                var value = d.GetParam("value");

                return $"{nameof(id)}: {id} | {nameof(value)}: {value}";
            }, ReturnType.Text);

        server.iGet["/:id/:value"] = (d) =>
            {

                var id = d.GetParam("id");
                var value = d.GetParam("value");

                return $"{nameof(id)}: {id} | {nameof(value)}: {value}";
            };

        server.Listen(3000, 4321, 5172); // blocking
    }
}