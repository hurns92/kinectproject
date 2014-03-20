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
using System.Windows.Forms;
using Microsoft.Kinect;


namespace TestKeystrokeInput
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        KinectSensor _ks;

        private void InitializeKinect()
        {
    
            if (KinectSensor.KinectSensors.Count > 0)
            {

                _ks = KinectSensor.KinectSensors[0];

                if (_ks.Status == KinectStatus.Connected)
                {
                    //_ks.ColorStream.Enable();
                    //_ks.DepthStream.Enable();
                    _ks.SkeletonStream.Enable();

                    _ks.AllFramesReady += _ks_AllFramesReady;
                    this._ks.SkeletonFrameReady += _ks_SkeletonFrameReady;
                    _ks.Start();
                }

            }

        }

        void _ks_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            

            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            
            using (DrawingContext dc = this.drawingGroup.Open())
            {
             

                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
            
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }
            }
            
        }

        void _ks_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

        }

        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = _ks.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        void DisKinect()
        {
            if (_ks != null)
            {
                _ks.Stop();
                _ks.AudioSource.Stop();
            }

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            InitializeKinect();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisKinect();
        }

        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            float headloc = 0;
            
            float LefthandlocY = 0;
            float LefthandlocX = 0;
            
            float RighthandlocY = 0;
            float RighthandlocX = 0;


            int movementcount = 0;



            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }


                //find head location
                
                if (joint.JointType == JointType.Head)
                {
                    headloc = joint.Position.Y;
                }

                //find left hand location
                if (joint.JointType == JointType.HandLeft)
                {
                    LefthandlocY = joint.Position.Y;
                    LefthandlocX = joint.Position.X;
                }

                //find right hand location
                if (joint.JointType == JointType.HandRight)
                {
                    RighthandlocY = joint.Position.Y;
                    RighthandlocX = joint.Position.X;
                }

            }
            //see if left hand is above head

            if (LefthandlocY > headloc && movementcount == 0)
            {
                
                Indicator.Background = new SolidColorBrush(Colors.Green);

                SendKeys.SendWait("{LEFT}");

                movementcount = movementcount + 1;

            }

            // see if right hand is above head

            if (RighthandlocY > headloc && movementcount == 0)
            {
                SendKeys.SendWait("{RIGHT}");

                movementcount = movementcount + 1;
            }

            // see if hands are crossed

            if (LefthandlocX > RighthandlocX && movementcount == 0)
            {
                SendKeys.SendWait("z");
                CrossIndicator.Background = new SolidColorBrush(Colors.Purple);
                movementcount = movementcount + 1;
               
            }

            drawingContext.Close();
          
        }

    }
}
