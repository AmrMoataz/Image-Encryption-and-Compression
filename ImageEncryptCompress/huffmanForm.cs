using ImageQuantization;
using PriorityQueueDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Priority_Queue;
using System.Diagnostics;

namespace ImageEncryptCompress
{
    public partial class huffmanForm : Form
    {
        public huffmanForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix_Enc;
        //RGBPixel[,] ImageMatrix_Dec;
        RGBPixel[,] ImageMatrix;
        Stopwatch stopWatch;
        List<SimplePriorityQueue<Node, int>> Queues;
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            redValue.Items.Clear();
            redPriority.Items.Clear();
            blueValue.Items.Clear();
            bluePriority.Items.Clear();
            greenValue.Items.Clear();
            greenPriority.Items.Clear();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stopWatch = new Stopwatch();
            

            ImageMatrix_Enc = ImageOperations.Process(ImageMatrix, initSeedTxt.Text, Convert.ToInt32(TapIndxUpDown.Value));

            stopWatch.Start();
            ImageOperations.DisplayImage(ImageMatrix_Enc, pictureBox1);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            textBox1.Text = elapsedTime.ToString();
            stopWatch.Reset();

            Queues = new List<SimplePriorityQueue<Node, int>>();
            stopWatch.Start();
            Queues = ImageOperations.getHistogram(ImageMatrix_Enc);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;

            string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
           ts1.Hours, ts1.Minutes, ts1.Seconds,
           ts1.Milliseconds / 10);

            textBox2.Text = elapsedTime1.ToString();
            stopWatch.Reset();

            ImageOperations.BuildHuffmanTree(Queues);
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            redValue.Items.Clear();
            redPriority.Items.Clear();
            blueValue.Items.Clear();
            bluePriority.Items.Clear();
            greenValue.Items.Clear();
            greenPriority.Items.Clear();

            ImageMatrix_Enc = ImageOperations.Process(ImageMatrix, initSeedTxt.Text, Convert.ToInt32(TapIndxUpDown.Value));

            Queues = new List<SimplePriorityQueue<Node, int>>();
            Queues = ImageOperations.getHistogram(ImageMatrix_Enc);


            for (int i = 0; i < Queues.Count; i++)
            {
                if (i == 0)
                {
                    while (Queues[i].Count != 0)
                    {
                        var item = Queues[i].First();
                        var priority = Queues[i].GetPriority(item);
                        Queues[i].Dequeue();
                        redValue.Items.Add(item.value.ToString());
                        redPriority.Items.Add(priority);

                    }
                }

                if (i == 1)
                {
                    while (Queues[i].Count != 0)
                    {
                        var item = Queues[i].First();
                        var priority = Queues[i].GetPriority(item);
                        Queues[i].Dequeue();
                        blueValue.Items.Add(item.value.ToString());
                        bluePriority.Items.Add(priority);
                    }
                }

                if (i == 2)
                {
                    while (Queues[i].Count != 0)
                    {
                        var item = Queues[i].First();
                        var priority = Queues[i].GetPriority(item);
                        Queues[i].Dequeue();
                        greenValue.Items.Add(item.value.ToString());
                        greenPriority.Items.Add(priority);
                    }
                }
            }

        }
    }
}
