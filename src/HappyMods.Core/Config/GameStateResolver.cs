namespace HappyMods.Core.Config;

public static class StateCache
{
    public static State? State { get; set; }
}

public interface IGameState<out T> where T : class
{
    T? State { get; }
}

public class GameState<T>(ILogger logger) : IGameState<T> where T : class
{
    private T? _state = null;
    
    public T? State
    {
        get
        {
            if (_state is not null) return _state;

            if (StateCache.State is null)
            {
                logger.Error("Game state has not been set in cache");
            }
            
            return State ??= StateCache.State?.Get<T>();
        }
        private set => _state = value;
    }
}