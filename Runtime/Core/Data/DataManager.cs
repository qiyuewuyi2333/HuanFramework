using System.Threading.Tasks;
using cfg;
using Huan.Framework.Runtime.Core.Asset;
using Luban;
using UnityEngine;

namespace Huan.Framework.Core.Data
{
    // TODO: 懒加载改造
    public class DataManager : BaseManager<DataManager>
    {
        private Tables _tables;
        public static Tables Tables => _instance._tables;

        protected override Task InitInternal()
        {
            _tables = new Tables(LoadByteBuf);
            return Task.CompletedTask;
        }

        public void InitTables()
        {
            _tables = new Tables(LoadByteBuf);
        }
        private static ByteBuf LoadByteBuf(string file)
        {
            return new ByteBuf(AssetManager.LoadAsset<TextAsset>($"Assets/ResourcesAB/GenerateData/{file}.bytes").bytes);
        }

        protected override void CleanupInternal()
        {
            
        }
    }
}