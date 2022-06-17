using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Internal;
using OpenCvSharp.WpfExtensions;

namespace DreamScene2
{
    public partial class Album : Form
    {
        public Album()
        {
            InitializeComponent();
        }

        string[] ImageFileTypes = new string[] { ".bmp", ".jpg", ".png" };
        string[] musicFileTypes = new string[] { ".mp3", ".wav", ".aac", ".m4a", ".flac" };
        string name;
        Image img;
        int i = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var imageTypes = string.Join(";", ImageFileTypes.Select(x => $"*{x}"));
            openFileDialog.Filter = $"Image Files|{imageTypes}";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                img = Image.FromFile(openFileDialog.FileName);
                pictureBox1.Image = img;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                int oriWid = pictureBox1.Image.Width;
                int oriHid = pictureBox1.Image.Height;
                if (1.0 * oriWid / oriHid > 800.0 / 450.0)
                {
                    oriHid = (int)(1.0 * oriHid / oriWid * 800);
                    oriWid = 800;
                }
                else
                {
                    oriWid = (int)(1.0 * oriWid / oriHid * 450);
                    oriHid = 450;
                }
                Bitmap b = new Bitmap(800, 450);
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Black);
                g.DrawImage(img, 400 - oriWid / 2, 225 - oriHid / 2, oriWid, oriHid);
                g.Dispose();
                b.Save((++i).ToString() + ".bmp");
                pictureBox1.Image = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            name = textBox1.Text.Insert(textBox1.Text.Length - 4, "(Silent)");
            VideoWriter vw = new VideoWriter(name, FourCC.DIVX, 0.5, new OpenCvSharp.Size(800, 450), true);
            Mat mat = new Mat();
            for (int j = 1; j <= i; j++)
            {
                string file = j.ToString() + ".bmp";
                mat = Cv2.ImRead(file);
                Cv2.Resize(mat, mat, new OpenCvSharp.Size(800, 450));
                
                vw.Write(mat);
            }
            vw.Dispose();
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            textBox1.Enabled = false;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var musicTypes = string.Join(";", musicFileTypes.Select(x => $"*{x}"));
            openFileDialog.Filter = $"Video Files|{musicTypes}";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                Process p = new Process();
                p.StartInfo.FileName = @"D:\ffmpeg-4.4.1-full_build\bin\ffmpeg.exe";
                string args = "-i " + name + " -stream_loop -1 -i " + textBox2.Text + " -t " + (++i) * 2 + " -y " + textBox1.Text;
                p.StartInfo.Arguments = args;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.BeginErrorReadLine();
                p.WaitForExit();
                p.Close();
                p.Dispose();
                button4.Enabled = false;
                button5.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
