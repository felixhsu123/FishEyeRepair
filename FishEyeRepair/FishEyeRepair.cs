using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace FishEyeRepair
{
    public partial class FishEyeRepair : Form
    {
        public FishEyeRepair()
        {
            InitializeComponent();
        }

        private void buttonLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog lvse = new OpenFileDialog())
            {
                lvse.Title = "Select a Picture file";
                lvse.InitialDirectory = "";
                lvse.Filter = "Type|*.bmp;*.jpg;*.jpeg;*.gif;*.png";
                lvse.FilterIndex = 1;

                if (lvse.ShowDialog() == DialogResult.OK)
                {
                    //TEST CODE BEGIN HERE
                    //init imageBefore and display it in the pictureBoxBefore
                    Bitmap imageBefore = new Bitmap(lvse.FileName, true);
                    pictureBoxBefore.Image = imageBefore;
                    pictureBoxBefore.Refresh();
                    //System.Threading.Thread.Sleep(1 * 1000);

                    //begin convert
                    Bitmap imageAfter = new Bitmap((Image)imageBefore.Clone());
                    imageAfter = convertToFishEyeType(imageAfter);
                    pictureBoxAfter.Image = imageAfter;
                    pictureBoxAfter.Refresh();
                    //保存变换后的图片
                    //pictureBoxAfter.Image.Save("after_fisheye.bmp");
                    //System.Threading.Thread.Sleep(1 * 1000);
                }
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string htmInfo = @"D:\C-Sharp\FishEyeRepair\index.htm";
            Process.Start("iexplore.exe", htmInfo);
        }

        private Bitmap convertToFishEyeType(Bitmap inputBitmap)
        {
            int width, height;
            const float OFFSET_X1 = 20.0f;
            const float OFFSET_Y1 = 10.0f;
            int w = inputBitmap.Width;
            int h = inputBitmap.Height;
            //R和detaR可调，会得到不同的效果，当R /= 2.8且detaR = 0.0f * R时，可以得到比较接近于圆的效果，但是图片有效区降低
            //目前来看detaR的值变化不受欢迎
            float R = ((w >= h) ? (w / 2) : (h / 2));
            R /= 1.20f;
            float detaR = 0.0f * R;
            float aw = 0.0f, ah = 0.0f;
            float awt2 = 0.0f, aht2 = 0.0f;
            float awt1 = 0.0f, aht1 = 0.0f;

            Bitmap outputBitmap = new Bitmap((Image)inputBitmap.Clone());
            //init outputBitmap: set all pixels to black color
            for (width = 0; width < outputBitmap.Width; width++)
            {
                for (height = 0; height < outputBitmap.Height; height++)
                {
                    //Color pixelColor = imageAfter.GetPixel(x, y);      //read specified pixels
                    Color blackColor = Color.FromArgb(0, 0, 0);
                    outputBitmap.SetPixel(width, height, blackColor);    //write specified pixels
                }
            }
            //convert
#if false 
            for (height = 0; height < outputBitmap.Height; height++)//bh
            {
                for (width = 0; width < outputBitmap.Width; width++)//bw
                {
                    awt1 = width - w / 2;
                    aht1 = height - h / 2;

                    float temp = (float)(Math.Sqrt((float)Math.Pow(awt1, 2) + (float)Math.Pow(aht1, 2) + (float)Math.Pow((R + detaR), 2)));
                    awt2 = (awt1 * R) / temp;
                    aht2 = (aht1 * R) / temp;
                    aw = awt2 + w / 2;
                    ah = aht2 + h / 2;
                    Color newColor = inputBitmap.GetPixel(width, height);
                    outputBitmap.SetPixel((int)aw, (int)ah, newColor);
                }
            }
#else
            for (height = 0; height < outputBitmap.Height; height++)//bh
            {
                for (width = 0; width < outputBitmap.Width; width++)//bw
                {
                    awt1 = width - w / 2 - OFFSET_X1;
                    aht1 = height - h / 2 - OFFSET_Y1;

                    float temp = (float)(Math.Sqrt((float)Math.Pow(awt1, 2) + (float)Math.Pow(aht1, 2) + (float)Math.Pow((R + detaR), 2)));
                    awt2 = (awt1 * R) / temp;
                    aht2 = (aht1 * R) / temp;

                    /*增大视场大小*/
                    //awt2 /= 0.85f;
                    //aht2 /= 0.85f;

                    aw = awt2 + w / 2 + OFFSET_X1;
                    ah = aht2 + h / 2 + OFFSET_Y1;

                    Color newColor = inputBitmap.GetPixel((int)aw, (int)ah);
                    outputBitmap.SetPixel((int)width, (int)height, newColor);
                }
            }
#endif
            return outputBitmap;
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {

        }

    }
}
