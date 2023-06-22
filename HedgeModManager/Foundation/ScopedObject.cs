namespace HedgeModManager.Foundation;

public struct ScopedObject<TObject> : IDisposable where TObject : IDisposable
{
    public TObject? Object { get; set; }

    public ScopedObject(TObject? obj)
    {
        Object = obj;
    }

    public void Set(TObject? obj)
    {
        Object?.Dispose();
        Object = obj;
    }

    public void Reset()
    {
        Set(default);
    }

    public void Dispose()
    {
        Reset();
    }
}