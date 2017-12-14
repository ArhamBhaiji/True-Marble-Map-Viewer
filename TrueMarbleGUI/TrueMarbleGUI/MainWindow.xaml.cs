//Fatema Shabbir
//1921960

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.IO;
using TrueMarbleBiz;

namespace TrueMarbleGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class MainWindow : ITMBizControllerCallback
    {
        ITMBizController m_tmBiz;
        int x = 0, y = 0, zoom = 4;
        bool sliderCall = true;

        public MainWindow()
        {
            InitializeComponent();
            lblZoom.Content = zoom;
        }

        //loads the image tile
        private void load_Image(bool addToHist)
        {
            byte[] bitmap;
            try
            {
                //calls load tile method from server
                bitmap = m_tmBiz.LoadTile(zoom, x, y);
                //open new memroy stream to convert to jpeg
                MemoryStream memStream = new MemoryStream(bitmap);
                //decodes memroy stream to a jpeg image
                JpegBitmapDecoder decoder = new JpegBitmapDecoder(memStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                BitmapFrame bitmapFrame = decoder.Frames[0];
                //sets the image source to the frame
                imgTile.Source = bitmapFrame;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                Console.WriteLine("Server is not on!");
            }
            catch (System.ServiceModel.CommunicationObjectFaultedException)
            {
                Console.WriteLine("Exception caught");
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2);
            }

            if (addToHist)
            {
                //adds for browse history
                HistEntry hAdd = new HistEntry(x, y, zoom);
                try
                {
                    m_tmBiz.AddHistEntry(hAdd);
                }
                catch (Exception) { }
                
            }
        }

        //Goes down on map
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (y > 0)
            {
                y--;
                load_Image(true);
            }
        }

        //Goes right on map
        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            //gets the max tiles we can go horizontally
            int maxX = m_tmBiz.GetNumTilesAcross(zoom);
            if (x < maxX - 1)
            {
                x++;
                load_Image(true);
            }  
        }

        //Goes up on map
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            //gets the max tiles we can go vertically
            int maxY = m_tmBiz.GetNumTilesDown(zoom);
            if (y < maxY - 1)
            {
                y++;
                load_Image(true);
            }
        }

        //Goes left on map
        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (x > 0)
            {
                x--;
                load_Image(true);
            }
        }

        //sets up window on load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Client side connects to server
            ChannelFactory<ITMBizController> tmBiz;
            NetTcpBinding tcpBinding = new NetTcpBinding();
            //sets max values for tcp binding
            tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;
            string sURL = "net.tcp://localhost:50002/TMBiz";
            try 
            {
                tmBiz = new DuplexChannelFactory<ITMBizController>(this, tcpBinding, sURL);
                m_tmBiz = tmBiz.CreateChannel();
            }
            catch (FaultException)
            {
                Console.WriteLine("Fault Exception thrown in MainWindow.xaml.cs > Window_Loaded");
            }

            //makes the asynchronous call
            try
            {
                m_tmBiz.VerifyTilesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            load_Image(true);

        }

        //goes next in the history
        private void btn_Forward_Click(object sender, RoutedEventArgs e)
        {
            HistEntry h;
            h = m_tmBiz.HistForward();
            if (h != null)
            {
                x = h.x;
                y = h.y;
                zoom = h.zoom;
                lblZoom.Content = zoom;
                sliderCall = false;

                load_Image(false);
            }            
        }

        //goes back in the history
        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            HistEntry h;
            h = m_tmBiz.HistBack();
            if (h != null)
            {
                x = h.x;
                y = h.y;
                zoom = h.zoom;
                lblZoom.Content = zoom;
                sliderCall = false;
                
                load_Image(false);
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            BrowseHistory h = m_tmBiz.GetFullHistory();
            FileStream fs = new FileStream("C:/History.xml", FileMode.Create, FileAccess.Write);

            DataContractSerializer serializer = new DataContractSerializer(typeof(BrowseHistory));
            serializer.WriteObject(fs, h);

            fs.Close();
        }

        private void btn_Load_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream("C:/History.xml", FileMode.Open, FileAccess.Read);

            DataContractSerializer serializer = new DataContractSerializer(typeof(BrowseHistory));
            BrowseHistory h = new BrowseHistory();
            h = (BrowseHistory)serializer.ReadObject(fs);

            m_tmBiz.SetFullHistory(h);

            int index = h.CurrEntryIndex;

            zoom = h.History[index].zoom;
            x = h.History[index].x;
            y = h.History[index].y;
            lblZoom.Content = zoom;

            load_Image(false);
            
            fs.Close();

        }

        private void btn_View_Click(object sender, RoutedEventArgs e)
        {
            BrowseHistory bh = m_tmBiz.GetFullHistory();
            DisplayHistory displayHist = new DisplayHistory(bh);
            displayHist.ShowDialog();
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (zoom > 0)
            {
                zoom--;
                lblZoom.Content = zoom;
                x = 0;
                y = 0;
                load_Image(true);
            }
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (zoom < 6)
            {
                zoom++;
                lblZoom.Content = zoom;
                x = 0;
                y = 0;
                load_Image(true);
            }
        }
        
        void ITMBizControllerCallback.OnVerificationComplete(bool check)
        {
            if (check == true)
            {
                MessageBox.Show("All tiles are verified!");
            }
            else if (check == false)
            {
                MessageBox.Show("Failure to verify all tiles!");
            }
        }
    }
}
