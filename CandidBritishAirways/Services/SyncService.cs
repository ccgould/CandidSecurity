using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidBritishAirways.Services
{
    public class SyncService
    {
        private readonly DatabaseService _localDb;
        //private readonly RemoteApiService _remoteApi;

        //public SyncService(DatabaseService localDb, RemoteApiService remoteApi)
        //{
        //    _localDb = localDb;
        //    _remoteApi = remoteApi;
        //}

        //public async Task SyncReportsAsync()
        //{
        //    if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        //    {
        //        var localReports = await _localDb.GetReportsAsync();
        //        foreach (var report in localReports)
        //        {
        //            await _remoteApi.SaveReportAsync(report);
        //        }

        //        var remoteReports = await _remoteApi.GetReportsAsync();
        //        foreach (var report in remoteReports)
        //        {
        //            await _localDb.SaveReportAsync(report);
        //        }
        //    }
        //}
    }
}
