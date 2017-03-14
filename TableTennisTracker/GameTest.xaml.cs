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
        public int tableLevel;
        public int netLocation;
        public bool startPosition;
        public DateTime startPosTime;
        public int _p1Score;
        public int _p2Score;
        public bool gameOver;

        public GameTest()
        {
            this.tableLevel = 515;      // Must be determined per setup
            this.netLocation = 960;     // Must be determined per setup

            InitVariables();
            NewGame();

            this.kinectSensor = KinectSensor.GetDefault();

            // Using MultiSourceFrame
            this.multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color);
            this.multiSourceFrameReader.MultiSourceFrameArrived += this.Frame_Arrived;
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            this.kinectSensor.Open();

            this.DataContext = this;

            InitializeComponent();
        }

        // Initialize variables
        private void InitVariables()
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
            this.startPosition = false;
            this.startPosTime = DateTime.MinValue;
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

        // Player 1 score
        public int P1Score
        {
            get { return _p1Score; }
            set
            {
                if (_p1Score != value)
                {
                    _p1Score = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("P1Score"));
                    }
                }
            }
        }

        // Player 2 score
        public int P2Score
        {
            get { return _p2Score; }
            set
            {
                if (_p2Score != value)
                {
                    _p2Score = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("P2Score"));
                    }
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

        // Find ball xy coordinates
        private DataPoint FindBall(MultiSourceFrame multiSourceFrame)
        {
            DataPoint BallLocation = new DataPoint(0, 0, 0, 0);
            List<int> xvals = new List<int>();
            List<int> yvals = new List<int>();

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
                        //int xsum = 0;
                        //int ysum = 0;
                        int vcount = 0;
                        for (int i = 0; i < colorFrameDescription.Width * colorFrameDescription.Height; i++)
                        {
                            int j = i * 4;
                            if (myBytes[j] < 140 && myBytes[j] > 100 && myBytes[j + 1] < 80 && myBytes[j + 1] > 40 && myBytes[j + 2] < 180 && myBytes[j + 2] > 140)
                            {
                                int yval = i / 1920;
                                int xval = i - yval * 1920;
                                xvals.Add(xval);
                                yvals.Add(yval);
                                //xsum += xval;
                                //ysum += yval;
                                vcount++;
                            }
                        }
                        //int xavg = 0;
                        //int yavg = 0;
                        //if (vcount > 9)
                        //{
                        //    xavg = xsum / vcount;
                        //    yavg = 1080 - (ysum / vcount);
                        //}
                        //else
                        //{
                        //    xavg = 1;
                        //    yavg = 1;
                        //}
                        if (vcount > 9)
                        {
                            int midindex = vcount / 2;
                            xvals.Sort();
                            BallLocation.X = xvals[midindex];
                            BallLocation.Y = 1080 - yvals[midindex];
                        } else
                        {
                            BallLocation.X = 1;
                            BallLocation.Y = 1;
                        }
                    }
                }
            }
            return (BallLocation);
        }

        // Get xyz physical coordinates for bounce location, add to Bounce list
        private void BounceLocation(DepthFrame depthFrame, int xavg, int yavg)
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

        // Color frame analysis
        private void Frame_Arrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            bool changeDir = false;

            if (!gameOver)
            {
                MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

                if (inVolley)
                {
                    // If too much time passes without bounce/return, someone missed
                    if (this.hitTime != DateTime.MinValue)
                    {
                        if ((float)DateTime.Now.Subtract(this.hitTime).TotalSeconds > 20000)
                        {
                            if (this.Direction == "Left")
                            {
                                Score("P2", "Time Limit");
                            }
                            else
                            {
                                Score("P1", "Time Limit");
                            }
                        }
                    }

                    // Get Ball xy coordinates
                    DataPoint BallLocation = FindBall(multiSourceFrame);
                    int xavg = (int)BallLocation.X;
                    int yavg = (int)BallLocation.Y;

                    // If good data point, analyze it
                    if (xavg > 1)
                    {
                        // Off (or rather under) table
                        if (bounce1 && yavg < tableLevel - 50)
                        {
                            if (this.Direction == "Left")
                            {
                                Score("P2", "Below Table");
                            }
                            else
                            {
                                Score("P1", "Below Table");
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
                                    changeDir = true;
                                }
                                this.Direction = "Left";
                            }
                            else if (xdelta < -10)
                            {
                                if (this.Direction == "Left")
                                {
                                    ChangeDirection();
                                    changeDir = true;
                                }
                                this.Direction = "Right";
                            }

                        // Vertical direction and bounce detection
                        if (ydelta > 5)
                        {
                            this.VertDir = "Down";
                        }
                        else if (ydelta < -5)
                        {
                            if (this.VertDir == "Down" && !changeDir)
                            {
                                // If a bounce, get xyz coords, handle bounce processesing
                                using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                                {
                                    BounceLocation(depthFrame, xavg, yavg);
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
                else   // Check for ball in start position to start volley
                {
                    // Get Ball xy coordinates
                    DataPoint BallLocation = FindBall(multiSourceFrame);
                    int xavg = (int)BallLocation.X;
                    int yavg = (int)BallLocation.Y;

                    if (Math.Abs(yavg - this.tableLevel) < 30 && (xavg > netLocation + 300 || xavg < netLocation - 300))
                    {
                        if (!startPosition)
                        {
                            this.startPosTime = DateTime.Now;
                            this.startPosition = true;
                        }
                        else if ((float)(DateTime.Now.Subtract(this.startPosTime).TotalSeconds) > 1.5)
                        {
                            this.PointScored = "Starting Volley";
                            StartVolley();
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

        // Start new volley (overloaded for now - button push or automatic detection calls)
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
            this.VertDir = "";
            this.Direction = "";
        }
        public void StartVolley()
        {
            this.PointScored = "Starting Volley";
            this.AllData.Clear();
            this.Bounces.Clear();
            this.xyData.Clear();
            this.timeStarted = DateTime.Now;
            this.serveBounce = false;
            this.bounce1 = false;
            this.hitTime = DateTime.MinValue;
            this.inVolley = true;
        }

        // Start New Game
        private void NewGame()
        {
            P1Score = 0;
            P2Score = 0;
            gameOver = false;
        }

        // Register point scored
        private void Score(string player, string message)
        {
            this.PointScored = "Point Scored by " + player + "!  --  " + message;
            this.hitTime = DateTime.MinValue;
            this.inVolley = false;

            if (player == "P1")
            {
                this.P1Score++;
            }
            else
            {
                this.P2Score++;
            }

            if (P1Score == 21)
            {
                this.PointScored = "Player 1 Wins!";
                gameOver = true;
            }
            else if (P1Score == 21)
            {
                this.PointScored = "Player 2 Wins!";
                gameOver = true;
            }

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
                        Score("P1", "No Bounce");
                    }
                    else
                    {
                        Score("P2", "No Bounce");
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
                if (this.bounce1)  // Second bounce - someone scored
                {
                    if (this.Direction == "Left")
                    {
                        Score("P2", "Two Bounces");
                    }
                    else
                    {
                        Score("P1", "Two Bounces");
                    }
                }
                else if (this.Direction == "Right" && hit.X < netLocation)   // Wrong side of net
                {
                    Score("P2", "Hit Net");
                }
                else if (this.Direction == "Left" && hit.X > netLocation)    // Wrong side of net
                {
                    Score("P1", "Hit Net");
                }
                else     // Legal bounce
                {
                    this.bounce1 = true;
                    this.hitTime = DateTime.Now;
                    this.PointScored = "Bounce 1";
                }
            }
            else  // Check for valid serve bounce
            {
                if ((this.Direction == "Right" && hit.X < netLocation) || (this.Direction == "Left" && hit.X > netLocation))
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
