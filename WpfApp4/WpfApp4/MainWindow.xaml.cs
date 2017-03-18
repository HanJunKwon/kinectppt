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

namespace WpfApp4
{
    public partial class MainWindow : Window
    {
        Polyline[] m_poly = new Polyline[5];

        public MainWindow()
        {
            InitializeComponent();
            InitializeNui();
            for (int i = 0; i < 5; i++)
            {
                m_poly[i] = new Polyline();
                m_poly[i].Stroke = new SolidColorBrush(Colors.Green);
                m_poly[i].StrokeThickness = 4;
                m_poly[i].Visibility = Visibility.Collapsed;
                Canvas.SetTop(m_poly[i], 0);
                Canvas.SetLeft(m_poly[i], 0);
                canvas1.Children.Add(m_poly[i]);
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
                            }                            PointCollection pc0 = new PointCollection(4);
                            pc0.Add(points[(int)JointType.HipCenter]);
                            pc0.Add(points[(int)JointType.Spine]);
                            pc0.Add(points[(int)JointType.ShoulderCenter]);
                            pc0.Add(points[(int)JointType.Head]);
                            m_poly[0].Points = pc0;
                            m_poly[0].Visibility = Visibility.Visible; //엉덩이부터 머리까지 연결되는 선

                            PointCollection pc1 = new PointCollection(5);
                            pc1.Add(points[(int)JointType.ShoulderCenter]);
                            pc1.Add(points[(int)JointType.ShoulderLeft]);
                            pc1.Add(points[(int)JointType.ElbowLeft]);
                            pc1.Add(points[(int)JointType.WristLeft]);
                            pc1.Add(points[(int)JointType.HandLeft]);
                            m_poly[1].Points = pc1;
                            m_poly[1].Visibility = Visibility.Visible; //왼손부터 어깨까지 연결되는 선

                            PointCollection pc2 = new PointCollection(5);
                            pc2.Add(points[(int)JointType.ShoulderCenter]);
                            pc2.Add(points[(int)JointType.ShoulderRight]);
                            pc2.Add(points[(int)JointType.ElbowRight]);
                            pc2.Add(points[(int)JointType.WristRight]);
                            pc2.Add(points[(int)JointType.HandRight]);
                            m_poly[2].Points = pc2;
                            m_poly[2].Visibility = Visibility.Visible; // 오른손부터 어깨까지 연결되는 선

                            PointCollection pc3 = new PointCollection(5);
                            pc3.Add(points[(int)JointType.HipCenter]);
                            pc3.Add(points[(int)JointType.HipLeft]);
                            pc3.Add(points[(int)JointType.KneeLeft]);
                            pc3.Add(points[(int)JointType.AnkleLeft]);
                            pc3.Add(points[(int)JointType.FootLeft]);
                            m_poly[3].Points = pc3;
                            m_poly[3].Visibility = Visibility.Visible; // 왼발부터 엉덩이까지 연결되는 선

                            PointCollection pc4 = new PointCollection(5);
                            pc4.Add(points[(int)JointType.HipCenter]);
                            pc4.Add(points[(int)JointType.HipRight]);
                            pc4.Add(points[(int)JointType.KneeRight]);
                            pc4.Add(points[(int)JointType.AnkleRight]);
                            pc4.Add(points[(int)JointType.FootRight]);
                            m_poly[4].Points = pc4;
                            m_poly[4].Visibility = Visibility.Visible; // 오른발부터 엉덩이까지 연결되는 선
                        }
                    }
                }
            }
        }
    }
}
