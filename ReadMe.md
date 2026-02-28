# Kronos

## Documentation

> very terrible and quick docs but it better than nothing?

### Basic Example

This is the bare minimum for a server with a default response message

```csharp
new Server.Builder
    {
        ["/"] =
        {
            [RequestMethod.Get] = d => d.Text("Hello, World!")
        }
    }
    .Build()
    .Listen();
```

These two methods are the same because `RequestMethod.Get` maps to `GET` internally. When you visit `/` it displays
`Hello, World! (twice?)`

```csharp
new Server.Builder
    {
        ["/"] =
        {
            [RequestMethod.Get] = d => d.Text("Hello, World!"),
            ["GET"] = d => d.Text("Hello, World! (twice?)"),
        }
    }
    .Build()
    .Listen();
```

### URL Params

Visiting `http://localhost:3000/test` will return `id: test`

```csharp
new Server.Builder
    {
        ["/:id"] =
        {
            [RequestMethod.Get] = d =>
            {
                var id = d.GetParam("id");

                return d.Text($"{nameof(id)}: {id}");
            }
        },
    }
    .Build()
    .Listen();
```

### Body

#### JSON

Visiting `http://localhost:3000/json` will return:

```json
{
  "Hello": "World!"
}
```

```csharp
new Server.Builder
    {
        ["/json"] =
        {
            [RequestMethod.Get] = d => d.Json(new Dictionary<string, string>()
            {
                ["Hello"] = "World!"
            })
        },
    }
    .Build()
    .Listen();
```

#### HTML

```csharp
new Server.Builder
    {
        ["/html"] =
        {
            [RequestMethod.Get] = d => // dont worry about the weird formatting :3
                d.Html("<!DOCTYPE html>\n<html lang=\"en\">\n<head>\n    <meta charset=\"UTF-8\">\n    <title>Title</title>\n</head>\n<body>\n    <h1>Hello, World!</h1>\n</body>\n</html>")
        },
    }
    .Build()
    .Listen();
```

### Groups

```csharp
var apiV1 = new Server.Builder
{
    ["/ping"] =
    {
        [RequestMethod.Get] = d => d.Text("pong")
    }
};
        
var apiV2 = new Server.Builder
{
    ["/debug/ping"] =
    {
        [RequestMethod.Get] = d => d.Text("pong")
    }
};

new Server.Builder()
    .AddGroup("/api/v1/", apiV1)
    .AddGroup("/api/v2/", apiV2)
    .Build()
    .Listen();

// resulting paths: 
// /api/v1/ping
// /api/v1/debug/ping
```