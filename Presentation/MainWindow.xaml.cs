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

namespace Presentation
{
    class EncryptionSystemData : INotifyPropertyChanged
    {
        public Double r;
        public Double X0_ws;
        public Byte[] Ke;
        public Byte[] Kd;
        public Double X0_ncm, a, b;
        public Byte[] plainText;
        public Byte[] cipherText;
        public Byte[] decodedText;
        public Byte[] decodingParamters;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void NotifyDataChanged()
        {
            OnPropertyChanged("ParameterR_SwitchWheel");
            OnPropertyChanged("ParameterX0_SwitchWheel");
            OnPropertyChanged("ParameterX0_NCM");
            OnPropertyChanged("ParameterB_NCM");
            OnPropertyChanged("ParameterA_NCM");
            OnPropertyChanged("EncodingKey_SwitchWheel");
            OnPropertyChanged("DecodingDey_SwitchWheel");
            OnPropertyChanged("PlainText");
            OnPropertyChanged("CipherText");
            OnPropertyChanged("DecodedText");
        }

        public void SetDefaultParameters()
        {
            r = EncryptionSystem.DefaultR_WheelSwitch;
            X0_ws = EncryptionSystem.DefaultX0_WheelSwitch;
            Ke = EncryptionSystem.DefaultKe_WheelSwicth;
            Kd = new Byte[0];
            X0_ncm = EncryptionSystem.DefaultX0_NCM;
            a = EncryptionSystem.DefaultA_NCM;
            b = EncryptionSystem.DefaultB_NCM;
            plainText = new Byte[0];
            cipherText = new Byte[0];
            decodedText = new Byte[0];
        }

        public EncryptionSystemData()
        {
            SetDefaultParameters();
        }

        public Double ParameterR_SwitchWheel
        {
            get { return r; }
        }

        public Double ParameterX0_SwitchWheel
        {
            get { return X0_ws; }
        }

        public Double ParameterX0_NCM
        {
            get { return X0_ncm; }
        }

        public Double ParameterB_NCM
        {
            get { return b; }
        }

        public Double ParameterA_NCM
        {
            get { return a; }
        }

        public String EncodingKey_SwitchWheel
        {
            get { return Encoding.Default.GetString(Ke); }
        }

        public String DecodingDey_SwitchWheel
        {
            get { return Encoding.Default.GetString(Kd); }
        }

        public String PlainText
        {
            set { plainText = Encoding.Default.GetBytes(value); OnPropertyChanged("PlainText"); }
            get { return Encoding.Default.GetString(plainText); }
        }

        public String CipherText
        {
            set { cipherText = Encoding.Default.GetBytes(value); OnPropertyChanged("CipherText"); }
            get { return Encoding.Default.GetString(cipherText); }
        }

        public String DecodedText
        {
            set { decodedText = Encoding.Default.GetBytes(value); OnPropertyChanged("DecodedText"); }
            get { return Encoding.Default.GetString(decodedText); }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EncryptionSystemData m_data = new EncryptionSystemData();

        private void m_BindData()
        {
            Binding binding0 = new Binding();
            binding0.Source = this.m_data;
            binding0.Path = new PropertyPath("PlainText");
            binding0.Mode = BindingMode.TwoWay;
            this.TextBok_PlainText.SetBinding(TextBox.TextProperty, binding0);

            Binding binding1 = new Binding();
            binding1.Source = this.m_data;
            binding1.Path = new PropertyPath("CipherText");
            binding1.Mode = BindingMode.TwoWay;
            this.TextBok_Cipher.SetBinding(TextBox.TextProperty, binding1);

            Binding binding2 = new Binding();
            binding2.Source = this.m_data;
            binding2.Path = new PropertyPath("DecodedText");
            binding2.Mode = BindingMode.TwoWay;
            this.TextBok_DecodedText.SetBinding(TextBox.TextProperty, binding2);

            Binding binding3 = new Binding();
            binding3.Source = this.m_data;
            binding3.Path = new PropertyPath("ParameterR_SwitchWheel");
            binding3.Mode = BindingMode.OneWay;
            this.TextBox_WS_r.SetBinding(TextBox.TextProperty, binding3);

            Binding binding4 = new Binding();
            binding4.Source = this.m_data;
            binding4.Path = new PropertyPath("ParameterX0_SwitchWheel");
            binding4.Mode = BindingMode.OneWay;
            this.TextBox_WS_X0.SetBinding(TextBox.TextProperty, binding4);

            Binding binding5 = new Binding();
            binding5.Source = this.m_data;
            binding5.Path = new PropertyPath("EncodingKey_SwitchWheel");
            binding5.Mode = BindingMode.OneWay;
            this.TextBox_WS_Ke.SetBinding(TextBox.TextProperty, binding5);

            Binding binding6 = new Binding();
            binding6.Source = this.m_data;
            binding6.Path = new PropertyPath("DecodingDey_SwitchWheel");
            binding6.Mode = BindingMode.OneWay;
            this.TextBox_WS_Kd.SetBinding(TextBox.TextProperty, binding6);

            Binding binding7 = new Binding();
            binding7.Source = this.m_data;
            binding7.Path = new PropertyPath("ParameterX0_NCM");
            binding7.Mode = BindingMode.OneWay;
            this.TextBox_NCM_X0.SetBinding(TextBox.TextProperty, binding7);

            Binding binding8 = new Binding();
            binding8.Source = this.m_data;
            binding8.Path = new PropertyPath("ParameterA_NCM");
            binding8.Mode = BindingMode.OneWay;
            this.TextBox_NCM_a.SetBinding(TextBox.TextProperty, binding8);

            Binding binding9 = new Binding();
            binding9.Source = this.m_data;
            binding9.Path = new PropertyPath("ParameterB_NCM");
            binding9.Mode = BindingMode.OneWay;
            this.TextBox_NCM_b.SetBinding(TextBox.TextProperty, binding9);
        }

        public MainWindow()
        {
            InitializeComponent();
            m_data.SetDefaultParameters();
            m_BindData();
        }

        private void Bth_Encode_Click(object sender, RoutedEventArgs e)
        {
            if (this.TextBox_NCM_a.Text.Length != 0)
                m_data.a = Convert.ToDouble(this.TextBox_NCM_a.Text);
            if (this.TextBox_NCM_b.Text.Length != 0)
                m_data.b = Convert.ToDouble(this.TextBox_NCM_b.Text);
            if (this.TextBox_NCM_X0.Text.Length != 0)
                m_data.X0_ncm = Convert.ToDouble(this.TextBox_NCM_X0.Text);
            if (this.TextBox_WS_r.Text.Length != 0)
                m_data.r = Convert.ToDouble(this.TextBox_WS_r.Text);
            if (this.TextBox_WS_X0.Text.Length != 0)
                m_data.X0_ws = Convert.ToDouble(this.TextBox_WS_X0.Text);
            if (this.TextBox_WS_Ke.Text.Length != 0)
            {
                List<Byte> bts = new List<byte>();
                bts.AddRange(Encoding.Default.GetBytes(this.TextBox_WS_Ke.Text));
                // 256bits = 32bytes
                if (bts.Count < 32)
                {
                    while (bts.Count < 32)
                        bts.Add(0xF);
                }
                m_data.Ke = bts.GetRange(0, 32).ToArray();
            }
            List<Byte> encodingParameters = new List<byte>();
            encodingParameters.AddRange(BitConverter.GetBytes(m_data.X0_ws));
            encodingParameters.AddRange(BitConverter.GetBytes(m_data.r));
            encodingParameters.AddRange(m_data.Ke);
            encodingParameters.AddRange(BitConverter.GetBytes(m_data.X0_ncm));
            encodingParameters.AddRange(BitConverter.GetBytes(m_data.a));
            encodingParameters.AddRange(BitConverter.GetBytes(m_data.b));

            m_data.decodingParamters = EncryptionSystem.Encode(m_data.plainText, encodingParameters.ToArray(), out m_data.cipherText);

            m_data.Kd = m_data.decodingParamters.ToList().GetRange(16, 32).ToArray();

            m_data.NotifyDataChanged();
        }

        private void Btn_Decode_Click(object sender, RoutedEventArgs e)
        {
            EncryptionSystem.Decode(m_data.cipherText, m_data.decodingParamters, out m_data.decodedText);
            m_data.NotifyDataChanged();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            m_data.SetDefaultParameters();
            m_data.NotifyDataChanged();
        }
    }
}
