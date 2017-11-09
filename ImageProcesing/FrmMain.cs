using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcesing
{
    public partial class FrmMain : Form
    {
        // CONVOLUTION ARRAY
        const int CONV_SIZE = 3;
        private int[] CONV_SHARP = { 0, -1, 0, -1, 5, -1, 0, -1, 0};
        // Save image in Input image
        private Bitmap imageInput = null;
        private Bitmap imageOutput = null;



        public FrmMain()
        {
            InitializeComponent();
        }


        //<sumary>
        //   Open FileDialog to open image from disk
        //</sumary>
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            // open file dialog   
            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "Image Files (*.jpg; *.jpeg; *.gif; *.bmp, *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                Bitmap bmp = new Bitmap(open.FileName);
                // 
                imageInput = bmp;
                picInput.Image = bmp;
                // Get image info
            }
            else
            {
                //TODO
            }
        }
        

        //<sumary>
        //   x, y : point (x,y)
        //   size : convolution size
        //</sumary>
        private Color GetAvg(LockBitmap lbm, int x, int y, int size = 3)
        {
            if(size % 2 == 0) throw new ArgumentException("Convolution size must is odd number");
            int sumR = 0, sumG = 0, sumB = 0, sumA = 0;
            for(int i = x  - size/2; i <= x + size/2; i++)
            {
                for (int j = y - size / 2; j <= y + size / 2; j++)
                {
                    if(i >= 0 && j >= 0 && i < lbm.Width && j < lbm.Height)
                    {
                        sumA += (int)lbm.GetPixel(i, j).A;
                        sumR += (int)lbm.GetPixel(i, j).R;
                        sumG += (int)lbm.GetPixel(i, j).G;
                        sumB += (int)lbm.GetPixel(i, j).B;
                    }
                }
            }
            sumA = (int)(Math.Round((double)sumA / (size * size), MidpointRounding.AwayFromZero));
            sumR = (int)(Math.Round((double)sumR / (size * size), MidpointRounding.AwayFromZero));
            sumG = (int)(Math.Round((double)sumG / (size * size), MidpointRounding.AwayFromZero));
            sumB = (int)(Math.Round((double)sumB / (size * size), MidpointRounding.AwayFromZero));

            return (lbm.Depth == 32 ? Color.FromArgb(sumA, sumR, sumG, sumB) : Color.FromArgb(sumR, sumG, sumB));
        }

        private Color GetMedian(LockBitmap lbm, int x, int y, int size = 3)
        {
            List<byte> rColor = new List<byte>();
            List<byte> gColor = new List<byte>();
            List<byte> bColor = new List<byte>();
            List<byte> aColor = new List<byte>();

            if (size % 2 == 0) throw new ArgumentException("Convolution size must is odd number");
            for (int i = x - size / 2; i <= x + size / 2; i++)
            {
                for (int j = y - size / 2; j <= y + size / 2; j++)
                {
                    if (i >= 0 && j >= 0 && i < lbm.Width && j < lbm.Height)
                    {
                        rColor.Add(lbm.GetPixel(i, j).R);
                        gColor.Add(lbm.GetPixel(i, j).G);
                        bColor.Add(lbm.GetPixel(i, j).B);
                        aColor.Add(lbm.GetPixel(i, j).A);
                    }
                    else
                    {
                        rColor.Add(0);
                        gColor.Add(0);
                        bColor.Add(0);
                        aColor.Add(0);
                    }
                }
            }
            int mid = (size * size - 1) / 2;
            rColor.Sort(); gColor.Sort(); bColor.Sort(); aColor.Sort();
            return (lbm.Depth == 32 ? Color.FromArgb(aColor[mid], rColor[mid], gColor[mid], bColor[mid]) : Color.FromArgb(rColor[mid], gColor[mid], bColor[mid]));
        }

        private Color GetSharp(LockBitmap lbm, int x, int y, int size = 3)
        {
            if (size % 2 == 0) throw new ArgumentException("Convolution size must is odd number");
            // index of CONV_SHARP matrix
            int index = 0;
            int R = 0, G = 0, B = 0, A = 0;
            for (int i = x - size / 2; i <= x + size / 2; i++)
            {
                for (int j = y - size / 2; j <= y + size / 2; j++)
                {
                    if (i >= 0 && j >= 0 && i < lbm.Width && j < lbm.Height)
                    {
                        A += (int)lbm.GetPixel(i, j).A * CONV_SHARP[index];
                        R += (int)lbm.GetPixel(i, j).R * CONV_SHARP[index];
                        G += (int)lbm.GetPixel(i, j).G * CONV_SHARP[index];
                        B += (int)lbm.GetPixel(i, j).B * CONV_SHARP[index];
                    }
                    index++;
                }
            }

            A = (A < 0 ? 0 : (A > 255 ? 255 : A));
            R = (R < 0 ? 0 : (R > 255 ? 255 : R));
            G = (G < 0 ? 0 : (G > 255 ? 255 : G));
            B = (B < 0 ? 0 : (B > 255 ? 255 : B));

            return (lbm.Depth == 32 ? Color.FromArgb(A, R, G, B) : Color.FromArgb(R, G, B));
        }

        private Bitmap GetResult(Bitmap bmp, int type)
        {
            imageOutput = new Bitmap(imageInput.Width, imageInput.Height, imageInput.PixelFormat);

            LockBitmap lockBitmapIn = new LockBitmap(imageInput);
            lockBitmapIn.LockBits();

            LockBitmap lockBitmapOut = new LockBitmap(imageOutput);
            lockBitmapOut.LockBits();

            int width = lockBitmapIn.Width;
            int height = lockBitmapIn.Height;

            switch (type)
            {
                case 1:
                    for(int i = 0; i < width; i++)
                    {
                        for(int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetAvg(lockBitmapIn, i, j, CONV_SIZE));
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetMedian(lockBitmapIn, i, j , CONV_SIZE));
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetSharp(lockBitmapIn, i, j, CONV_SIZE));
                        }
                    }
                    break;
                default:
                    MessageBox.Show("Selection filter option, pls!", "Oops!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lockBitmapIn.UnlockBits();
                    lockBitmapOut.UnlockBits();
                    return null;
            }
            lockBitmapIn.UnlockBits();
            lockBitmapOut.UnlockBits();

            return imageOutput;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            Bitmap result = GetResult((Bitmap)picInput.Image, cmbType.SelectedIndex);
            if(imageInput != null && result != null)
            {
                picOutput.Image = result;
                picOutput.Refresh();
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            cmbType.SelectedIndex = 0;
        }

        private void picInput_MouseHover(object sender, EventArgs e)
        {
            if (imageInput != null)
            {
                string s = "Display size: " + picInput.Width + " x " + picInput.Height +
                "\nBit depth: " + imageInput.PixelFormat +
                "\nDimensions: " + imageInput.Width + " x " + imageInput.Height +
                "\nBit depth: " + Bitmap.GetPixelFormatSize(imageInput.PixelFormat);
                ttImageInfo.Show(s, picInput);
            }
                 
        }

        private void menu1_Click(object sender, EventArgs e)
        {
            if(picOutput.Image != null)
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.DefaultExt = "jpg";
                    sfd.Filter = "Image Files (*.jpg; *.jpeg; *.gif; *.bmp, *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png|All files (*.*)|*.*";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        imageOutput.Save(sfd.FileName);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
