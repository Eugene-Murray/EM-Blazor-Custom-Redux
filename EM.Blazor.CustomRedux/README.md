# Blazor.CustomRedux

`Blazor.CustomRedux` packages a reusable immutable Redux-style store for Blazor with:

- typed actions and reducers
- Rx.NET-powered observable state updates and effects
- optional browser `localStorage` hydration/persistence
- a generic debug panel component with action history

## Install

```powershell
dotnet add package Blazor.CustomRedux
```

## Register a store

```csharp
using Blazor.CustomRedux.Store;

builder.Services.AddCustomReduxStore<TodoState, TodoReducer>(options =>
{
    options.InitialStateFactory = TodoState.CreateDefault;
    options.LocalStorageKey = "my-app.todo-store";
});
```

Register additional effects with DI by implementing `IReduxStoreEffect<TState>` and adding them as services before building the app.

## Use in components

```razor
@using Blazor.CustomRedux.Components
@using Blazor.CustomRedux.Store

<ReduxStoreBootstrapper TState="TodoState" />
<ReduxDebugPanel TState="TodoState" />
```

Inject the store anywhere:

```csharp
[Inject] private ReduxStore<TodoState> Store { get; set; } = default!;
```

## Pack and publish

```powershell
dotnet pack .\Blazor.CustomRedux\Blazor.CustomRedux.csproj -c Release
dotnet nuget push .\Blazor.CustomRedux\bin\Release\Blazor.CustomRedux.1.0.0.nupkg --source https://api.nuget.org/v3/index.json
```
