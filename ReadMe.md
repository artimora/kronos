# Kronos

## Documentation
> very terrible and quick docs but it better than nothing?

### Basic Example

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
