using ImageEncryptCompress;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;
        RGBPixel[,] ImageMatrix_Enc;
        RGBPixel[,] ImageMatrix_Dec;
        List<Dictionary<byte, int>> histograms;
        private void btnOpen_Click(object sender, EventArgs e)
        {
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

        
        private void DecBtn_Click(object sender, EventArgs e)
        {
            ImageMatrix_Dec = ImageOperations.Process(ImageMatrix, initSeedTxt.Text, Convert.ToInt32(TapIndxUpDown.Value));
            ImageOperations.DisplayImage(ImageMatrix_Dec, pictureBox2);
        }

        private void EncBtn_Click(object sender, EventArgs e)
        {
            ImageMatrix_Enc = ImageOperations.Process(ImageMatrix, initSeedTxt.Text, Convert.ToInt32(TapIndxUpDown.Value));
            ImageOperations.DisplayImage(ImageMatrix_Enc, pictureBox2);
        }

        private void HistogramBtn_Click(object sender, EventArgs e)
        {
            huffmanForm frm = new huffmanForm();
            frm.Show();
            this.Hide();
        }
    }
}