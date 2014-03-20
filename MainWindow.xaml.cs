using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;

namespace KinectSDKDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Runtime _nui;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _nui = new Runtime();
            
            _nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(Nui_VideoFrameReady);
            _nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(Nui_SkeletonFrameReady);

            try
            {
                _nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
                _nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        void Nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            var image = e.ImageFrame.Image;

            img.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.Bits, image.Width * image.BytesPerPixel);
        }

        void Nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            canvas.Children.Clear();

            foreach (SkeletonData user in e.SkeletonFrame.Skeletons)
            {
                if (user.TrackingState == SkeletonTrackingState.Tracked)
                {
                    foreach (Joint joint in user.Joints)
                    {
                        DrawPoint(joint, Colors.Blue);
                    }
                }
            }
        }

        private void DrawPoint(Joint joint, Color color)
        {
            var scaledJoint = ScaleJoint(joint);

            Ellipse ellipse = new Ellipse
            {
              //  Fill = new SolidColorBrush(color),
                Width = 25,
                Height = 25,
                Opacity = 0.5,
                Margin = new Thickness(scaledJoint.Position.X, scaledJoint.Position.Y, 0, 0)
            };

            canvas.Children.Add(ellipse);
        }

        private Joint ScaleJoint(Joint joint)
        {
            return new Joint()
            {
                ID = joint.ID,
                Position = new Microsoft.Research.Kinect.Nui.Vector
                {
                    X = ScalePosition(640, joint.Position.X),
                    Y = ScalePosition(480, -joint.Position.Y),
                    Z = joint.Position.Z,
                    W = joint.Position.W
                },
                TrackingState = joint.TrackingState
            };
        }

        private float ScalePosition(int size, float position)
        {
            float scaledPosition = (((size / 2) * position) + (size / 2));

            if (scaledPosition > size)
            {
                return size;
            }

            if (scaledPosition < 0)
            {
                return 0;
            }

            return scaledPosition;
        }
    }
}
