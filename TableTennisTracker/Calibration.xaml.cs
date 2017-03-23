using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for Calibration.xaml
    /// </summary>
    public partial class Calibration : Page, INotifyPropertyChanged
    {
        private KinectSensor kinectSensor = null;
        private MultiSourceFrameReader multiSourceFrameReader = null;
        private CoordinateMapper coordinateMapper = null;
        private WriteableBitmap colorBitmap = null;
        private bool calSignal = false;
        private string _message;

        public Calibration()
        {
            this.Message = "Not Calibrated";

            this.kinectSensor = KinectSensor.GetDefault();

            // Using MultiSourceFrame
            this.multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color);
            this.multiSourceFrameReader.MultiSourceFrameArrived += this.Frame_Arrived;
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            this.kinectSensor.Open();

            this.DataContext = this;

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource ImageSource
        {
            get
            {
                return this.colorBitmap;
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Message"));
                    }
                }
            }
        }

        // Show camera video
        public void Frame_Arrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            // ColorFrame is IDisposable
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

            using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                        {
                            if (depthFrame == null)
                            {
                                return;
                            }
                            using (KinectBuffer depthFrameData = depthFrame.LockImageBuffer())
                            {
                                byte[] depthBytes = new byte[depthFrameData.Size];
                                CameraSpacePoint[] camSpacePoints = new CameraSpacePoint[1920 * 1080];
                                this.coordinateMapper.MapColorFrameToCameraSpaceUsingIntPtr(depthFrameData.UnderlyingBuffer, depthFrameData.Size, camSpacePoints);

                                if (calSignal)
                                {
                                    FindTable(camSpacePoints);
                                }

                                // verify data and write the new color frame data to the display bitmap
                                if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                                {
                                    byte[] myBytes = new byte[colorFrameDescription.Width * colorFrameDescription.Height * 4];
                                    colorFrame.CopyConvertedFrameDataToArray(myBytes, (ColorImageFormat)3);
                                    this.colorBitmap.WritePixels(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight), myBytes, 7680, 0);
                                }
                            }
                            this.colorBitmap.Unlock();
                        }
                    }
                }
            }
        }

        // Get table level in pixels in specified x range
        public int TableLevel(CameraSpacePoint[] camSpacePoints, int xstart, int xstop)
        {
            int topRow = 360;
            List<int> tablePts = new List<int>();
            for (int i = 720; i > 360; i--)
            {
                int rowPixelCount = 0;
                for (int j = xstart; j < xstop; j++)
                {
                    int index = i * 1920 + j;
                    if (camSpacePoints[index].Z < 2.0 && camSpacePoints[index].Z > 0)
                    {
                        rowPixelCount++;
                        tablePts.Add(i);
                    }
                }
                if (rowPixelCount > 100)
                {
                    topRow = i;
                }
            }

            int midIndex = tablePts.Count / 2;
            //return tablePts[midIndex];
            return topRow;
        }

        // Find net x coord in pixels
        public int FindNet(CameraSpacePoint[] camSpacePoints)
        {
            List<int> netPts = new List<int>();
            List<float> Zvals = new List<float>();
            for (int i = 360; i < 1080 - GlobalClass.tableHeight - 40; i++)
            {
                for (int j = 640; j < 1280; j++)
                {
                    int index = i * 1920 + j;
                    if (camSpacePoints[index].Z < 2.0 && camSpacePoints[index].Z > 0)
                    {
                        netPts.Add(j);
                        Zvals.Add(camSpacePoints[index].Z);
                    }
                }
            }
            Zvals.Sort();
            netPts.Sort();
            int midIndex = netPts.Count / 2;
            GlobalClass.minZ = Zvals[midIndex];
            return netPts[midIndex];
        }

        // Find table level in pixels
        public void FindTable(CameraSpacePoint[] camSpacePoints)
        {
            int leftSide = TableLevel(camSpacePoints, 0, 640);
            int rightSide = TableLevel(camSpacePoints, 1280, 1919);
            GlobalClass.tableHeight = 1080 - (leftSide + rightSide) / 2;

            GlobalClass.netLocation = FindNet(camSpacePoints);

            this.Message = "Left: " + leftSide + "   Right: " + rightSide + "    Net: " + GlobalClass.netLocation;
            this.calSignal = false;
        }

        // Button click signals to do table calibration
        private void Calibration_Click(object sender, RoutedEventArgs e)
        {
            this.calSignal = true;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            CalibrationWindow_Closing();
            NavigationService.Navigate(new Splash());
        }

        // Kinect clean up stuff when closing program
        private void CalibrationWindow_Closing()
        {
            if (this.multiSourceFrameReader != null)
            {
                // ColorFrameReder is IDisposable
                this.multiSourceFrameReader.Dispose();
                this.multiSourceFrameReader = null;
            }

            //if (this.kinectSensor != null)
            //{
            //    this.kinectSensor.Close();
            //    this.kinectSensor = null;
            //}
        }
    }
}
