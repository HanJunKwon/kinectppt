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
using System.Windows.Threading;

namespace WpfApp7
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            InitializeNui();

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);

            m_SaveTime = DateTime.Now.Ticks + 2000 * 10000;
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
            nui.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(nui_AllFramesReady);
            nui.Start();
        }

        int m_nAngle = -27;
        void timer_Tick(object sender, EventArgs e)
        {
            textBlock1.Text = nui.ElevationAngle.ToString();
            try
            {
                nui.ElevationAngle = (m_nAngle + 2);
                m_nAngle += 2;
            }
            catch (Exception ex)
            {
            }
            if (nui.ElevationAngle >= 27)
            {
                m_nAngle = -27;
            }
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

        long m_SaveTime = 0;

        void nui_AllFramesReady(object sender, SkeletonFrameReadyEventArgs e)
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
                            textBlock1.Text = string.Format("X:{0:0.00} Y:{1:0.00}", point.X, point.Y);
                            Canvas.SetLeft(ellipse1, point.X);
                            Canvas.SetTop(ellipse1, point.Y);

                            m_SaveTime = DateTime.Now.Ticks;

                            if (timer.IsEnabled == true)
                            {
                                timer.Stop();
                            }
                        }
                    }
                }
            }
            if (m_SaveTime + 2000 < DateTime.Now.Ticks)
            {
                if (timer.IsEnabled == false)
                {
                    timer.Start();
                }
            }
        }
    }
}
