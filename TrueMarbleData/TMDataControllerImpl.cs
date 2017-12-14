//Fatema Shabbir 
//19201960

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.CompilerServices;

namespace TrueMarbleData
{
    //sets the service behaviour of the server
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class TMDataControllerImpl : ITMDataController
    {
        //Constructor
        public TMDataControllerImpl() {
            Console.WriteLine("Client Connected!");
        }

        //Destructor
        ~TMDataControllerImpl()
        {
            Console.WriteLine("Client Disconnected!");
        }

        //Gets the width
        public int GetTileWidth()
        {
            int h, w;
            TMDLLWrapper.GetTileSize(out w, out h);
            return w;
        }

        //gets the height
        public int GetTileHeight()
        {
            int h, w;
            TMDLLWrapper.GetTileSize(out w, out h);
            return h;
        }

        //gets the max horizontal tiles
        public int GetNumTilesAcross(int zoom)
        {
            int x, y;
            TMDLLWrapper.GetNumTiles(zoom, out x, out y);
            Console.WriteLine("Max valX for zoom {0} is {1}", zoom, x);
            return x;
        }

        //gets max vertical tiles
        public int GetNumTilesDown(int zoom)
        {
            int x, y;
            TMDLLWrapper.GetNumTiles(zoom, out x, out y);
            Console.WriteLine("Max valY for zoom {0} is {1}", zoom, y);
            return y;
        }

        //loads the tiles
        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] LoadTile(int zoom, int x, int y)
        {
            int h, w, jpgSize;
            byte[] imageBuf;

            //gets the size of the tile
            TMDLLWrapper.GetTileSize(out w, out h);
            int buffsize = w * h * 3;
            imageBuf = new byte[buffsize];
            
            //gets the raw jpg file
            TMDLLWrapper.GetTileImageAsRawJPG(zoom, x, y, imageBuf, buffsize, out jpgSize);
            return imageBuf;
        }
    }
}
