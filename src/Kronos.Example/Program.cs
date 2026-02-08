using Artimora.Kronos;

new Server.Builder
    {
        ["/"] =
        {
            [RequestMethod.Get] = d => d.Text("Hello, World!")
        }
    }
    .Build()
    .Listen();