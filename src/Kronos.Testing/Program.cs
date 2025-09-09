using System.Text.Json;
using CopperDevs.Kronos;
using CopperDevs.Kronos.Data;
using CopperDevs.Kronos.Testing;

var loader = new ResourceLoader(typeof(ResourceLoader).Assembly, "CopperDevs.Kronos.Testing.Resources");

var server = new Server()
    .Get("/", () => "Hello, World!", ReturnType.Text)
    .Get("/json", () => JsonSerializer.Serialize(new Dictionary<string, string>()
    {
        ["Hello"] = "World!"
    }), ReturnType.Json)
    .Get("/html", () => loader.LoadTextResource("Testing.html"), ReturnType.Html)
    .Post("/", () => "Hello, World!", ReturnType.Text);


server.Listen(3000); // blocking