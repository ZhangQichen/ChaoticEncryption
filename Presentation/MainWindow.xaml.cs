using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using ChaoticEncryption;
using System.Windows.Forms;
using System.IO;

namespace FileEncryptor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum EncryptionWorkType
        {
            ENCRYPT, DECRYPT
        }

        class EncryptionWorker
        {
            Double r = 3.588;
            Double X0_ws = 0.566;
            String Ke = "ChaoticEncryptionSystemNcmWSzqch";
            Double X0_ncm = 0.666, a = 0.7, b = 28;
            String filepath; // Directory + fileName
            String SavePath; // Path of a folder
            EncryptionSystem encryptionSystem;
            EncryptionWorkType workType;

            public EncryptionWorker(EncryptionWorkType type,
                String inputPath, String outputPath,
                double pa, double pb, double px0_ncm, double pr, double px0_ws, String pke)
            {
                workType = type;
                filepath = inputPath;
                SavePath = outputPath;
                a = pa; b = pb; X0_ncm = px0_ncm; X0_ws = px0_ws; Ke = pke;
            }
            
            public void WorkAsync(BackgroundWorker worker, DoWorkEventArgs e)
            {
                worker.WorkerReportsProgress = true;
                worker.WorkerSupportsCancellation = true;

                ChaoticEncryption.Builder ceb = new ChaoticEncryption.Builder();
                ceb.SetNcmParams(a, b, X0_ncm);
                ceb.SetWheelSwitchParams(r, X0_ws, Encoding.UTF8.GetBytes(Ke));

                Byte[] buffer = new Byte[2048];
                Byte[] cipherBlock = null;
                FileStream inStream = null;
                FileStream outStream = null;
                BinaryReader binaryReader = null;
                BinaryWriter binaryWriter = null;
                int length = 0;
                FileInfo fi = new FileInfo(filepath);
                long totalSize = fi.Length;
                long completedSize = 0;
                try
                {
                    inStream = new FileStream(filepath, FileMode.Open);
                    binaryReader = new BinaryReader(inStream);

                    if (workType == EncryptionWorkType.DECRYPT)
                    {
                        // Set FileName
                        // Read byteCount of filename
                        String SaveName; // Filename to save
                        int filenameLength = binaryReader.ReadInt32();
                        SaveName = Encoding.UTF8.GetString(binaryReader.ReadBytes(filenameLength));
                        SavePath = String.Concat(SavePath, '\\', SaveName);
                        fi = new FileInfo(SavePath);
                        if (fi.Exists)
                        {
                            SavePath = String.Concat(SavePath.Remove(SavePath.LastIndexOf('.')), "_Copy", SavePath.Substring(SavePath.LastIndexOf('.')));
                        }
                    }

                    outStream = new FileStream(SavePath, FileMode.OpenOrCreate);
                    binaryWriter = new BinaryWriter(outStream);

                    if (workType == EncryptionWorkType.ENCRYPT)
                    {
                        // Write fileName in the output. First 4 Byte(int) is the byteCount of fileName. Then fileName in Bytes follows.
                        String filename;
                        filename = filepath.Substring(filepath.LastIndexOf('\\') + 1);
                        Byte[] fileNameInBytes = Encoding.UTF8.GetBytes(filename);
                        int size = fileNameInBytes.Length;
                        binaryWriter.Write(size);
                        binaryWriter.Write(fileNameInBytes);
                    }

                    while ((length = binaryReader.Read(buffer, 0, 2048)) > 0)
                    {
                        encryptionSystem = ceb.CreateSystem(buffer);
                        if (workType == EncryptionWorkType.ENCRYPT)
                            encryptionSystem.Encrypt(ref buffer, out cipherBlock);
                        else
                            encryptionSystem.Decrypt(ref buffer, out cipherBlock);
                        binaryWriter.Write(cipherBlock, 0, length);
                        completedSize += length;
                        worker.ReportProgress((int)(100 * completedSize / totalSize));
                    }
                }
                catch (Exception exception)
                {
                    System.Windows.MessageBox.Show("Error occurs when reading file!\n" + exception.Message);
                    worker.CancelAsync();
                }
                if (binaryReader != null) binaryReader.Close();
                if (inStream != null) inStream.Close();
                if (binaryWriter != null) binaryWriter.Close();
                if (outStream != null) outStream.Close();

                worker.ReportProgress(100);
            }
        }
         
        Double r = 3.588;
        Double X0_ws = 0.566;
        String Ke = "ChaoticEncryptionSystemNcmWSzqch";
        Double X0_ncm = 0.666, a = 0.7, b = 28;
        String inputPath; // Directory + fileName
        String outputPath; // Path of a folder
        BackgroundWorker BGWorker;
        
        public static String FILE_EXTENSION = ".cef";
        
        public MainWindow()
        {
            InitializeComponent();
            FillParams();
        }

        private void LockUI()
        {
            this.IsEnabled = false;
        }

        private void UnLockUI()
        {
            this.IsEnabled = true;
        }

        private void StartEncryptionThread()
        {
            this.m_ProgressBar.Value = 0.0;
            this.m_ProgressPercentage.Content = "0%";
            // This method runs on the main thread.
            LockUI();
            EncryptionWorker eworker = new EncryptionWorker(EncryptionWorkType.ENCRYPT, inputPath, outputPath, a, b, X0_ncm, r, X0_ws, Ke);
            // Start the asynchronous operation.
            BGWorker = new BackgroundWorker();
            BGWorker.DoWork += BGWorker_DoWork;
            BGWorker.ProgressChanged += BGWorker_ProgressChanged;
            BGWorker.RunWorkerCompleted += BGWorker_RunWorkerCompleted;
            BGWorker.RunWorkerAsync(eworker);
        }

        private void StartDecryptionThread()
        {
            this.m_ProgressBar.Value = 0.0;
            this.m_ProgressPercentage.Content = "0%";
            // This method runs on the main thread.
            LockUI();
            EncryptionWorker dworker = new EncryptionWorker(EncryptionWorkType.DECRYPT, inputPath, outputPath, a, b, X0_ncm, r, X0_ws, Ke);
            // Start the asynchronous operation.
            BackgroundWorker BGWorker = new BackgroundWorker();
            BGWorker.DoWork += BGWorker_DoWork;
            BGWorker.ProgressChanged += BGWorker_ProgressChanged;
            BGWorker.RunWorkerCompleted += BGWorker_RunWorkerCompleted;
            BGWorker.RunWorkerAsync(dworker);
        }

        private void BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // This event handler is where the actual work is done.
            // This method runs on the background thread.
            // Get the BackgroundWorker object that raised this event.
            System.ComponentModel.BackgroundWorker worker;
            worker = (System.ComponentModel.BackgroundWorker)sender;

            // Get the Words object and call the main method.
            EncryptionWorker ew = (EncryptionWorker)e.Argument;
            ew.WorkAsync(worker, e);
        }

        private void BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                System.Windows.MessageBox.Show(e.Error.Message);
            else if (e.Cancelled)
            {
                System.Windows.MessageBox.Show("Procedure Cancelled!");
                this.m_ProgressPercentage.Content = "Cancelled";
            }
            else
            {
                System.Windows.MessageBox.Show("Finished!");
                this.m_ProgressPercentage.Content = "100%";
            }
            UnLockUI();
        }

        private void BGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This method runs on the main thread.
            this.m_ProgressBar.Value = ((double)e.ProgressPercentage);
            this.m_ProgressPercentage.Content = this.m_ProgressBar.Value + "%";
        }

        private void FillParams()
        {
            this.TextBox_NCM_a.Text = a.ToString();
            this.TextBox_NCM_b.Text = b.ToString();
            this.TextBox_NCM_X0.Text = X0_ncm.ToString();
            this.TextBox_WS_Ke.Text = Ke;
            this.TextBox_WS_X0.Text = X0_ws.ToString();
            this.TextBox_WS_r.Text = r.ToString();
        }

        private bool ReadParams()
        {
            try
            {
                if (this.TextBox_NCM_a.Text.Length != 0)
                    a = Double.Parse(this.TextBox_NCM_a.Text);
                else throw new Exception("Please enter all parameters!");
                if (this.TextBox_NCM_b.Text.Length != 0)
                    b = Double.Parse(this.TextBox_NCM_b.Text);
                else throw new Exception("Please enter all parameters!");
                if (this.TextBox_NCM_X0.Text.Length != 0)
                    X0_ncm = Double.Parse(this.TextBox_NCM_X0.Text);
                else throw new Exception("Please enter all parameters!");
                if (this.TextBox_WS_X0.Text.Length != 0)
                    X0_ws = Double.Parse(this.TextBox_WS_X0.Text);
                else throw new Exception("Please enter all parameters!");
                if (this.TextBox_WS_Ke.Text.Length != 0)
                    Ke = this.TextBox_WS_Ke.Text;
                else throw new Exception("Please enter all parameters!");
                if (this.TextBox_WS_r.Text.Length != 0)
                    r = Double.Parse(this.TextBox_WS_r.Text);
                else throw new Exception("Please enter all parameters!");
            }
            catch (FormatException)
            {
                System.Windows.MessageBox.Show("Some of parameters you input is invalid!\nPlese check and re-enter.");
                return false;
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        
        private void Button_Click(object sender, RoutedEventArgs e) // Set default parameters
        {   
            r = 3.588;
            X0_ws = 0.566;
            Ke = "ChaoticEncryptionSystemNcmWSzqch";
            X0_ncm = 0.666; a = 0.7; b = 28;
            FillParams();
        }

        private void button_browerFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = false;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                inputPath = ofd.FileName;
                this.textBox_file.Text = inputPath;
            }
        }

        private void btn_encrypt_Click(object sender, RoutedEventArgs e)
        {
            if (inputPath == null || inputPath.Length == 0)
            {
                System.Windows.MessageBox.Show("Please select a file!");
                return;
            }
            if (!ReadParams())
                return;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = ".cef(Chaoticly Encrypted File)|*.cef";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputPath = sfd.FileName;
                FileInfo fi = new FileInfo(outputPath);
                if (fi.Exists)
                {
                    fi.Delete();
                }
                StartEncryptionThread();
            }
            
        }

        private void btn_decrypt_Click(object sender, RoutedEventArgs e)
        {
            if (inputPath == null || inputPath.Length == 0)
            {
                System.Windows.MessageBox.Show("Please select a file!");
                return;
            }
            if (!ReadParams())
                return;
            if (!inputPath.Substring(inputPath.LastIndexOf('.')).Equals(FILE_EXTENSION))
            {
                System.Windows.MessageBox.Show(String.Format("The selected file is not a valid \"{0}\" file.", FILE_EXTENSION));
                return;
            }
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputPath = fbd.SelectedPath;
                StartDecryptionThread();
            }
        }
    }
}