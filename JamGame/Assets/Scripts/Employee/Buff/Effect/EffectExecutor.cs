public interface IEffectExecutor { }

public interface IEffectExecutor<E> : IEffectExecutor
    where E : class, IEffect
{
    public void RegisterEffect(E effect);
    public void UnregisterEffect(E effect);
}
