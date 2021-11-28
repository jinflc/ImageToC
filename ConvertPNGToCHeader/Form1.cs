using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvertPNGToCHeader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image file (*.png;*.bmp;*.jpg) | *.png;*.bmp;*.jpg";
            if(dlg.ShowDialog() ==  DialogResult.OK)
            {
                string filepath = dlg.FileName;
                textBox1.Text = filepath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = textBox1.Text;
            if(!File.Exists(filename))
            {
                MessageBox.Show("can't read file.");
                return;
            }
            string fileonlyname = Path.GetFileNameWithoutExtension(filename);
            fileonlyname = fileonlyname.Replace(" ", "_");
            string outputfilename = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + ".h";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#include <stdlib.h>");

            Bitmap bmp = new Bitmap(filename);
            sb.AppendLine($"const unsigned char {Path.GetFileNameWithoutExtension(filename)}logo_{bmp.Width}_{bmp.Height}[] = " + "{");
            Bitmap bmp565 = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format16bppRgb565);
            bmp565.Save("temp.bmp", ImageFormat.Bmp);

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            IntPtr ptr = bmpData.Scan0;
            byte[] bufs = File.ReadAllBytes("temp.bmp");

            int cnt = 0;
            foreach (var b in bufs)
            {
                string szs = $"0x{b:X2}, ";
                sb.Append(szs);
                if (cnt > 0 && cnt % 40 == 0) { sb.AppendLine(""); }
                cnt++;
            }
            sb = sb.Remove(sb.Length - 1, 1);             
            sb.AppendLine("};");
            File.WriteAllText(outputfilename, sb.ToString());
            MessageBox.Show("Commpleted!");

        }
    }
}
