using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
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
using TableTennisTracker.Models;
using TableTennisTracker.Services;

namespace TableTennisTracker
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Player PlayerOne = null;
        Player PlayerTwo = null;
        Game CurrentGame = null;
        int _playerOneScore;
        int _playerTwoScore;
        SoundPlayer PointScored = new SoundPlayer(@"../../Sounds/PointScored.wav");
        SoundPlayer BallSet = new SoundPlayer(@"../../Sounds/BallSet.wav");
        SoundPlayer BadServe = new SoundPlayer(@"../../Sounds/BadServe.wav");
        SoundPlayer GameWin = new SoundPlayer(@"../../Sounds/GameWin.wav");
        GameService gs = new GameService();

        private KinectSensor kinectSensor = null;
        private MultiSourceFrameReader multiSourceFrameReader = null;
        private CoordinateMapper coordinateMapper = null;
        public List<KeyValuePair<float, float>> _xyData;
        public DateTime timeStarted;
        public List<DataPoint> AllData;
        public List<HitLocation> Bounces;
        public string Direction;
        public string VertDir;
        // Volley tracking variables
        public bool bounce1;
        public bool serveBounce;
        public bool inVolley;
        public int VolleyHits;
        public int LongestVolleyHits;
        public DateTime VolleyStartTime;
        public float LongestVolleyTime;
        public DateTime hitTime;
        public int tableLevel;
        public int netLocation;
        public bool startPosition;
        public DataPoint startLocation;
        public DateTime startPosTime;
        public DateTime scoreDelay;
        public bool served;
        public bool gameOver;
        public bool PossibleBounce;
        public DataPoint tempBounce;
        public HitLocation tempBounceXYZ;
        public string _Server;
        public UInt16[] PreviousDepthFrame;

        // Constructor
        public GamePage(Player pOne, Player pTwo)
        {
            InitializeComponent();

            PlayerOne = new Player
            {
                Id = pOne.Id
            };
            PlayerTwo = new Player
            {
                Id = pTwo.Id
            };

            NewGame();
            this.DataContext = this;
            //Player One DataContext
            PlayerOneUserName.DataContext = pOne;
            PlayerOneName.DataContext = pOne;
            PlayerOneNation.DataContext = pOne;
            PlayerOnePPHand.DataContext = pOne;


            //Player Two DataContext
            PlayerTwoUserName.DataContext = pTwo;
            PlayerTwoName.DataContext = pTwo;
            PlayerTwoNation.DataContext = pTwo;
            PlayerTwoPPHand.DataContext = pTwo;

            this.PreviousDepthFrame = new UInt16[217088];
            InitVariables();

            this.kinectSensor = KinectSensor.GetDefault();

            // Using MultiSourceFrame
            this.multiSourceFrameReader = this.kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Color);
            this.multiSourceFrameReader.MultiSourceFrameArrived += this.Frame_Arrived;
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            this.kinectSensor.Open();
        }

        // Creates the current game object
        private void NewGame()
        {
            PlayerOneScore = 0;
            PlayerTwoScore = 0;
            LongestVolleyHits = 0;
            LongestVolleyTime = 0;
            gameOver = false;
            CurrentGame = new Game
            {
                Player1 = PlayerOne,
                Player2 = PlayerTwo,
                Player1Score = PlayerOneScore,
                Player2Score = PlayerTwoScore
            };
        }

        // Initialize variables
        private void InitVariables()
        {
            this.tableLevel = GlobalClass.tableHeight;
            this.netLocation = GlobalClass.netLocation;

            this._xyData = new List<KeyValuePair<float, float>>();
            this.AllData = new List<DataPoint>();
            this.Bounces = new List<HitLocation>();
            this.startLocation = new DataPoint(0, 0, 0, 0);
            this.Direction = "";
            this.VertDir = "";
            this.bounce1 = false;
            this.serveBounce = false;
            this.served = false;
            this.inVolley = false;
            this.hitTime = DateTime.MinValue;
            this.startPosition = false;
            this.startPosTime = DateTime.MinValue;
            this.PossibleBounce = false;
            this.scoreDelay = DateTime.MinValue;
            Server = "";
            VolleyHits = 0;
        }

        public string Server
        {
            get { return _Server;  }
            set
            {
                _Server = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("Server"));
                }
            }
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
                        int vcount = 0;
                        for (int i = 0; i < colorFrameDescription.Width * colorFrameDescription.Height; i++)
                        {
                            int j = i * 4;
                            //if (myBytes[j] < 140 && myBytes[j] > 100 && myBytes[j + 1] < 80 && myBytes[j + 1] > 40 && myBytes[j + 2] < 180 && myBytes[j + 2] > 120)
                            if (myBytes[j] < 140 && myBytes[j] > 80 && myBytes[j + 1] < 60 && myBytes[j + 1] > 40 && myBytes[j + 2] < 160 && myBytes[j + 2] > 110)
                            {
                                int yval = i / 1920;
                                int xval = i - yval * 1920;
                                xvals.Add(xval);
                                yvals.Add(yval);
                                vcount++;
                            }
                        }

                        if (vcount > 9)
                        {
                            int midindex = vcount / 2;
                            xvals.Sort();
                            BallLocation.X = xvals[midindex];
                            BallLocation.Y = 1080 - yvals[midindex];
                        }
                        else
                        {
                            BallLocation.X = 1;
                            BallLocation.Y = 1;
                        }
                    }
                }
            }
            return (BallLocation);
        }

        // Return median of a list of float values
        private float FindMedian(List<float> inList)
        {
            inList.Sort();
            int index = inList.Count / 2;
            return inList[index];
        }

        // Get xyz physical coordinates for bounce location, add to Bounce list - not very good right now
        private HitLocation BounceLocation(DepthFrame depthFrame, int xavg, int yavg)
        {
            HitLocation bounceLocn = new Models.HitLocation();
            if (depthFrame == null)
            {
                return (bounceLocn);
            }

            using (KinectBuffer depthFrameData = depthFrame.LockImageBuffer())
            {
                CameraSpacePoint[] camSpacePoints = new CameraSpacePoint[1920 * 1080];
                this.coordinateMapper.MapColorFrameToCameraSpaceUsingIntPtr(depthFrameData.UnderlyingBuffer, depthFrameData.Size, camSpacePoints);
                List<float> xvals = new List<float>();
                List<float> yvals = new List<float>();
                List<float> zvals = new List<float>();
                int Vcount = 0;

                // Find ball in camera space
                for (int i = -40; i < 40; i++)
                {
                    for (int j = -40; j < 40; j++)
                    {
                        if (yavg + i > tableLevel)
                        {
                            int tempIndex = (yavg + i) * 1920 + xavg + j;
                            int arrVal = 4 * tempIndex;
                            if (arrVal > 4 * 1920 * 1080 || arrVal < 0)
                            {
                                arrVal = 4;
                            }
                            if (camSpacePoints[tempIndex].Z > 1 && camSpacePoints[tempIndex].Z < 3.5)
                            {
                                xvals.Add(camSpacePoints[tempIndex].X);
                                yvals.Add(camSpacePoints[tempIndex].Y);
                                zvals.Add(camSpacePoints[tempIndex].Z);
                                Vcount++;
                            }
                        }
                    }
                }
                if (Vcount > 0)
                {
                    bounceLocn.X = FindMedian(xvals);
                    bounceLocn.Y = FindMedian(yvals);
                    bounceLocn.Z = FindMedian(zvals);
                } 
                else
                {
                    bounceLocn.X = 0;
                    bounceLocn.Y = 0;
                    bounceLocn.Z = 0;
                }
            return (bounceLocn);
            }
        }

        // Color frame analysis With ball action analysis
        private void Frame_Arrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            bool changeDir = false;
            bool pause = false;

            if (scoreDelay != DateTime.MinValue)
            {
                if ((float)DateTime.Now.Subtract(this.scoreDelay).TotalSeconds < 2)
                {
                    pause = true;
                }
                else
                {
                    scoreDelay = DateTime.MinValue;
                }
            }

            if (!gameOver && !pause)
            {
                MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

                if (inVolley)
                {
                    // If too much time passes without bounce/return, someone missed
                    if (this.hitTime != DateTime.MinValue)
                    {
                        if ((float)DateTime.Now.Subtract(this.hitTime).TotalSeconds > 2)
                        {
                            if (this.Direction == "Left" && bounce1)
                            {
                                Score("P2", "Time Limit");
                            }
                            else if (this.Direction == "Left")
                            {
                                Score("P1", "Time Limit");
                            }
                            else if (this.Direction == "Right" && bounce1)
                            {
                                Score("P1", "Time Limit");
                            }
                            else
                            {
                                Score("P2", "Time Limit");
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
                        if (yavg < tableLevel - 100)
                        {
                            if (bounce1)
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
                            else
                            {
                                if (this.Direction == "Left")
                                {
                                    Score("P1", "Below Table");
                                }
                                else
                                {
                                    Score("P2", "Below Table");
                                }
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

                        if (served)
                        {
                            // Horizontal direction determination and direction change detection
                            if (xdelta > 5)
                            {
                                if (this.Direction == "Right")
                                {
                                    ChangeDirection();
                                    changeDir = true;
                                }
                                this.Direction = "Left";
                            }
                            else if (xdelta < -5)
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
                                PossibleBounce = false;
                            }
                            else if (ydelta < -5)
                            {
                                if (this.VertDir == "Down" && !changeDir)   // Log possible bounce
                                {
                                    PossibleBounce = true;
                                    this.hitTime = DateTime.Now;
                                    // Get xyz coords for potential bounce
                                    using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                                    {
                                        this.tempBounceXYZ = BounceLocation(depthFrame, xavg, yavg);
                                    }
                                    this.tempBounce = new DataPoint(xavg, yavg, 0, 0);
                                }
                                else if (PossibleBounce)  // if no direction change one frame later, possible bounce is a bounce
                                {
                                    if (!changeDir)
                                    {
                                        // Handle bounce processing
                                        Bounce(new DataPoint(tempBounce.X, tempBounce.Y, 0, (float)(DateTime.Now.Subtract(this.timeStarted).TotalSeconds)));
                                        if (tempBounceXYZ.X != 0 && tempBounceXYZ.Y != 0 && tempBounceXYZ.Z != 0)
                                        {
                                            this.Bounces.Add(tempBounceXYZ);
                                        }
                                    }
                                    PossibleBounce = false;
                                }
                                this.VertDir = "Up";
                            }
                            else if (ydelta <= 0)
                            {
                                if (this.VertDir == "Down" && !changeDir)   // Log possible bounce
                                {
                                    PossibleBounce = true;
                                    this.hitTime = DateTime.Now;
                                    // Get xyz coords for potential bounce
                                    using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                                    {
                                        this.tempBounceXYZ = BounceLocation(depthFrame, xavg, yavg);
                                    }
                                    this.tempBounce = new DataPoint(xavg, yavg, 0, 0);
                                }
                            }
                        }
                        else if (inVolley)    // if not served yet, check for serve hit
                        {
                            // Serve defined as moving in x and negative y
                            if ((xdelta > 10 || xdelta < 10) && ydelta > 10)
                            {
                                this.served = true;
                                this.startPosTime = DateTime.MinValue;
                                this.VertDir = "Down";
                                if (xdelta > 0)
                                {
                                    this.Direction = "Left";
                                }
                                else
                                {
                                    this.Direction = "Right";
                                }
                            }
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

                    CheckStartPosition(xavg, yavg);
                }
            }
        }

        // Check if ball is in start position for new volley.  If so set params to start volley
        public void CheckStartPosition(int xavg, int yavg)
        {
            if (Math.Abs(yavg - tableLevel) < 30 && (xavg > netLocation + 300 || xavg < netLocation - 300))
            {
                string currentServer = "";
                if (xavg < netLocation)
                {
                    currentServer = "P1";
                }
                else
                {
                    currentServer = "P2";
                }

                if (!startPosition)  // Log ball in possible start position
                {
                    this.startPosTime = DateTime.Now;
                    this.startPosition = true;
                    this.startLocation.X = xavg;
                    this.startLocation.Y = yavg;
                }
                else if (Math.Abs(this.startLocation.X - xavg) > 10 || Math.Abs(this.startLocation.Y - yavg) > 10)    // Check ball not moving, reset if it moved
                {
                    this.startPosTime = DateTime.Now;
                    this.startLocation.X = xavg;
                    this.startLocation.Y = yavg;
                }
                else if ((float)(DateTime.Now.Subtract(this.startPosTime).TotalSeconds) > 1.0)    // Start volley if ball not moved in 1.0 seconds
                {
                    if (Server == "")
                    {
                        if (currentServer == "P1")
                        {
                            P1Serve();
                        } else
                        {
                            P2Serve();
                        }
                    }

                    if (Server == currentServer)
                    {
                        this.startPosTime = DateTime.MinValue;
                        this.startPosition = false;
                        StartVolley();
                    }
                    else
                    {
                        //Make some warning sound
                    }
                }
            }
        }

        // Set Player1 as server
        public void P1Serve()
        {
            Server = "P1";
            PlayerOneRecord.Visibility = Visibility.Visible;
            PlayerTwoRecord.Visibility = Visibility.Hidden;
        }

        // Set Player2 as server
        public void P2Serve()
        {
            Server = "P2";
            PlayerOneRecord.Visibility = Visibility.Hidden;
            PlayerTwoRecord.Visibility = Visibility.Visible;
        }

        // Determine whose serve it is
        public void DetermineServe(int diff = 0)
        {
            if (PlayerOneScore == 20)
            {
                P2Serve();
            }
            else if (PlayerTwoScore == 20)
            {
                P1Serve();
            }
            else if ((PlayerOneScore + PlayerTwoScore + diff) % 5 == 0)
            {
                if (Server == "P1")
                {
                    P2Serve();
                }
                else
                {
                    P1Serve();
                }
            }
        }

        // Start new volley 
        public void StartVolley()
        {
            this.AllData.Clear();
            this.xyData.Clear();
            this.timeStarted = DateTime.Now;
            this.served = false;
            this.serveBounce = false;
            this.bounce1 = false;
            this.hitTime = DateTime.MinValue;
            this.inVolley = true;
            this.VertDir = "";
            this.Direction = "";
            VolleyHits = 0;
            VolleyStartTime = DateTime.Now;
            PlayBallSet();
        }

        // Determine volley stats
        private void VolleyStats()
        {
            // Check volley length in hits
            if (VolleyHits > LongestVolleyHits)
            {
                LongestVolleyHits = VolleyHits;
            }

            // check volley length in time
            float volleyLength = (float)(DateTime.Now.Subtract(this.VolleyStartTime).TotalSeconds);
            if (volleyLength > LongestVolleyTime)
            {
                LongestVolleyTime = volleyLength;
            }
        }

        // Register point scored
        private void Score(string player, string message)
        {
            this.hitTime = DateTime.MinValue;
            this.inVolley = false;
            this.scoreDelay = DateTime.Now;
            VolleyStats();

            if (player == "P1")
            {
                this.PlayerOneScore++;
                PlayScore();
            }
            else
            {
                this.PlayerTwoScore++;
                PlayScore();
            }
            DetermineServe();

            if (PlayerOneScore == 21 || PlayerTwoScore == 21)
            {
                gameOver = true;
                PlayGameWin();
                GameOver();
            }
        }

        // Game over handling
        private void GameOver()
        {
            CurrentGame.Player1Score = PlayerOneScore;
            CurrentGame.Player2Score = PlayerTwoScore;
            CurrentGame.LongestVolleyHits = LongestVolleyHits;
            CurrentGame.LongestVolleyTime = LongestVolleyTime;
            gs.AddGame(CurrentGame, Bounces);

            // Write bounce locn data to file (temporary code for testing)
            string[] bounceOut = new string[this.Bounces.Count + 1];
            bounceOut[0] = "X, Y, Z";
            int i = 1;
            foreach (HitLocation item in this.Bounces)
            {
                bounceOut[i] = item.X.ToString() + ", " + item.Y.ToString() + ", " + item.Z.ToString();
                i++;
            }
            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path1 = System.IO.Path.Combine(myPhotos, "BounceData" + ".txt");
            File.WriteAllLines(path1, bounceOut);
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
                    VolleyHits++;
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
                }
            }
            else  // Check for valid serve bounce
            {
                if ((this.Direction == "Right" && hit.X < netLocation) || (this.Direction == "Left" && hit.X > netLocation))
                {
                    this.serveBounce = true;
                    this.hitTime = DateTime.Now.AddSeconds(1);
                }
                else
                {
                    ServeIsBad();
                }
            }
        }

        // Handle bad serve
        private void ServeIsBad()
        {
            PlayBadServe();
            this.serveBounce = false;
            this.inVolley = false;
        }

        // Add Point button Player One
        private void PlayerOneAddPoint_Click(object sender, RoutedEventArgs e)
        {
            Score("P1", "Add Point Button");
        }

        //Subtract Point Button Player One
        private void PlayerOneSubPoint_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerOneScore == 21)
            {
                gameOver = false;
            }
            if (PlayerOneScore !=0)
            {
                PlayerOneScore--;
                PlayBadServe();
            }
            DetermineServe(1);
        }

        // Add Point Button Player Two
        private void PlayerTwoAddPoint_Click(object sender, RoutedEventArgs e)
        {
            Score("P2", "Add Point Button");
        }

        // Subtract Point Player Two
        private void PlayerTwoSubPoint_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerTwoScore == 21)
            {
                gameOver = false;
            }
            if (PlayerTwoScore != 0)
            {
                PlayerTwoScore--;
                PlayBadServe();
            }
            DetermineServe(1);
        }


        // Player One Score and Game Board Update 
        public int PlayerOneScore
        {
            get {return _playerOneScore; }
            set
            {
                if (_playerOneScore != value)
                {
                    _playerOneScore = value;
                    CurrentGame.Player1Score = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("PlayerOneScore"));
                    }
                }
            }
        }
        // Player Two Score and Game Board Update
        public int PlayerTwoScore
        {
            get {return _playerTwoScore; }
            set
            {
                if (_playerTwoScore != value)
                {
                    _playerTwoScore = value;
                    CurrentGame.Player2Score = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("PlayerTwoScore"));
                    }
                }
            }
        }

        // Plays Point Scored Sound
        private void PlayScore()
        {
            PointScored.Load(); 
            PointScored.Play();
        }
        // Plays Ball Set Sound
        private void PlayBallSet()
        {
            BallSet.Load();
            BallSet.Play();
        }
        // Plays Bad Serve Sound
        private void PlayBadServe()
        {
            BadServe.Load();
            BadServe.Play();
        }

        //Plays Game Win Sound
        private void PlayGameWin()
        {
            GameWin.Load();
            GameWin.Play();
        }

        private void PlayerOneRecord_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// Execute shutdown tasks
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
