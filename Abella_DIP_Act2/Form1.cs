using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using AForge.Video.DirectShow;
using AForge.Video;


namespace Abella_DIP_Act2
{
    
    public partial class Form1 : Form
    {
        Bitmap imageA, imageB, colorgreen, loaded, result;
        String path;
        private bool isCamera = false;
        private bool isRunning = false;
        private bool isFiltered = false;
        private FilterInfoCollection devices;
        private VideoCaptureDevice video;

        public Form1()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageA = new Bitmap(openFileDialog2.FileName);
            pictureBox2.Image = imageA;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            colorgreen.Save(saveFileDialog1.FileName);
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            result = new Bitmap(imageB.Width, imageB.Height);
            Color pixel;
            for (int i = 0; i < imageB.Width; i++)
            {
                for (int j = 0; j < imageB.Height; j++)
                {
                    pixel = imageB.GetPixel(i, j);
                    result.SetPixel(i, j, pixel);
                }
            }
            pictureBox3.Image = imageB;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            result = new Bitmap(imageB.Width, imageB.Height);
            Color pixel;
            int grey;
            for (int i = 0; i < imageB.Width; i++)
            {
                for (int j = 0; j < imageB.Height; j++)
                {
                    pixel = imageB.GetPixel(i, j);
                    grey = (pixel.R + pixel.G + pixel.B) / 3;
                    Color c = Color.FromArgb(grey, grey, grey);
                    result.SetPixel(i, j, c);
                }
            }
            pictureBox3.Image = result;
        }

        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            result = new Bitmap(imageB.Width, imageB.Height);
            Color pixel;
            for (int i = 0; i < imageB.Width; i++)
            {
                for (int j = 0; j < imageB.Height; j++)
                {
                    pixel = imageB.GetPixel(i, j);
                    Color c = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    result.SetPixel(i, j, c);
                }
            }
            pictureBox3.Image = result;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //to greyscale
            result = new Bitmap(imageB.Width, imageB.Height);
            Color pixel;
            int grey;
            for (int i = 0; i < imageB.Width; i++)
            {
                for (int j = 0; j < imageB.Height; j++)
                {
                    pixel = imageB.GetPixel(i, j);
                    grey = (pixel.R + pixel.G + pixel.B) / 3;
                    Color c = Color.FromArgb(grey, grey, grey);
                    result.SetPixel(i, j, c);
                }
            }

            //to data array
            int[] histData = new int[256];
            Color hist;
            for (int i = 0; i < imageB.Width; i++)
            {
                for (int j = 0; j < imageB.Height; j++)
                {
                    hist = result.GetPixel(i, j); // 0-255 either r,g,b
                    histData[hist.R]++;
                }
            }

            Bitmap mydata = new Bitmap(256, 800);
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 800; j++)
                {
                    mydata.SetPixel(i, j, Color.White);
                }
            }

            //plot histdata
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < Math.Min(histData[i] / 5, 800); j++)
                {
                    mydata.SetPixel(i, 799 - j, Color.Black);
                }
            }
            pictureBox3.Image = mydata;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            result = new Bitmap(imageB.Width, imageB.Height);
            Color pixel;
            int tr, tg, tb, r, g, b;
            for (int i = 0; i < imageB.Width; i++)
            {
                for (int j = 0; j < imageB.Height; j++)
                {
                    pixel = imageB.GetPixel(i, j);
                    tr = (int)((0.393 * pixel.R) + (0.796 * pixel.G) + (0.189 * pixel.B));
                    tg = (int)((0.349 * pixel.R) + (0.686 * pixel.G) + (0.168 * pixel.B));
                    tb = (int)((0.272 * pixel.R) + (0.534 * pixel.G) + (0.131 * pixel.B));

                    if (tr > 255)
                    {
                        r = pixel.R;
                    }
                    else
                    {
                        r = tr;
                    }

                    if (tg > 255)
                    {
                        g = pixel.G;
                    }
                    else
                    {
                        g = tg;
                    }

                    if (tb > 255)
                    {
                        b = pixel.B;
                    }
                    else
                    {
                        b = tb;
                    }

                    Color c = Color.FromArgb(r, g, b);
                    result.SetPixel(i, j, c);
                }
            }
            pictureBox3.Image = result;
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void loadBg_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void subtractButton_Click(object sender, EventArgs e)
        {
            result = new Bitmap(imageB.Width, imageB.Height);
            Color pixel;
            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 10;
            for (int i = 0; i < imageB.Width; i++)
            {
                for (int j = 0; j < imageB.Height; j++)
                {
                    pixel  = imageB.GetPixel(i, j);
                    Color backpixel = imageA.GetPixel(i, j);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue < threshold)
                        result.SetPixel(i, j, backpixel);
                    else
                        result.SetPixel(i, j, pixel);
                }
            }
            pictureBox3.Image = result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            StartCameraView();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopCameraView();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            imageB = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = imageB;
        }

        private void videoNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Image cameraFrame = (Image)eventArgs.Frame.Clone();

            pictureBox1.Invoke((MethodInvoker)delegate
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = cameraFrame;
                if (!isFiltered)
                {
                    pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox3.Image = cameraFrame;
                }
            });

        }
        private void StartCameraView()
        {
            devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (devices.Count > 0)
            {
                video = new VideoCaptureDevice(devices[0].MonikerString);
                video.NewFrame += videoNewFrame;
                video.Start();
                isRunning = true;
            }
            else
            {
                MessageBox.Show("No video devices found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopCameraView()
        {

            video.SignalToStop();
            video.WaitForStop();
            isRunning = false;

        }

        
    }
}
