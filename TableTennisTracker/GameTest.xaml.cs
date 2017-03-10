using System;
using System.Collections.Generic;
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
using System.IO;
using Microsoft.Kinect;
using System.ComponentModel;

namespace TableTennisTracker
{
    public class DataPoint
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Time { get; set; }

        public DataPoint(float x, float y, float z, float t)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Time = t;
        }
    }

        /// <summary>
        /// Interaction logic for GameTest.xaml
        /// </summary>
        public partial class GameTest : Page, INotifyPropertyChanged
    {
        private KinectSensor kinectSensor = null;
        private MultiSourceFrameReader multiSourceFrameReader = null;
        private CoordinateMapper coordinateMapper = null;
        public List<KeyValuePair<float, float>> _xyData;
        public DateTime timeStarted;
        public List<DataPoint> AllData;
        public List<DataPoint> Bounces;
        public string _direction;
        public string _vertdir;
        public string _pointScored;
        // Volley tracking variables
        public bool bounce1;
        public bool serveBounce;
        public bool inVolley;
        public DateTime hitTime;

        public GameTest()
        {
            this._xyData = new List<KeyValuePair<float, float>>();
            this.AllData = new List<DataPoint>();
            this.Bounces = new List<DataPoint>();
            this._direction = "";
            this._vertdir = "";
            this._pointScored = "";
            this.bounce1 = false;
            this.serveBounce = false;
            this.inVolley = false;
            this.hitTime = DateTime.MinValue;

            this.kinectSensor = KinectSensor.GetDefault();

            // Using MultiSourceFrame
            this.multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color);
            this.multiSourceFrameReader.MultiSourceFrameArrived += this.Frame_Arrived;
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            this.kinectSensor.Open();

            this.DataContext = this;

            InitializeComponent();
        }

        // Data points to be graphed
        public List<KeyValuePair<float, float>> xyData
        {
            get { return _xyData; }
            set
            {
                _xyData = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("xyData"));
                }
            }
        }

        // Ball direction
        public string Direction
        {
            get { return _direction; }
            set
            {
                if (_direction != value)
                {
                    _direction = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("Direction"));
                    }
                }
            }
        }

        // Ball vertical direction
        public string VertDir
        {
            get { return _vertdir; }
            set
            {
                if (_vertdir != value)
                {
                    _vertdir = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("VertDir"));
                    }
                }
            }
        }

        // Status output string
        public string PointScored
        {
            get { return _pointScored; }
            set
            {
                if (_pointScored != value)
                {
                    _pointScored = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("PointScored"));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Color frame analysis
        private void Frame_Arrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if (inVolley)
            {
                // If too much time passes without bounce/return, someone missed
                if (this.hitTime != DateTime.MinValue)
                {
                    if ((float)DateTime.Now.Subtract(this.hitTime).TotalSeconds > 20000)
                    {
                        if (this.Direction == "Left")
                        {
                            Score("P2");
                        }
                        else
                        {
                            Score("P1");
                        }
                    }
                }

                MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

                using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
                {
                    if (colorFrame != null)
                    {
                        FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                        using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                        {
                            byte[] myBytes = new byte[colorFrameDescription.Width * colorFrameDescription.Height * 4];
                            colorFrame.CopyConvertedFrameDataToArray(myBytes, ColorImageFormat.Bgra);

                            // Find location of the ball based on pixel colors
                            int xsum = 0;
                            int ysum = 0;
                            int vcount = 0;
                            for (int i = 0; i < colorFrameDescription.Width * colorFrameDescription.Height; i++)
                            {
                                int j = i * 4;
                                if (myBytes[j] < 140 && myBytes[j] > 100 && myBytes[j + 1] < 80 && myBytes[j + 1] > 40 && myBytes[j + 2] < 180 && myBytes[j + 2] > 140)
                                {
                                    int yval = i / 1920;
                                    int xval = i - yval * 1920;
                                    xsum += xval;
                                    ysum += yval;
                                    vcount++;
                                }
                            }
                            int xavg = 0;
                            int yavg = 0;
                            if (vcount > 0)
                            {
                                xavg = xsum / vcount;
                                yavg = 1080 - (ysum / vcount);
                            }
                            else
                            {
                                xavg = 1;
                                yavg = 1;
                            }

                            // If good data point, analyze it
                            if (xavg > 1)
                            {
                                // Off (or rather under) table
                                if (bounce1 && yavg < 500)
                                {
                                    if (this.Direction == "Left")
                                    {
                                        Score("P2");
                                    }
                                    else
                                    {
                                        Score("P1");
                                    }
                                }

                                // Determine direction and do game processing
                                float xdelta = 0;
                                float ydelta = 0;
                                if (AllData.Count > 0)
                                {
                                    xdelta = AllData[AllData.Count - 1].X - xavg;
                                    ydelta = AllData[AllData.Count - 1].Y - yavg;
                                }

                                // Horizontal direction determination and direction change detection
                                if (xdelta > 10)
                                {
                                    if (this.Direction == "Right")
                                    {
                                        ChangeDirection();
                                    }
                                    this.Direction = "Left";
                                }
                                else if (xdelta < -10)
                                {
                                    if (this.Direction == "Left")
                                    {
                                        ChangeDirection();
                                    }
                                    this.Direction = "Right";
                                }

                                // Vertical direction and bounce detection
                                if (ydelta > 5)
                                {
                                    this.VertDir = "Down";
                                }
                                else if (ydelta < 5)
                                {
                                    if (this.VertDir == "Down" && yavg < 600)
                                    {
                                        // If a bounce get x,y,z coords, handle bounce processesing
                                        using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                                        {
                                            if (depthFrame == null)
                                            {
                                                return;
                                            }

                                            using (KinectBuffer depthFrameData = depthFrame.LockImageBuffer())
                                            {
                                                CameraSpacePoint[] camSpacePoints = new CameraSpacePoint[1920 * 1080];
                                                this.coordinateMapper.MapColorFrameToCameraSpaceUsingIntPtr(depthFrameData.UnderlyingBuffer, depthFrameData.Size, camSpacePoints);
                                                int index = (1080 - yavg) * 1920 + xavg;
                                                float xtval = camSpacePoints[index].X;
                                                float ytval = camSpacePoints[index].Y;
                                                float ztval = camSpacePoints[index].Z;

                                                if (ztval < 0)
                                                {
                                                    for (int a = -5; a <= 5; a++)
                                                    {
                                                        for (int b = -5; b <= 5; b++)
                                                        {
                                                            if (camSpacePoints[index + (a * 1920) + b].Z > 0)
                                                            {
                                                                xtval = camSpacePoints[index + (a * 1920) + b].X;
                                                                ytval = camSpacePoints[index + (a * 1920) + b].Y;
                                                                ztval = camSpacePoints[index + (a * 1920) + b].Z;
                                                            }
                                                        }
                                                    }
                                                }
                                                this.Bounces.Add(new DataPoint(xtval, ytval, ztval, (float)(DateTime.Now.Subtract(this.timeStarted).TotalSeconds)));
                                            }
                                        }
                                        // Handle bounce processing
                                        Bounce(new DataPoint(xavg, yavg, 0, (float)(DateTime.Now.Subtract(this.timeStarted).TotalSeconds)));
                                    }
                                    this.VertDir = "Up";
                                }
                                // Add current location to points list
                                this.AllData.Add(new DataPoint(xavg, yavg, 0, (float)(DateTime.Now.Subtract(this.timeStarted).TotalSeconds)));
                            }
                        }
                    }
                }
            }
        }

        // Update data to plot, write xy and bounce data to text files
        public void PlotIt(object sender, RoutedEventArgs e)
        {
            PlotXYData();

            string[] dataOut = new string[this.AllData.Count + 1];
            dataOut[0] = "Time, X, Y, Z";
            int i = 1;
            foreach (DataPoint item in this.AllData)
            {
                dataOut[i] = item.Time.ToString() + ", " + item.X.ToString() + ", " + item.Y.ToString() + ", " + item.Z.ToString();
                i++;
            }
            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path2 = System.IO.Path.Combine(myPhotos, "TestData" + ".txt");
            File.WriteAllLines(path2, dataOut);

            string[] bounceOut = new string[this.Bounces.Count + 1];
            bounceOut[0] = "Time, X, Y, Z";
            i = 1;
            foreach (DataPoint item in this.Bounces)
            {
                bounceOut[i] = item.Time.ToString() + ", " + item.X.ToString() + ", " + item.Y.ToString() + ", " + item.Z.ToString();
                i++;
            }
            string path3 = System.IO.Path.Combine(myPhotos, "BounceData" + ".txt");
            File.WriteAllLines(path3, bounceOut);
        }

        // Create xyData list from AllData, send to XAML plot
        public void PlotXYData()
        {
            int i = 0;
            foreach (DataPoint item in this.AllData)
            {
                this.xyData.Add(new KeyValuePair<float, float>(item.X, item.Y));
                i++;
            }
            chart1.DataContext = null;
            chart1.DataContext = this.xyData;
        }

        // Start new volley
        public void StartVolley(object sender, RoutedEventArgs e)
        {
            this.PointScored = "";
            this.AllData.Clear();
            this.Bounces.Clear();
            this.xyData.Clear();
            this.timeStarted = DateTime.Now;
            this.serveBounce = false;
            this.bounce1 = false;
            this.hitTime = DateTime.MinValue;
            this.inVolley = true;
        }

        // Register point scored
        private void Score(string player)
        {
            this.PointScored = "Point Scored by " + player + "!";
            this.hitTime = DateTime.MinValue;
            this.inVolley = false;

            PlotXYData();
        }

        // Handle change in ball direction
        private void ChangeDirection()
        {
            if (serveBounce)
            {
                if (!bounce1)
                {
                    if (this.Direction == "Left")
                    {
                        Score("P1");
                    }
                    else
                    {
                        Score("P2");
                    }
                    this.hitTime = DateTime.MinValue;
                }
                else
                {
                    this.bounce1 = false;
                    this.hitTime = DateTime.Now;
                    this.PointScored = "Direction Change";
                }
            }
        }

        // Handle bounce
        private void Bounce(DataPoint hit)
        {
            if (this.serveBounce)
            {
                if (this.bounce1)
                {
                    if (this.Direction == "Left")
                    {
                        Score("P2");
                    }
                    else
                    {
                        Score("P1");
                    }
                }
                else if (this.Direction == "Right" && hit.X < 960)
                {
                    Score("P2");
                }
                else if (this.Direction == "Left" && hit.X > 960)
                {
                    Score("P1");
                }
                else
                {
                    this.bounce1 = true;
                    this.hitTime = DateTime.Now;
                    this.PointScored = "Bounce 1";
                }
            }
            else
            {
                if ((this.Direction == "Right" && hit.X < 960) || (this.Direction == "Left" && hit.X > 960))
                {
                    this.serveBounce = true;
                    this.PointScored = "Serve Bounce";
                }
                else
                {
                    BadServe();
                }
            }
        }

        // Handle bad serve
        private void BadServe()
        {
            this.serveBounce = false;
            this.PointScored = "Bad Serve!";
            this.inVolley = false;
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.multiSourceFrameReader != null)
            {
                // MultiSourceFrameReder is IDisposable
                this.multiSourceFrameReader.Dispose();
                this.multiSourceFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
    }
}
