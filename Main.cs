using System.ComponentModel;

namespace Zad2
{
    public partial class Main : Form
    {
        private Button openPpmFileBtn;
        private PictureBox pictureBox;
        private Button zoomInBtn;
        private Button zoomOutBtn;
        private Button zoomBackToNormalBtn;
        private Label pixelInfoLabel;
        private Bitmap image;
        private Bitmap originalImage;
        private Bitmap onCreatingBitmap;
        private PictureBox loadingBox;
        private BackgroundWorker loadingWorker;
        private bool foundedIdBit = false;
        private Button openJPEGFile;
        private Button saveJPEGFile;

        private RadioButton highQCompressionRadioBtn;
        private RadioButton goodQCompressionRadioBtn;
        private RadioButton lowQCompressionRadioBtn;
        public Main()
        {
            this.Size = new Size(1200, 720);
            // Inicjalizacja PictureBox

            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox.MouseMove += PictureBox_MouseMove;
            loadingBox = new PictureBox();
            Image imageGif = Image.FromFile("Fountain.gif");
            loadingBox.Image = imageGif;
            loadingBox.Dock = DockStyle.Bottom;
            loadingBox.SizeMode = PictureBoxSizeMode.CenterImage;
            loadingBox.Visible = false;
            loadingBox.Enabled = true;
            // Inicjalizacja TrackBar do zmiany powiększenia
            zoomInBtn = new Button();
            zoomInBtn.Dock = DockStyle.Bottom;
            zoomInBtn.Text = "Zoom IN";
            zoomOutBtn = new Button();
            zoomOutBtn.Dock = DockStyle.Bottom;
            zoomOutBtn.Text = "Zoom OUT";
            zoomInBtn.Click += ZoomIn;
            zoomOutBtn.Click += ZoomOut;
            zoomBackToNormalBtn = new Button();
            zoomBackToNormalBtn.Dock = DockStyle.Bottom;
            zoomBackToNormalBtn.Text = "Back to default zoom of picture";
            zoomBackToNormalBtn.Click += ZoomBackToNormal;

            highQCompressionRadioBtn = new RadioButton();
            highQCompressionRadioBtn.Text = "High Quality Compression";
            highQCompressionRadioBtn.Dock = DockStyle.Top;
            goodQCompressionRadioBtn = new RadioButton();
            goodQCompressionRadioBtn.Text = "Good Quality Compression";
            goodQCompressionRadioBtn.Dock = DockStyle.Top;
            lowQCompressionRadioBtn = new RadioButton();
            lowQCompressionRadioBtn.Text = "Low Quality Compression";
            lowQCompressionRadioBtn.Dock = DockStyle.Top;

            openJPEGFile = new Button();
            openJPEGFile.Dock = DockStyle.Top;
            openJPEGFile.Text = "Load JPEG file";
            openJPEGFile.Click += ChooseJPEGFile;

            saveJPEGFile = new Button();
            saveJPEGFile.Dock = DockStyle.Top;
            saveJPEGFile.Text = "Save JPEG file";
            saveJPEGFile.Click += SaveJPEGFile;


            // Inicjalizacja Label do wyświetlania informacji o pikselu
            pixelInfoLabel = new Label();
            pixelInfoLabel.Dock = DockStyle.Bottom;
            pixelInfoLabel.TextAlign = ContentAlignment.MiddleCenter;
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox.Visible = true;
            pictureBox.Enabled = true;
            // Dodanie kontrolki PictureBox do formularza
            this.Controls.Add(pictureBox);
            this.Controls.Add(zoomInBtn);
            this.Controls.Add(zoomOutBtn);
            this.Controls.Add(zoomBackToNormalBtn);
            this.Controls.Add(pixelInfoLabel);
            this.Controls.Add(loadingBox);
            this.Controls.Add(openJPEGFile);
            this.Controls.Add(saveJPEGFile);
            this.Controls.Add(highQCompressionRadioBtn);
            this.Controls.Add(goodQCompressionRadioBtn);
            this.Controls.Add(lowQCompressionRadioBtn);
            this.DoubleBuffered = true;
            // Inicjalizacja przycisku "Wybierz plik"
            openPpmFileBtn = new Button();
            openPpmFileBtn.Text = "Choose ppm file";
            openPpmFileBtn.Click += ChoosePpmFile;
            openPpmFileBtn.Dock = DockStyle.Top;
            this.Controls.Add(openPpmFileBtn);

            loadingWorker = new BackgroundWorker();
            loadingWorker.WorkerReportsProgress = false;
            loadingWorker.WorkerSupportsCancellation = false;
            loadingWorker.DoWork += new DoWorkEventHandler(StartLoadImage);
            loadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
        }


    }
}
