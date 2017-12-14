//Fatema Shabbir 
//19201960

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleData
{
    class TMDLLWrapper
    {
        //  int GetTileSize(int *width, int *height);
        [DllImport("TrueMarble.dll")] public static extern int GetTileSize(out int width, out int height);
        //  int GetNumTiles(int zoomLevel, int *numTilesX, int *numTilesY);
        [DllImport("TrueMarble.dll")] public static extern int GetNumTiles(int zoomLevel, out int numTilesX, out int numTilesY);
        //  int GetTileImageAsRawJPG(int zoomLevel, int tileX, int tileY, unsigned char *imageBuf, int bufSize, int *jpgSize);
        [DllImport("TrueMarble.dll")] public static extern int GetTileImageAsRawJPG(int zoomLevel, int tileX, int tileY, [In][Out] byte[] imageBuf, int bufSize, out int jpgSize);
        //  int DllExport GetTileImageAsRawJPG_dbg(int zoomLevel, int tileX, int tileY, unsigned char *imageBuf, int bufSize, int *jpgSize, char **sFile, char **sErr);
        [DllImport("TrueMarble.dll")] public static extern int GetTileImageAsRawJPG_dbg(int zoomLevel, int tileX, int tileY, out byte[] imageBuf, int bufSize, out int jpgSize, out string sFile, out string sErr);
    
}
}
