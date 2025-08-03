namespace Huan.Framework.Runtime.Core
{
    public interface IGameBootstrap
    {
        void Init();
        void UpdateGame(float deltaTime, float unscaledDeltaTime);
        void Cleanup();
    }
}