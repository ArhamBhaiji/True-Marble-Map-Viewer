using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TrueMarbleData;

namespace TrueMarbleBiz
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class TMBizControllerImpl : ITMBizController
    {
        ITMDataController m_tmData;
        BrowseHistory browseH;

        public TMBizControllerImpl()
        {
            //Biz server connects to server
            ChannelFactory<ITMDataController> tmData;
            NetTcpBinding tcpBinding = new NetTcpBinding();
            //sets max values for tcp binding
            tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;
            string sURL = "net.tcp://localhost:50001/TMData";
            try
            {
                tmData = new ChannelFactory<ITMDataController>(tcpBinding, sURL);
                m_tmData = tmData.CreateChannel();
            }
            catch (FaultException)
            {
                Console.WriteLine("Fault Exception thrown in MainWindow.xaml.cs > Window_Loaded");
            }

            browseH = new BrowseHistory();

            Console.WriteLine("Biz Server Started");
            
        }

        //Gets the width
        public int GetTileWidth()
        {
            return m_tmData.GetTileWidth();
        }

        //gets the height
        public int GetTileHeight()
        {
            return m_tmData.GetTileHeight();
        }

        //gets the max horizontal tiles
        public int GetNumTilesAcross(int zoom)
        {
            return m_tmData.GetNumTilesAcross(zoom);
        }

        //gets max vertical tiles
        public int GetNumTilesDown(int zoom)
        {
            return m_tmData.GetNumTilesDown(zoom);
        }

        //loads the tiles
        public byte[] LoadTile(int zoom, int x, int y)
        {
            return m_tmData.LoadTile(zoom, x, y);
        }

        // verifies  that each of the tiles actually exists and is loadable, with the tile range defined by the DLL’s GetNumTilesAcross/Down
        public bool VerifyTiles()
        {
            bool check = true;

            //set inital zoom level
            int zoom = 0;
            int x, y, maxX, maxY;
            byte[] image;
            //iterate through all the zoom levels
            for (zoom = 0;  zoom < 7; zoom++)
            {
                //set the max x and y for the given zoom level
                maxX = m_tmData.GetNumTilesAcross(zoom);
                maxY = m_tmData.GetNumTilesDown(zoom);
                Console.WriteLine("MaxX: " + maxX);

                //iterate through all the x values
                for (x = 0; x <= maxX; x++)
                {
                    //iterate through all the y values
                    for (y = 0; y <= maxY; y++)
                    {
                        image = null;
                        //call the load tile method to check if the tile loads
                        image  = m_tmData.LoadTile(zoom, x, y);

                        //checks if the image byte array has an image in it
                        if (image != null && image.Length > 0)
                        {
                            check = true;
                        }
                        else
                        {
                            check = false;
                        }
                    }
                }
            }

            return check;
        }

        //delegate for async call
        private delegate bool VerifyDelg();
        
        //Async call method
        public void VerifyTilesAsync()
        {
            VerifyDelg verifyDelg;
            verifyDelg = this.VerifyTiles;

            AsyncCallback cbDelg;
            cbDelg = this.VerifyTiles_OnComplete;

            ITMBizControllerCallback cb = OperationContext.Current.GetCallbackChannel<ITMBizControllerCallback>();
            
            verifyDelg.BeginInvoke(cbDelg, cb);
        }

        //Method for completion callback
        private void VerifyTiles_OnComplete(IAsyncResult res)
        {
            bool result;
            VerifyDelg verifyDelg;
            AsyncResult asyncObj = (AsyncResult) res;

            //makes sure end invoke isn't called more than once
            if (asyncObj.EndInvokeCalled == false)
            {
                verifyDelg = (VerifyDelg) asyncObj.AsyncDelegate;
                //gets the result of the call
                result = verifyDelg.EndInvoke(asyncObj);
                
                //prints result to console in biz server
                if (result == true)
                {
                    Console.WriteLine("Success");
                }
                else if (result == false)
                {
                    Console.WriteLine("Failure");
                }

                //to do the remote callback for the client
                ITMBizControllerCallback cb = (ITMBizControllerCallback)asyncObj.AsyncState;
                cb.OnVerificationComplete(result);
            }
            asyncObj.AsyncWaitHandle.Close();
        }

        public void AddHistEntry(HistEntry h)
        {
            //browseH.CurrEntryIndex = browseH.CurrEntryIndex + 1;
            browseH.CurrEntryIndex++;
            browseH.History.Insert(browseH.CurrEntryIndex, h);
            Console.WriteLine("AddHist: " + browseH.CurrEntryIndex);
            Console.WriteLine("AddHist Count: " + browseH.History.Count);
            while (browseH.History.Count > browseH.CurrEntryIndex +1)
              browseH.History.RemoveAt(browseH.History.Count - 1);
        }

        public HistEntry GetCurrHistEntry()
        {
            int index = browseH.CurrEntryIndex;
            HistEntry h = browseH.History[index];
            return h;
        }

        public HistEntry HistBack()
        {
            if (browseH.CurrEntryIndex > 0)
            {
                browseH.CurrEntryIndex = browseH.CurrEntryIndex - 1;
                HistEntry h = browseH.History[browseH.CurrEntryIndex];
                Console.WriteLine("Back: " + browseH.CurrEntryIndex);
                Console.WriteLine("Back Count: " + browseH.History.Count);
                return h;
            }
            else
            {
                return null;
            }
        }

        public HistEntry HistForward()
        {
            if (browseH.CurrEntryIndex < browseH.History.Count -1)
            {
                browseH.CurrEntryIndex = browseH.CurrEntryIndex + 1;
                HistEntry h = browseH.History[browseH.CurrEntryIndex];
                Console.WriteLine("Forward: " + browseH.CurrEntryIndex);
                Console.WriteLine("Forward Count: " + browseH.History.Count);
                return h;
            }
            else
            {
                return null;
            }
        }

        public BrowseHistory BrowseHistory()
        {
            return browseH;
        }
        
        public BrowseHistory GetFullHistory()
        {
            return browseH;
        }

        void ITMBizController.SetFullHistory(BrowseHistory hist)
        {
            browseH.History = hist.History;
            browseH.CurrEntryIndex = hist.CurrEntryIndex;
        }
    }
}
