using EM.Blazor.CustomRedux.Store;

namespace EM.Blazor_Custom_Redux.Store;

public sealed record IncrementCounterAction : IReduxAction;

public sealed record DecrementCounterAction : IReduxAction;

public sealed record ResetCounterAction : IReduxAction;
