using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public List<DataPoint> Bounces;
        public string Direction;
        public string VertDir;
        // Volley tracking variables
        public bool bounce1;
        public bool serveBounce;
        public bool inVolley;
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
        public DataPoint tempBounceXYZ;
        public UInt16[] PreviousDepthFrame;

        // Constructor
        public GamePage(Player pOne, Player pTwo)
        {
            InitializeComponent();
            PlayerOne = pOne;
            PlayerTwo = pTwo;
            NewGame();
            this.DataContext = this;
            //Player One DataContext
            PlayerOneUserName.DataContext = PlayerOne;
            PlayerOneName.DataContext = PlayerOne;
            PlayerOneNation.DataContext = PlayerOne;
            PlayerOnePPHand.DataContext = PlayerOne;
            

            //Player Two DataContext
            PlayerTwoUserName.DataContext = PlayerTwo;
            PlayerTwoName.DataContext = PlayerTwo;
            PlayerTwoNation.DataContext = PlayerTwo;
            PlayerTwoPPHand.DataContext = PlayerTwo;

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
            this.Bounces = new List<DataPoint>();
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

        // Get xyz physical coordinates for bounce location, add to Bounce list - not very good right now
        private DataPoint BounceLocation(DepthFrame depthFrame, int xavg, int yavg)
        {
            DataPoint bounceLocn = new TableTennisTracker.DataPoint(0, 0, 0, 0);
            if (depthFrame == null)
            {
                return (bounceLocn);
            }

            using (KinectBuffer depthFrameData = depthFrame.LockImageBuffer())
            {

            CameraSpacePoint[] camSpacePoints = new CameraSpacePoint[1920 * 1080];
            //this.coordinateMapper.MapColorFrameToCameraSpace(this.PreviousDepthFrame, camSpacePoints);
            this.coordinateMapper.MapColorFrameToCameraSpaceUsingIntPtr(depthFrameData.UnderlyingBuffer, depthFrameData.Size, camSpacePoints);
            int index = (1080 - yavg) * 1920 + xavg;
            float xtval = camSpacePoints[index].X;
            float ytval = camSpacePoints[index].Y;
            float ztval = camSpacePoints[index].Z;

            if (ztval < 0 || ztval > 3.5)
            {
                for (int a = -5; a <= 5; a++)
                {
                    for (int b = -5; b <= 5; b++)
                    {
                        if (camSpacePoints[index + (a * 1920) + b].Z > 0 || camSpacePoints[index + (a * 1920) + b].Z < 3.5)
                        {
                            xtval = camSpacePoints[index + (a * 1920) + b].X;
                            ytval = camSpacePoints[index + (a * 1920) + b].Y;
                            ztval = camSpacePoints[index + (a * 1920) + b].Z;
                        }
                    }
                }
            }
            bounceLocn.X = xtval;
            bounceLocn.Y = ytval;
            bounceLocn.Z = ztval;
            return (bounceLocn);
            }
        }

        // Color frame analysis
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
                        if (yavg < tableLevel - 200)
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

                            //this.tempBounceXYZ = BounceLocation(PreviousDepthFrame, xavg, yavg);
                            using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                            {
                                this.tempBounceXYZ = BounceLocation(depthFrame, xavg, yavg);
                                //if (depthFrame != null)
                                //{
                                //    depthFrame.CopyFrameDataToArray(this.PreviousDepthFrame);
                                //}

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
                                    //using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                                    //{
                                    //    this.tempBounceXYZ = BounceLocation(depthFrame, xavg, yavg);
                                    //}
                                    this.tempBounce = new DataPoint(xavg, yavg, 0, 0);
                                }
                                else if (PossibleBounce)  // if no direction change one frame later, possible bounce is a bounce
                                {
                                    if (!changeDir)
                                    {
                                        // Handle bounce processing
                                        Bounce(new DataPoint(tempBounce.X, tempBounce.Y, 0, (float)(DateTime.Now.Subtract(this.timeStarted).TotalSeconds)));
                                        this.Bounces.Add(tempBounceXYZ);
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
                                    //using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                                    //{
                                        //this.tempBounceXYZ = BounceLocation(depthFrame, xavg, yavg);
                                    //}
                                    this.tempBounce = new DataPoint(xavg, yavg, 0, 0);
                                }
                            }
                        }
                        else if (inVolley)    // if not served yet, check for serve hit
                        {
                            // Serve defined as moving in x and negative y, and decently above table 
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

                    // Determine if ball in start position 1.5 seconds to signal start of volley
                    if (Math.Abs(yavg - this.tableLevel) < 30 && (xavg > netLocation + 300 || xavg < netLocation - 300))
                    {
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
                        else if ((float)(DateTime.Now.Subtract(this.startPosTime).TotalSeconds) > 1.5)    // Start volley if ball not moved in 1.5 seconds
                        {
                            this.startPosTime = DateTime.MinValue;
                            this.startPosition = false;
                            StartVolley();
                        }
                        else
                        {
                        }
                    }
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
            PlayBallSet();
        }

        // Register point scored
        private void Score(string player, string message)
        {
            this.hitTime = DateTime.MinValue;
            this.inVolley = false;
            this.scoreDelay = DateTime.Now;

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

            if (PlayerOneScore == 21 || PlayerTwoScore == 21)
            {
                gameOver = true;
                PlayGameWin();
                GameOver();
            }
        }

        // Find longest volley in number of hits - NOT YET WRITTEN
        private int LongestVolleyHits()
        {
            return 0;
        }

        // Find longest volley in time - NOT YET WRITTEN
        private float LongestVolleyTime()
        {
            return 0;
        }

        // Game over handling
        private void GameOver()
        {
            CurrentGame.Player1Score = PlayerOneScore;
            CurrentGame.Player2Score = PlayerTwoScore;
            CurrentGame.LongestVolleyHits = LongestVolleyHits();
            CurrentGame.LongestVolleyTime = LongestVolleyTime();
            gs.AddGame(CurrentGame);
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
            PlayerOneScore++;
            PlayScore();
        }

        //Subtract Point Button Player One
        private void PlayerOneSubPoint_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerOneScore !=0)
            {
                PlayerOneScore--;
                PlayBadServe();
            }
        }

        // Add Point Button Player Two
        private void PlayerTwoAddPoint_Click(object sender, RoutedEventArgs e)
        {
            PlayerTwoScore++;
            PlayScore();
        }

        // Subtract Point Player Two
        private void PlayerTwoSubPoint_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerTwoScore != 0)
            {
                PlayerTwoScore--;
                PlayBadServe();
            }
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
