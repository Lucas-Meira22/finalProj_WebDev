using System;
using System.Threading.Tasks;
using Microsoft.Maui.Networking;

namespace personalFinacialTrack.Resources.Model
{
    public class DataService
    {
        private readonly DatabaseHelper _onlineDb;
        private readonly SqliteHelper _offlineDb;

        public DataService()
        {
            _onlineDb = new DatabaseHelper();
            _offlineDb = new SqliteHelper();
        }

        private bool HasInternet => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            if (HasInternet)
            {
                try
                {
                    var list = await _onlineDb.GetTransactionsAsync(200);
                    foreach (var t in list)
                        await _offlineDb.SaveTransactionAsync(t);
                    return list;
                }
                catch
                {
                    return await _offlineDb.GetTransactionsAsync();
                }
            }
            else
            {
                return await _offlineDb.GetTransactionsAsync();
            }
        }

        public async Task AddTransactionAsync(Transaction t)
        {
            if (HasInternet)
            {
                try
                {
                    await _onlineDb.CreateTransactionAsync(t);
                    await _offlineDb.SaveTransactionAsync(t); 
                }
                catch
                {
                    await _offlineDb.SaveTransactionAsync(t); 
                }
            }
            else
            {
                await _offlineDb.SaveTransactionAsync(t); 
            }
        }
    }
}
