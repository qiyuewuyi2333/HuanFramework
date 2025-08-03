using System.Threading.Tasks;

namespace Huan.Framework.Core
{
    public interface IManager
    {
        public Task Init();
        public void Cleanup();
    }
}