using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    [ServiceContract(CallbackContract = typeof(ITMBizControllerCallback))]
    public interface ITMBizController
    {
        [OperationContract]
        int GetTileWidth();
        [OperationContract]
        int GetTileHeight();
        [OperationContract]
        int GetNumTilesAcross(int zoom);
        [OperationContract]
        int GetNumTilesDown(int zoom);
        [OperationContract]
        byte[] LoadTile(int zoom, int x, int y);
        //methods for Async Call
        [OperationContract]
        bool VerifyTiles();
        [OperationContract]
        void VerifyTilesAsync();
        //Methods for Browse History
        [OperationContract]
        void AddHistEntry(HistEntry h);
        [OperationContract]
        HistEntry GetCurrHistEntry();
        [OperationContract]
        HistEntry HistBack();
        [OperationContract]
        HistEntry HistForward();
        [OperationContract]
        BrowseHistory BrowseHistory();
        [OperationContract]
        BrowseHistory GetFullHistory();
        [OperationContract]
        void SetFullHistory(BrowseHistory hist);
    }
}
