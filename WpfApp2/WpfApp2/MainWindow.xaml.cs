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
using Microsoft.Kinect;
namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeNui();
        }
        KinectSensor nui = null;
        void InitializeNui()
        {
            nui = KinectSensor.KinectSensors[0];
            nui.ColorStream.Enable();
            nui.ColorFrameReady += new
            EventHandler<ColorImageFrameReadyEventArgs>(nui_ColorFrameReady);
            nui.DepthStream.Enable();
            nui.SkeletonStream.Enable();
            nui.AllFramesReady += new
            EventHandler<AllFramesReadyEventArgs>(nui_AllFramesReady);
            nui.Start();
        }

        void nui_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame ImageParam = e.OpenColorImageFrame();
            if (ImageParam == null) return;
            byte[] ImageBits = new byte[ImageParam.PixelDataLength];
            ImageParam.CopyPixelDataTo(ImageBits);
            BitmapSource src = null;
            src = BitmapSource.Create(ImageParam.Width,
            ImageParam.Height,
            96, 96,
            PixelFormats.Bgr32,
            null,
            ImageBits,
            ImageParam.Width * ImageParam.BytesPerPixel);
            image1.Source = src;
        }

        void nui_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            SkeletonFrame sf = e.OpenSkeletonFrame();
            if (sf == null) return;
            Skeleton[] skeletonData = new Skeleton[sf.SkeletonArrayLength];
            sf.CopySkeletonDataTo(skeletonData);
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {
                    foreach (Skeleton sd in skeletonData)
                    {
                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            Joint joint = sd.Joints[JointType.Head];
                            DepthImagePoint depthPoint;
                            depthPoint = depthImageFrame.MapFromSkeletonPoint(joint.Position);
                            Point point = new Point((int)(image1.Width *
                            depthPoint.X / depthImageFrame.Width),
                            (int)(image1.Height *
                            depthPoint.Y / depthImageFrame.Height));
                            textBlock1.Text = string.Format("X:{0:0.00} Y:{1:0.00} Z:{2:0.00}",
                            point.X, point.Y, joint.Position.Z);
                            Canvas.SetLeft(ellipse1, point.X);
                            Canvas.SetTop(ellipse1, point.Y);
                        }
                    }
                }
            }
        }
    }
}
