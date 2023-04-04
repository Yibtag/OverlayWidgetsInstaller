using IWshRuntimeLibrary;
using System.ComponentModel;
using System.Net;
using System.IO;

namespace OverlayWidgetsInstaller
{
    public partial class Form1 : Form
    {
        public String path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\OverlayWidgets\\";
        public String startmenu_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\";

        Button path_button;

        CheckBox create_startmenu;
        Button startmenu_button;

        CheckBox create_desktop;

        ProgressBar progressBar;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            path_button = this.Controls["button1"] as Button;
            path_button.Text = path;

            startmenu_button = this.Controls["button2"] as Button;
            startmenu_button.Text = startmenu_path;

            progressBar = this.Controls["progressBar1"] as ProgressBar;

            create_startmenu = this.Controls["checkBox1"] as CheckBox;
            create_desktop = this.Controls["checkBox2"] as CheckBox;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            folderDialog.SelectedPath = path;

            DialogResult result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                path = folderDialog.SelectedPath;
                path_button.Text = path;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                startmenu_button.Enabled = true;
            }
            else
            {
                startmenu_button.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            folderDialog.SelectedPath = startmenu_path;

            DialogResult result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                startmenu_path = folderDialog.SelectedPath;
                startmenu_button.Text = startmenu_path;
            }
        }

        public void createShortcut(string create_path)
        {
            WshShell shell = new WshShell();
            string shortcutAddress = create_path + "OverlayWidgets.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "OverlayWidgets";
            shortcut.TargetPath = path + "\\OverlayWidgets.exe";
            shortcut.Save();

            using (var fs = new FileStream(create_path + "OverlayWidgets.lnk", FileMode.Open, FileAccess.ReadWrite))
            {
                fs.Seek(21, SeekOrigin.Begin);
                fs.WriteByte(0x22);
            }
        }

        private void startDownload()
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileAsync(new Uri("https://github.com/Yibtag/OverlayWidgets/releases/download/Relese/OverlayWidgets.exe"), path + "\\OverlayWidgets.exe");
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!Directory.Exists(path + "\\plugins\\"))
            {
                Directory.CreateDirectory(path + "\\plugins\\");
            }

            startDownload();

            if (create_startmenu.Checked)
            {
                createShortcut(startmenu_path);
            }

            if (create_desktop.Checked)
            {
                createShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            }
        }
    }
}