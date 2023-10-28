# GigaChatSharp

A class library for quick access to Sber's GigaChat AI in c#.

## Can usage

### Get list of models
```csharp
var GigaChatClient = new GigaChat("CLIENT_SECRET", "AUTH_DATA")
await GigaChatClient.Authorize();

var ModelsArray = await GigaChatClient.GetModels();
ModelsArray.ToList().ForEach(model => {
  Console.WriteLine(model.id);
});
```
