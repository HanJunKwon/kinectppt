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

namespace WpfApp5
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            InitializeNui();

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        KinectSensor nui = null;
        void InitializeNui()
        {
            nui = KinectSensor.KinectSensors[0];
            nui.ColorStream.Enable();
            nui.ColorFrameReady += new
            EventHandler<ColorImageFrameReadyEventArgs>(nui_ColorFrameReady);
            nui.Start();
        }
        void nui_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame ImageParam = e.OpenColorImageFrame();
            if (ImageParam == null) return;
            byte[] ImageBits = new byte[ImageParam.PixelDataLength];
            ImageParam.CopyPixelDataTo(ImageBits);
            BitmapSource src = null;
            src = BitmapSource.Create(
            ImageParam.Width,
            ImageParam.Height,
            96, 96,
            PixelFormats.Bgr32,
            null,
            ImageBits,
            ImageParam.Width * ImageParam.BytesPerPixel);
            image1.Source = src;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            textBlock1.Text = nui.ElevationAngle.ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                nui.ElevationAngle++;
            }
            catch (Exception ex)
            {
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                nui.ElevationAngle--;
            }
            catch (Exception ex)
            {
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            nui.ElevationAngle = nui.MaxElevationAngle;
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            nui.ElevationAngle = nui.MinElevationAngle;
        }ㅌ

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                nui.ElevationAngle = int.Parse(textBox1.Text);
            }
            catch (Exception ex)
            {
            }
        }


    }
}
