# GigaChatSharp

A class library for quick access to Sber's GigaChat AI in c#.

## Can usage

### Get list of models
```csharp
var GigaChatClient = new GigaChat("CLIENT_SECRET", "AUTH_DATA", Scope.GIGACHAT_API_PERS);
await GigaChatClient.Authorize();

var ModelsArray = await GigaChatClient.GetModels();
ModelsArray.ToList().ForEach(model => {
  Console.WriteLine(model.id);
});
```

### Get new access token when will old token in expire
```csharp
GigaChatClient.AccessTokenExpiredHandler += async (args) => {
  await args.ReAuthorize();
};
```
