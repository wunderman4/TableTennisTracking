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
using System.Windows.Controls.Primitives;
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
    public class BallCoords
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public DateTime Time { get; set; }
    }

    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page, INotifyPropertyChanged
    {
        public bool debug = true;

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
        bool pause;
        public bool bounce1;
        public bool serveBounce;
        public bool inVolley;
        public int VolleyHits;
        public int LongestVolleyHits;
        public DateTime VolleyStartTime;
        public float LongestVolleyTime;
        public int VolleyNumber;
        public double maxSpeed;
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
        public BallCoords CurrentXYZ;
        public BallCoords PreviousXYZ;
        public string _Server;
        public bool plotRepeat = false;
        public string _scoreMessageString;
        public string[] SpeedData;
        public int SpeedIndex;

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
            this.tempBounceXYZ = new HitLocation();
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
            VolleyNumber = 1;
            maxSpeed = 0;
            pause = false;
            SpeedData = new string[10000];
            SpeedIndex = 0;
        }

        // Which player has serve
        public string Server
        {
            get { return _Server; }
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
                        for (int i = 0; i < colorFrameDescription.Width * (colorFrameDescription.Height - (tableLevel - 200)); i++)
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
            BallLocation.Time = (float)(DateTime.Now.Subtract(this.timeStarted).TotalSeconds);
            return (BallLocation);
        }

        // Return median of a list of float values
        private float FindMedian(List<float> inList)
        {
            inList.Sort();
            int index = inList.Count / 2;
            return inList[index];
        }

        // Get xyz physical coordinates 
        private BallCoords XYZLocation(DepthFrame depthFrame, int xavg, int yavg)
        {
            BallCoords ballLocn = new BallCoords();
            if (depthFrame == null)
            {
                return (ballLocn);
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
                            if (camSpacePoints[tempIndex].Z > GlobalClass.minZ && camSpacePoints[tempIndex].Z < 3.6)
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
                    ballLocn.X = FindMedian(xvals);
                    ballLocn.Y = FindMedian(yvals);
                    ballLocn.Z = FindMedian(zvals);
                    ballLocn.Time = DateTime.Now;
                }
                else
                {
                    ballLocn.X = 0;
                    ballLocn.Y = 0;
                    ballLocn.Z = 0;
                }
                return (ballLocn);
            }
        }

        // Check if ball location is reasonable compared to previous location, return false if not
        private bool ReasonableLocation(DataPoint NewLocation)
        {
            DataPoint PrevLocation = AllData[AllData.Count - 1];
            double deltaTimeSteps = (NewLocation.Time - PrevLocation.Time) / 0.03;
            float deltax = NewLocation.X - PrevLocation.X;
            float deltay = NewLocation.Y - PrevLocation.Y;
            double distanceTravelled = Math.Sqrt((deltax * deltax + deltay * deltay));

            // check for travel of more than 200 pixels per time step
            if (distanceTravelled > 200 * deltaTimeSteps)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Calculate ball speed in m/s
        private double BallSpeed(BallCoords currLocn, BallCoords prevLocn)
        {
            if (currLocn.X != 0 && currLocn.Y != 0 && prevLocn.X != 0 && prevLocn.Y != 0)
            {
                double deltax = currLocn.X - prevLocn.X;
                double deltay = currLocn.Y - prevLocn.Y;
                double deltat = currLocn.Time.Subtract(prevLocn.Time).TotalSeconds;
                double distance = Math.Sqrt(deltax * deltax + deltay * deltay);
                SpeedData[SpeedIndex] = currLocn.X + "  " + prevLocn.X + "  " + currLocn.Y + "  " + prevLocn.Y + "  " + distance + "  " + deltat;
                SpeedIndex++;
                return distance / deltat;
            } else
            {
                return 0;
            }
        }

        // Color frame analysis With ball action analysis
        private void Frame_Arrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            bool changeDir = false;
            

            if (scoreDelay != DateTime.MinValue)
            {
                if ((float)DateTime.Now.Subtract(this.scoreDelay).TotalSeconds < 2)
                {
                    pause = true;
                }
                else
                {
                    scoreDelay = DateTime.MinValue;
                    pause = false;
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
                        if ((float)DateTime.Now.Subtract(this.hitTime).TotalSeconds > 1.5)
                        {
                            if (this.Direction == "Left" && bounce1)
                            {
                                Score("P2", "Time Limit - no return");
                            }
                            else if (this.Direction == "Left")
                            {
                                Score("P1", "Time Limit - no bounce");
                            }
                            else if (this.Direction == "Right" && bounce1)
                            {
                                Score("P1", "Time Limit - no return");
                            }
                            else
                            {
                                Score("P2", "Time Limit - no bounce");
                            }
                        }
                    }

                    // Get Ball xy coordinates
                    DataPoint BallLocation = FindBall(multiSourceFrame);
                    int xavg = (int)BallLocation.X;
                    int yavg = (int)BallLocation.Y;

                    // Check if location seems reasonable, set xavg to 1 (default no ball found value) if not
                    if (AllData.Count > 0)
                    {
                        if (!ReasonableLocation(BallLocation))
                        {
                            xavg = 1;
                        }
                    }

                    // If good data point, analyze it
                    if (xavg > 1)
                    {
                        // Get ball xyz coordinates
                        using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
                        {
                            CurrentXYZ = XYZLocation(depthFrame, xavg, yavg);
                        }

                        // Off (or rather under) table
                        if (yavg < tableLevel - 100)
                        {
                            if (bounce1)
                            {
                                if (this.Direction == "Left")
                                {
                                    Score("P2", "Below Table - no return");
                                }
                                else
                                {
                                    Score("P1", "Below Table - no return");
                                }
                            }
                            else if (served)
                            {
                                if (this.Direction == "Left")
                                {
                                    Score("P1", "Below Table - no bounce");
                                }
                                else
                                {
                                    Score("P2", "Below Table - no bounce");
                                }
                            }
                            else
                            {
                                ServeIsBad();
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
                            if (xdelta > 10)
                            {
                                if (this.Direction == "Right")
                                {
                                    ChangeDirection(ydelta);
                                    changeDir = true;
                                }
                                this.Direction = "Left";
                            }
                            else if (xdelta < -10)
                            {
                                if (this.Direction == "Left")
                                {
                                    ChangeDirection(ydelta);
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
                                if (this.VertDir == "Down" && !changeDir && yavg - tableLevel < 100)   // Log possible bounce
                                {
                                    PossibleBounce = true;
                                    this.hitTime = DateTime.Now;
                                    this.tempBounce = new DataPoint(xavg, yavg, 0, 0);
                                }
                                else if (PossibleBounce)  // if no direction change one frame later, possible bounce is a bounce
                                {
                                    if (!changeDir)
                                    {
                                        // Handle bounce processing
                                        if (PreviousXYZ.X != 0 && PreviousXYZ.Y != 0 && PreviousXYZ.Z != 0 && PreviousXYZ.Z > GlobalClass.minZ && this.serveBounce)
                                        {
                                            HitLocation bounceXYZ = new HitLocation();
                                            bounceXYZ.X = PreviousXYZ.X;
                                            bounceXYZ.Y = PreviousXYZ.Y;
                                            bounceXYZ.Z = PreviousXYZ.Z;
                                            bounceXYZ.Volley = VolleyNumber;
                                            this.Bounces.Add(bounceXYZ);
                                        }
                                        Bounce(new DataPoint(tempBounce.X, tempBounce.Y, 0, (float)(DateTime.Now.Subtract(this.timeStarted).TotalSeconds)));
                                    }
                                    PossibleBounce = false;
                                }
                                this.VertDir = "Up";
                            }
                            else if (PossibleBounce)
                            {
                                PossibleBounce = false;
                            }
                            else if (ydelta <= 0)
                            {
                                if (this.VertDir == "Down" && !changeDir && yavg - tableLevel < 100)   // Log possible bounce
                                {
                                    PossibleBounce = true;
                                    this.hitTime = DateTime.Now;
                                    this.tempBounce = new DataPoint(xavg, yavg, 0, 0);
                                }
                            }
                        }
                        else if (inVolley)    // if not served yet, check for serve hit
                        {
                            // check for low, shallow bounce serve
                            int xStartDelta = 0;
                            if (Server == "P1")
                            {
                                xStartDelta = xavg - (int)startLocation.X;
                            }
                            else
                            {
                                xStartDelta = (int)startLocation.X - xavg;
                            }

                            if (xStartDelta > 30)
                            {
                                served = true;
                                VolleyStartTime = DateTime.Now;
                                if (ydelta > 10)
                                {
                                    VertDir = "Down";
                                } 
                                else if (yavg - tableLevel < 50)
                                {
                                    VertDir = "Up";
                                    serveBounce = true;
                                }

                                if (xdelta > 0)
                                {
                                    this.Direction = "Left";
                                }
                                else
                                {
                                    this.Direction = "Right";
                                }
                            }

                            // Serve defined as moving in x and negative y (slower serve, easy bounce detection)
                             else if ((xdelta > 10 || xdelta < 10) && ydelta > 10 && xStartDelta > 30)
                            {
                                this.served = true;
                                VolleyStartTime = DateTime.Now;
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
                        double currentSpeed = 0;

                        // Check ball speed
                        if (PreviousXYZ != null)
                        {
                            currentSpeed = BallSpeed(CurrentXYZ, PreviousXYZ);
                        }
                        if (currentSpeed > maxSpeed)
                        {
                            maxSpeed = currentSpeed;
                        }
                        PreviousXYZ = CurrentXYZ;
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
            if (Math.Abs(yavg - tableLevel) < 300 && (xavg > netLocation + 300 || xavg < netLocation - 300))
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
                        }
                        else
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
            PlayerOneBall.Visibility = Visibility.Collapsed;
            PlayerTwoBall.Visibility = Visibility.Collapsed;
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
            PlayBallSet();
            if (Server == "P1")
            {
                PlayerOneBall.Visibility = Visibility.Visible;
            }
            else
            {
                PlayerTwoBall.Visibility = Visibility.Visible;
            }
            PlayerOneUserName.TextDecorations = null;
            PlayerTwoUserName.TextDecorations = null;
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
            if (volleyLength > LongestVolleyTime && volleyLength < 90)
            {
                LongestVolleyTime = volleyLength;
            }
        }

        // Point scored message
        public string ScoreMessageString
        {
            get { return _scoreMessageString; }
            set
            {
                _scoreMessageString = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs("ScoreMessageString"));
                }
            }
        }

        // Register point scored
        private void Score(string player, string message)
        {
            this.hitTime = DateTime.MinValue;
            this.inVolley = false;
            this.scoreDelay = DateTime.Now;
            VolleyStats();
            VolleyNumber++;

            ScoreMessageString = message;

            TextDecoration myUnderline = new TextDecoration();
            myUnderline.Pen = new Pen(Brushes.LimeGreen, 2);
            TextDecorationCollection myCollection = new TextDecorationCollection();
            myCollection.Add(myUnderline);

            if (player == "P1")
            {
                this.PlayerOneScore++;
                PlayScore();
                PlayerOneUserName.TextDecorations = myCollection;
                if (debug)
                {
                    P1ScoreMessage.Visibility = Visibility.Visible;
                }
                P2ScoreMessage.Visibility = Visibility.Hidden;
            }
            else
            {
                this.PlayerTwoScore++;
                PlayScore();
                PlayerTwoUserName.TextDecorations = myCollection;
                P1ScoreMessage.Visibility = Visibility.Hidden;
                if (debug)
                {
                    P2ScoreMessage.Visibility = Visibility.Visible;
                }
            }
            DetermineServe();

            if (PlayerOneScore == 21 || PlayerTwoScore == 21)
            {
                PlayerOneSubPoint.Visibility = Visibility.Collapsed;
                PlayerOneAddPoint.Visibility = Visibility.Collapsed;
                PlayerTwoSubPoint.Visibility = Visibility.Collapsed;
                PlayerTwoAddPoint.Visibility = Visibility.Collapsed;
                UndoGameOverButton.Visibility = Visibility.Visible;
                GameSummaryButton.Visibility = Visibility.Visible;

                gameOver = true;
                PlayGameWin();

            }
        }


        // Asks if user would like to undo game over
        private void UndoGameOverButton_Click(object sender, RoutedEventArgs e)
        {
            UndoGameOverButton.Visibility = Visibility.Collapsed;
            GameSummaryButton.Visibility = Visibility.Collapsed;
            PlayerOneSubPoint.Visibility = Visibility.Visible;
            PlayerOneAddPoint.Visibility = Visibility.Visible;
            PlayerTwoSubPoint.Visibility = Visibility.Visible;
            PlayerTwoAddPoint.Visibility = Visibility.Visible;



            if (PlayerOneScore == 21)
            {
                PlayerOneScore--;

            }
            else if (PlayerTwoScore == 21)
            {
                PlayerTwoScore--;
            }
            DetermineServe(1);
            gameOver = false;


        }

        // Confirms Game Over navigates to game summary page
        private async void GameSummaryButton_Click(object sender, RoutedEventArgs e)
        {
            PBarSpan.Visibility = Visibility.Visible;
            Task FinishGame = Task.Factory.StartNew(() => GameOver());
            await FinishGame;
            NavigationService.Navigate(new GameSummary(CurrentGame));
        }

        // Game over handling
        private void GameOver()
        {
            CurrentGame.Player1Score = PlayerOneScore;
            CurrentGame.Player2Score = PlayerTwoScore;
            CurrentGame.LongestVolleyHits = LongestVolleyHits;
            CurrentGame.LongestVolleyTime = LongestVolleyTime;
            CurrentGame.MaxVelocity = (float)maxSpeed;
            gs.AddGame(CurrentGame, Bounces);

            // Write bounce locn data to file (temporary code for testing)
            if (debug)
            {
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

                string path2 = System.IO.Path.Combine(myPhotos, "SpeedData.txt");
                File.WriteAllLines(path2, SpeedData);
            }
        }

        // Handle change in ball direction
        private void ChangeDirection(float ydelta)
        {
            if (serveBounce)
            {
                // Check for fast bounce + return
                if (PossibleBounce && ydelta < -5 && !bounce1)
                {
                    bounce1 = true;
                    PossibleBounce = false;
                }

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
                    Score("P2", "Bounce wrong side");
                }
                else if (this.Direction == "Left" && hit.X > netLocation)    // Wrong side of net
                {
                    Score("P1", "Bounce wrong side");
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
                    this.hitTime = DateTime.Now.AddSeconds(2);
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
            PlayerOneBall.Visibility = Visibility.Collapsed;
            PlayerTwoBall.Visibility = Visibility.Collapsed;
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
            if (PlayerOneScore != 0)
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
            get { return _playerOneScore; }
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
            get { return _playerTwoScore; }
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

        // Create xyData list from AllData, send to XAML plot
        public async void PlotXYData()
        {
            // Pause ball tracking while showing volley graph
            pause = true;

            for (int i = 0; i < AllData.Count(); i++)
            {
                this.xyData.Add(new KeyValuePair<float, float>(AllData[i].X, AllData[i].Y));
                if (i >= 5)
                {
                    this.xyData.Remove(new KeyValuePair<float, float>(AllData[i - 5].X, AllData[i - 5].Y));
                }
                chart1.DataContext = null;
                chart1.DataContext = this.xyData;
                await Task.Delay(110);
            }
            await Task.Delay(1500);
            this.xyData.Clear();
            chart1.DataContext = null;
            chart1.DataContext = this.xyData;
            VolleyPlot.IsOpen = false;

            // resume ball tracking
            pause = false;
        }

        // Show plot of ball locations for volley
        private void ShowVolleyPlot(object sender, RoutedEventArgs e)
        {
            PlotXYData();
            VolleyPlot.IsOpen = true;
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
        }

    }
}
