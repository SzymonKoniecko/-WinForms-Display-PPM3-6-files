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
        int xLoc = 0, yLoc = 0;
        string r = "", g = "", b = "";
        int width = 1;
        int height = 1;
        string filePath = "";
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (image == null) return;

            int x = e.X * image.Width / pictureBox.Width;
            int y = e.Y * image.Height / pictureBox.Height;

            Color pixelColor = ((Bitmap)image).GetPixel(x, y);
            pixelInfoLabel.Text = $"Piksel ({x}, {y}) - R:{pixelColor.R}, G:{pixelColor.G}, B:{pixelColor.B}";
        }
        private void ChoosePpmFile(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Pliki PPM P3 (*.ppm)|*.ppm";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    loadingBox.Visible = true;
                    filePath = openFileDialog.FileName;
                    image = null;
                    originalImage = null;
                    loadingWorker.RunWorkerAsync();
                }
            }
        }
        private void StartLoadImage(object sender, DoWorkEventArgs e)
        {
            if (filePath.Contains("-p3"))
            {
                LoadPPM3(filePath);
            }
            else if (filePath.Contains("-p6"))
            {
                LoadPPM6(filePath);
            }
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Error on load image by thread" + e.Error.Message);
            }
            else
            {
                if (image != null)
                {
                    loadingBox.Visible = false;
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

        private void ChooseJPEGFile(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        private void SaveJPEGFile(object? sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "JPEG Image|*.jpg";
                if (originalImage == null)
                {
                    MessageBox.Show("Firstly the picture has to be generated!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!highQCompressionRadioBtn.Checked && !goodQCompressionRadioBtn.Checked && !lowQCompressionRadioBtn.Checked)
                {
                    MessageBox.Show("Select the type of compression!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = dialog.FileName;

                    try
                    {
                        using (Bitmap bitmap = new Bitmap(originalImage))
                        {
                            ImageCodecInfo jpegCodec;
                            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                            {
                                if (codec.MimeType == "image/jpeg")
                                {
                                    jpegCodec = codec;
                                    EncoderParameters encoderParams = new EncoderParameters(1);
                                    if (highQCompressionRadioBtn.Checked)
                                    {
                                        encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, 90);
                                        bitmap.Save(filePath, jpegCodec, encoderParams);
                                    }
                                    else if (goodQCompressionRadioBtn.Checked)
                                    {
                                        encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, 70);
                                        bitmap.Save(filePath, jpegCodec, encoderParams);
                                    }
                                    else if (lowQCompressionRadioBtn.Checked)
                                    {
                                        encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, 40);
                                        bitmap.Save(filePath, jpegCodec, encoderParams);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Cannot save JPEG: " + ex.Message + ex.StackTrace);
                    }
                }
            }
        }
        private void LoadPPM3(string filePath)
        {
            string nextLine = "";
            int positionInArrayIndex = 0;
            xLoc = 0; yLoc = 0;
            r = ""; g = ""; b = "";
            width = 1;
            height = 1;
            try
            {
                char[] splitParams = { ' ', '\n' , '\t'};
                string[] fileData = new string[10];
                onCreatingBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
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
                                    CreatePixel(tmpFileData[i], width, height, ref r, ref g, ref b, ref xLoc, ref yLoc);
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
                                        CreatePixel(splittedValidLine[i], width, height, ref r, ref g, ref b, ref xLoc, ref yLoc);
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
                                        CreatePixel(splittedValidLine[i], width, height, ref r, ref g, ref b, ref xLoc, ref yLoc);
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
                onCreatingBitmap.Save("output.bitmap");
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
        private void LoadPPM6(string filePath)
        {
            throw new NotImplementedException();
        }
        private void CreatePixel(string value, int width, int height, ref string r, ref string g, ref string b, ref int xLoc, ref int yLoc)
        {
            if (value.Equals("255") && !foundedIdBit)
            {
                foundedIdBit = true;
            }
            else
            {
                if (r == "")
                {
                    r = value;
                }
                else if (g == "")
                {
                    g = value;
                }
                else if (b == "")
                {
                    b = value;
                    onCreatingBitmap.SetPixel(xLoc, yLoc, Color.FromArgb(int.Parse(r), int.Parse(g), int.Parse(b)));
                    xLoc++;
                    if (xLoc >= width)
                    {
                        xLoc = 0;
                        yLoc++;
                    }
                    r = ""; g = ""; b = "";
                }
            }
        }
        private void ZoomIn(object? sender, EventArgs e)
        {
            if (image == null) return;
            if (originalImage == null)
            {
                originalImage = image;
            }
            int newWidth = (int)(image.Width * 2);
            int newHeight = (int)(image.Height * 2);
            if (newWidth > 40000 || newHeight > 30000)
            {
                MessageBox.Show("Maximum zoom has been reached!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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