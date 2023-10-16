using System.ComponentModel;

namespace Zad2
{
    public partial class Main : Form
    {
        private Button openFileBtn; // Przycisk do otwierania pliku
        private PictureBox pictureBox; // Pole do wyświetlania obrazu
        private Button zoomInBtn;
        private Button zoomOutBtn;
        private Button zoomBackToNormalBtn;
        private Label pixelInfoLabel;
        private Bitmap image;
        private Bitmap originalImage;
        private Bitmap onCreatingBitmap;
        private PictureBox loadingBox;
        private BackgroundWorker loadingWorker;
        private bool completedLoading = false;
        public Main()
        {
            this.Size = new Size(1200, 720);
            // Inicjalizacja PictureBox

            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.MouseMove += PictureBox_MouseMove;
            loadingBox = new PictureBox();
            Image imageGif = Image.FromFile("Fountain.gif");
            loadingBox.Dock = DockStyle.Bottom;
            loadingBox.SizeMode = PictureBoxSizeMode.CenterImage;
            loadingBox.Visible = true;
            loadingBox.Enabled = true;
            // Inicjalizacja TrackBar do zmiany powiększenia
            zoomInBtn = new Button();
            zoomInBtn.Dock = DockStyle.Bottom;
            zoomInBtn.Text = "Zoom IN";
            zoomOutBtn = new Button();
            zoomOutBtn.Dock = DockStyle.Bottom;
            zoomOutBtn.Text = "zoom OUT";
            zoomInBtn.Click += ZoomIn;
            zoomOutBtn.Click += ZoomOut;
            zoomBackToNormalBtn = new Button();
            zoomBackToNormalBtn.Dock = DockStyle.Bottom;
            zoomBackToNormalBtn.Text = "Back to default zoom of picture";
            zoomBackToNormalBtn.Click += ZoomBackToNormal;
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
            this.DoubleBuffered = true;
            // Inicjalizacja przycisku "Wybierz plik"
            openFileBtn = new Button();
            openFileBtn.Text = "Wybierz plik";
            openFileBtn.Click += openFileButton_Click;
            openFileBtn.Dock = DockStyle.Top;
            this.Controls.Add(openFileBtn);

            loadingWorker = new BackgroundWorker();
            loadingWorker.WorkerReportsProgress = false;
            loadingWorker.WorkerSupportsCancellation = false;
            loadingWorker.DoWork += new DoWorkEventHandler(DisplayLoadingGif);
            loadingWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HideLoadingGif);
        }

    }
}
