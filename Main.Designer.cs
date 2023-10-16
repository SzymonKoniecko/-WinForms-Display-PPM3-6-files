using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Zad2
{
    partial class Main
    {
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (image == null) return;

            int x = e.X * image.Width / pictureBox.Width;
            int y = e.Y * image.Height / pictureBox.Height;

            Color pixelColor = ((Bitmap)image).GetPixel(x, y);
            pixelInfoLabel.Text = $"Piksel ({x}, {y}) - R:{pixelColor.R}, G:{pixelColor.G}, B:{pixelColor.B}";
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Pliki PPM P3 (*.ppm)|*.ppm";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    LoadPPM(filePath);

                    if (image != null)
                    {
                        pictureBox.Image = image;
                        pictureBox.Width = image.Width;
                        pictureBox.Height = image.Height;
                        pictureBox.Dock = DockStyle.Fill;
                        pictureBox.Invalidate();
                        this.Invalidate();
                    }
                    else
                    {
                        MessageBox.Show("Cannot load PPM P3 file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadPPM(string filePath)
        {
            try
            {
                string[] splitParams = { " ", "\n" };
                string[] fileData = File.ReadAllText(filePath).Split(splitParams, StringSplitOptions.RemoveEmptyEntries);
                int width = int.Parse(fileData[1]);
                int height = int.Parse(fileData[2]);
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                int index = 3;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color = Color.FromArgb(255, int.Parse(fileData[index]), int.Parse(fileData[index + 1]), int.Parse(fileData[index + 2]));
                        index += 3;
                        bitmap.SetPixel(x, y, color);
                    }
                }
                bitmap.Save("output.bmp");
                image = bitmap;
                originalImage = image;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void ZoomIn(object? sender, EventArgs e)
        {
            if (image == null) return;
            if (originalImage == null)
            {
                originalImage = image;
            }
            int newWidth = (int)(image.Width * 1.2);
            int newHeight = (int)(image.Height * 1.2);

            image = new Bitmap(image, new Size(newWidth, newHeight));
            image.Save($"zoomIN{newWidth}-{newHeight}.bmp");
            pictureBox.Width = newWidth;
            pictureBox.Height = newHeight;
            try
            {
                pictureBox.Image = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd: " + ex.Message);
            }
            this.Invalidate();
        }
        private void ZoomOut(object? sender, EventArgs e)
        {
            if (image == null) return;
            if (originalImage == null)
            {
                originalImage = image;
            }
            int newWidth = (int)(image.Width * 0.8);
            int newHeight = (int)(image.Height * 0.8);
            if (newWidth < 1 || newHeight < 1)
            {
                MessageBox.Show("Minimum zoom has been reached!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            image = new Bitmap(image, new Size(newWidth, newHeight));
            pictureBox.Width = newWidth;
            pictureBox.Height = newHeight;
            try
            {
                pictureBox.Image = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            this.Invalidate();
        }
        private void ZoomBackToNormal(object? sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Cannot back to default picture zoom!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            image = new Bitmap(originalImage, new Size(originalImage.Width, originalImage.Height));
            try
            {
                pictureBox.Width = originalImage.Width;
                pictureBox.Height = originalImage.Height;
                pictureBox.Image = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
