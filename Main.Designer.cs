using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
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
                    Image imageGif = Image.FromFile("Fountain.gif");
                    loadingBox.Image = imageGif;
                    string filePath = openFileDialog.FileName;
                    image = null;
                    originalImage = null;
                    completedLoading = false;
                    //loadingWorker.RunWorkerAsync();
                    LoadPPM3(filePath);
                    completedLoading = true;
                    if (image != null)
                    {
                        pictureBox.Width = image.Width;
                        pictureBox.Height = image.Height;
                        pictureBox.Image = image;
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
        private void DisplayLoadingGif(object sender, DoWorkEventArgs e)
        {

            Image imageGif = Image.FromFile("Fountain.gif");
            loadingBox.Invoke(new Action(() =>
            {
                loadingBox.Image = imageGif;
            }));
        }
        private void HideLoadingGif(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (completedLoading)
            {
                loadingBox.Visible = false;
                loadingBox.Enabled = false;
                loadingBox.Dispose();
                loadingBox.Invalidate();
            }
        }
        private void LoadPPM3(string filePath)
        {
            string nextLine = "";
            int positionInArrayIndex = 0;
            int xLoc = 0, yLoc = 0;
            try
            {
                char[] splitParams = { ' ', '\n' , '\t'};
                string[] fileData = new string[10];
                string r = "", g = "", b = "";
                int width = 1;
                int height = 1;
                Bitmap onCreatingBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                using (StreamReader reader = new StreamReader(filePath))
                {
                    bool startRegisterImage = false;
                    while ((nextLine = reader.ReadLine()) != null)
                    {
                        if (positionInArrayIndex >= 3 && fileData.Length == 10)
                        {
                            var tmpFileData = new string[fileData.Length];
                            for (int i = 0; i < fileData.Length; i++)
                            {
                                tmpFileData[i] = fileData[i];
                            }
                            width = int.Parse(fileData[1]);
                            height = int.Parse(fileData[2]);
                            onCreatingBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                            fileData = new string[width * height * 3 + 4];
                            int tmpIndex = 0;
                            for (int i = 0; i < tmpFileData.Length; i++)
                            {
                                if (i > 2 && !String.IsNullOrEmpty(tmpFileData[i]))
                                {
                                    fileData[tmpIndex] = tmpFileData[i];
                                    tmpIndex++;
                                    if (r == "")
                                    {
                                        r = tmpFileData[i];
                                    }
                                    else if (g == "")
                                    {
                                        g = tmpFileData[i];
                                    }
                                    else if (b == "")
                                    {
                                        b = tmpFileData[i];
                                    }
                                    else if (r != "" && g != "" && b != "")
                                    {
                                        if (xLoc < width)
                                        {
                                            onCreatingBitmap.SetPixel(xLoc, yLoc, Color.FromArgb(int.Parse(r), int.Parse(g), int.Parse(b)));
                                        }
                                        else if (xLoc == width)
                                        {
                                            xLoc = 0;
                                            onCreatingBitmap.SetPixel(xLoc, yLoc, Color.FromArgb(int.Parse(r), int.Parse(g), int.Parse(b)));
                                            yLoc++;
                                        }
                                        r = ""; g = ""; b = "";
                                    }
                                }
                            }
                            startRegisterImage = true;
                        }
                        if (nextLine.Contains('#'))
                        {
                            string validLine = "";
                            for (int i = 0; i < nextLine.Length; i++)
                            {
                                if (nextLine[i].Equals('#'))
                                {
                                    break;
                                }
                                else
                                {
                                    validLine += nextLine[i];
                                }
                            }
                            var splittedValidLine = validLine.Split(splitParams, StringSplitOptions.RemoveEmptyEntries);
                            if (splittedValidLine.Length != 0 && !String.IsNullOrEmpty(splittedValidLine[0]))
                            {
                                for (int i = 0; i < splittedValidLine.Length; i++)
                                {
                                    if (startRegisterImage)
                                    {
                                        if (r == "")
                                        {
                                            r = splittedValidLine[i];
                                        }
                                        else if (g == "")
                                        {
                                            g = splittedValidLine[i];
                                        }
                                        else if (b == "")
                                        {
                                            b = splittedValidLine[i];
                                        }
                                        else if (r != "" && g != "" && b != "")
                                        {
                                            if (xLoc < width)
                                            {
                                                onCreatingBitmap.SetPixel(xLoc, yLoc, Color.FromArgb(int.Parse(r), int.Parse(g), int.Parse(b)));
                                                xLoc++;
                                                if (xLoc == 5999)
                                                {

                                                }
                                            }
                                            else if (xLoc == width)
                                            {
                                                xLoc = 0;
                                                onCreatingBitmap.SetPixel(xLoc, yLoc, Color.FromArgb(int.Parse(r), int.Parse(g), int.Parse(b)));
                                                yLoc++;
                                            }
                                            r = ""; g = ""; b = "";
                                        }
                                    }
                                    else {
                                        fileData[positionInArrayIndex] = splittedValidLine[i];
                                        positionInArrayIndex++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var splittedValidLine = nextLine.Split(splitParams, StringSplitOptions.RemoveEmptyEntries);
                            if (splittedValidLine.Length != 0 && !String.IsNullOrEmpty(splittedValidLine[0]))
                            {
                                for (int i = 0; i < splittedValidLine.Length; i++)
                                {
                                    if (startRegisterImage)
                                    {
                                        if (r == "")
                                        {
                                            r = splittedValidLine[i];
                                        }
                                        else if (g == "")
                                        {
                                            g = splittedValidLine[i];
                                        }
                                        else if (b == "")
                                        {
                                            b = splittedValidLine[i];
                                        }
                                        else if (r != "" && g != "" && b != "")
                                        {
                                            if (xLoc < width)
                                            {
                                                onCreatingBitmap.SetPixel(xLoc, yLoc, Color.FromArgb(int.Parse(r), int.Parse(g), int.Parse(b)));
                                                xLoc++;
                                                if (xLoc == 5999)
                                                {

                                                }
                                            }
                                            else if (xLoc == width)
                                            {
                                                xLoc = 0;
                                                onCreatingBitmap.SetPixel(xLoc, yLoc, Color.FromArgb(int.Parse(r), int.Parse(g), int.Parse(b)));
                                                yLoc++;
                                            }
                                            r = ""; g = ""; b = "";
                                        }
                                    }
                                    else
                                    {
                                        fileData[positionInArrayIndex] = splittedValidLine[i];
                                        positionInArrayIndex++;
                                    }
                                }
                            }
                        }
                    }
                }
                onCreatingBitmap.Save("output.bmp");
                image = onCreatingBitmap;
                originalImage = image;
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace + $", text: {nextLine} Index: {positionInArrayIndex} xLoc: {xLoc} yLoc: {yLoc}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void CreatePixel()
        {

        }
        private void LoadPPM(string filePath)
        {
            try
            {
                string[] splitParams = { " ", "\n" };
                string line = "";
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string nextLine;
                    while ((nextLine = reader.ReadLine()) != null)
                    {
                        if (!nextLine.Contains('#'))
                        {
                            line += nextLine + "\n";
                        }
                    }
                }
                string[] fileData = line.Split(splitParams, StringSplitOptions.RemoveEmptyEntries);
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
            int newWidth = (int)(image.Width * 5);
            int newHeight = (int)(image.Height * 5);

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
            if (originalImage == image)
            {
                MessageBox.Show("Image already has default zoom!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
