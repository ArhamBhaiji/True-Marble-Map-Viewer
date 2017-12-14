using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    [DataContract]
    public class HistEntry
    {
        [DataMember]
        public int x { get; set; }
        [DataMember]
        public int y { get; set; }
        [DataMember]
        public int zoom { get; set; }
        public HistEntry (int X, int Y, int Zoom)
        {
            x = X;
            y = Y;
            zoom = Zoom;
        }

        public static implicit operator List<object>(HistEntry v)
        {
            throw new NotImplementedException();
        }
    }

    [DataContract]
    public class BrowseHistory
    {
        [DataMember]
        public List<HistEntry> History { get; set; }
        [DataMember]
        public int CurrEntryIndex { get; set; }
        public BrowseHistory()
        {
            History = new List<HistEntry>();
            CurrEntryIndex = -1;
        }
        
    }
}
