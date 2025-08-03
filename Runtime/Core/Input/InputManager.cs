using System.Threading.Tasks;
using Huan.Character;

namespace Huan.Framework.Core.Input
{
    public class InputManager : BaseManager<InputManager>
    {
        #region API

        public static IA_Input InputMaps => _instance._inputMaps;

        #endregion

        private IA_Input _inputMaps;

        protected override Task InitInternal()
        {
            _inputMaps = new IA_Input();
            return Task.CompletedTask;
        }

        protected override void CleanupInternal()
        {
            _inputMaps = null;
        }
    }
}