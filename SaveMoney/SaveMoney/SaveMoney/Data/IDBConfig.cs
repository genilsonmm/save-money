using SQLite.Net.Interop;

namespace SaveMoney.Data
{
    public interface IDBConfig
    {
        string Diretory { get; }
        ISQLitePlatform Platform { get; }
    }
}
