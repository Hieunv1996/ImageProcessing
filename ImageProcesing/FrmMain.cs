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

namespace ImageProcesing
{
    public partial class FrmMain : Form
    {
        // CONVOLUTION ARRAY
        const int CONV_SIZE_3_3 = 3;
        const int CONV_SIZE_5_5 = 5;
        const int CONV_SIZE_7_7 = 7;
        private int[] CONV_SHARP_3_3 = {    0, -1, 0,
                                            -1, 5, -1,
                                            0, -1, 0
                                        };
        private int[] CONV_SHARP_5_5 = {
                                          -1, -1, -1, -1, -1,
                                          -1,  2,  2,  2, -1,
                                          -1,  2,  8,  2, -1,
                                          -1,  2,  2,  2, -1,
                                          -1, -1, -1, -1, -1,
                                        };
        // Save image in Input image
        private Bitmap imageInput = null;
        private Bitmap imageOutput = null;
        private String imagePath = "";


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
                imagePath = open.FileName;
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
            if (size % 2 == 0) throw new ArgumentException("Convolution size must is odd number");
            int sumR = 0, sumG = 0, sumB = 0, sumA = 0;
            for (int i = x - size / 2; i <= x + size / 2; i++)
            {
                for (int j = y - size / 2; j <= y + size / 2; j++)
                {
                    if (i >= 0 && j >= 0 && i < lbm.Width && j < lbm.Height)
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

        private Color GetMin(LockBitmap lbm, int x, int y, int size = 3)
        {
            Color color = new Color();
            int minVal = 500000, tmp;

            if (size % 2 == 0) throw new ArgumentException("Convolution size must is odd number");
            for (int i = x - size / 2; i <= x + size / 2; i++)
            {
                for (int j = y - size / 2; j <= y + size / 2; j++)
                {
                    if (i >= 0 && j >= 0 && i < lbm.Width && j < lbm.Height)
                    {
                        tmp = lbm.GetPixel(i, j).R + lbm.GetPixel(i, j).G + lbm.GetPixel(i, j).B + lbm.GetPixel(i, j).A;
                        if(tmp < minVal)
                        {
                            minVal = tmp;
                            color = lbm.GetPixel(i, j);
                        }
                    }
                    else
                    {
                        
                    }
                }
            }
            return color;
        }

        private Color GetMax(LockBitmap lbm, int x, int y, int size = 3)
        {
            Color color = new Color();
            int maxVal = 0, tmp;

            if (size % 2 == 0) throw new ArgumentException("Convolution size must is odd number");
            for (int i = x - size / 2; i <= x + size / 2; i++)
            {
                for (int j = y - size / 2; j <= y + size / 2; j++)
                {
                    if (i >= 0 && j >= 0 && i < lbm.Width && j < lbm.Height)
                    {
                        tmp = lbm.GetPixel(i, j).R + lbm.GetPixel(i, j).G + lbm.GetPixel(i, j).B + lbm.GetPixel(i, j).A;
                        if (tmp > maxVal)
                        {
                            maxVal = tmp;
                            color = lbm.GetPixel(i, j);
                        }
                    }
                    else
                    {

                    }
                }
            }
            return color;
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
                        if(size == 3)
                        {
                            A += (int)lbm.GetPixel(i, j).A * CONV_SHARP_3_3[index];
                            R += (int)lbm.GetPixel(i, j).R * CONV_SHARP_3_3[index];
                            G += (int)lbm.GetPixel(i, j).G * CONV_SHARP_3_3[index];
                            B += (int)lbm.GetPixel(i, j).B * CONV_SHARP_3_3[index];
                        }else if(size == 5)
                        {
                            A += (int)lbm.GetPixel(i, j).A * CONV_SHARP_5_5[index];
                            R += (int)lbm.GetPixel(i, j).R * CONV_SHARP_5_5[index];
                            G += (int)lbm.GetPixel(i, j).G * CONV_SHARP_5_5[index];
                            B += (int)lbm.GetPixel(i, j).B * CONV_SHARP_5_5[index];
                        }
                        else
                        {
                            throw new System.ArgumentException("size cannot be" +size , "original");
                        }
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
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetAvg(lockBitmapIn, i, j, CONV_SIZE_3_3));
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetAvg(lockBitmapIn, i, j, CONV_SIZE_5_5));
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetMedian(lockBitmapIn, i, j, CONV_SIZE_3_3));
                        }
                    }
                    break;
                case 4:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetMedian(lockBitmapIn, i, j, CONV_SIZE_5_5));
                        }
                    }
                    break;
                case 5:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetSharp(lockBitmapIn, i, j, CONV_SIZE_3_3));
                        }
                    }
                    break;
                case 6:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetSharp(lockBitmapIn, i, j, CONV_SIZE_5_5));
                        }
                    }
                    break;
                case 7:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetMax(lockBitmapIn, i, j, CONV_SIZE_3_3));
                        }
                    }
                    break;
                case 8:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetMax(lockBitmapIn, i, j, CONV_SIZE_5_5));
                        }
                    }
                    break;
                case 9:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetMin(lockBitmapIn, i, j, CONV_SIZE_3_3));
                        }
                    }
                    break;
                case 10:
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            lockBitmapOut.SetPixel(i, j, GetMin(lockBitmapIn, i, j, CONV_SIZE_5_5));
                        }
                    }
                    break;
                default:
                    MessageBox.Show("Hãy chọn một bộ lọc!, pls");
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
            if (imageInput == null)
            {
                MessageBox.Show("Chọn ảnh trước, pls!!!");
                return;
            }
            Bitmap result = GetResult((Bitmap)picInput.Image, cmbType.SelectedIndex);
            if (imageInput != null && result != null)
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
                string s = "Dimensions: " + imageInput.Width + " x " + imageInput.Height +
                "\nBit depth: " + imageInput.PixelFormat +
                "\nSize: " + (new System.IO.FileInfo(imagePath).Length) + " Bytes" +
                "\nPath: " + imagePath;
                ttImageInfo.Show(s, picInput);
            }

        }

        private void menu1_Click(object sender, EventArgs e)
        {
            if (imageOutput != null)
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

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (picOutput.Image != null)
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.DefaultExt = "jpg";
                    sfd.Filter = "Image Files (*.jpg; *.jpeg; *.gif; *.bmp, *.png)|*.jpg; *.jpeg; *.gif; *.bmp; *.png|All files (*.*)|*.*";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        imageOutput.Save(sfd.FileName);
                        MessageBox.Show("Lưu thành công tại " + sfd.FileName);
                    }
                }
                catch
                {
                    MessageBox.Show("Lưu không thành công");
                }
            }
            else
            {
                MessageBox.Show("Không có ảnh để lưu, xử lí ảnh trước, pls!!!");
            }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (imageInput != null)
            {
                string s = "Dimensions: " + imageInput.Width + " x " + imageInput.Height +
                "\nBit depth: " + imageInput.PixelFormat +
                "\nSize: " + (new System.IO.FileInfo(imagePath).Length) + " Bytes" +
                "\nPath: " + imagePath;
                MessageBox.Show(s);
            }
            else
            {
                MessageBox.Show("Hãy chọn ảnh trước, pls!!!");
            }
        }


    }
}
