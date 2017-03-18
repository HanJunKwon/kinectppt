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
using Microsoft.Kinect
    ;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        Rectangle[] m_rect = new Rectangle[20];
        public MainWindow()
        {
            InitializeComponent();
            InitializeNui();

            for (int i = 0; i < 20; i++)
            {
                m_rect[i] = new Rectangle();
                m_rect[i].Fill = new SolidColorBrush(Colors.Red);
                m_rect[i].Height = 10;
                m_rect[i].Width = 10;
                m_rect[i].Visibility = Visibility.Collapsed;
                Canvas.SetTop(m_rect[i], 0);
                Canvas.SetLeft(m_rect[i], 0);
                canvas1.Children.Add(m_rect[i]);
            }
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
                            int nMax = 20;
                            Joint[] joints = new Joint[nMax];
                            for (int j = 0; j < nMax; j++)
                            {
                                joints[j] = sd.Joints[(JointType)j];
                            }

                            Point[] points = new Point[nMax];
                            for (int j = 0; j < nMax; j++)
                            {
                                DepthImagePoint depthPoint;
                                depthPoint = depthImageFrame.MapFromSkeletonPoint(joints[j].Position);
                                points[j] = new Point((int)(image1.Width *
                                depthPoint.X / depthImageFrame.Width),
                                (int)(image1.Height *
                                depthPoint.Y / depthImageFrame.Height));
                            }

                            for (int j = 0; j < nMax; j++)
                            {
                                m_rect[j].Visibility = Visibility.Visible;
                                Canvas.SetTop(m_rect[j],
                                points[j].Y - (m_rect[j].Height / 2));
                                Canvas.SetLeft(m_rect[j],
                                points[j].X - (m_rect[j].Width / 2));
                            }
                        }
                    }

                }
            }
        }
    }
}
            