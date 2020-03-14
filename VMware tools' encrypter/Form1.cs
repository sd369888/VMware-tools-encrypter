using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace VMware_tools__encrypter
{


    public partial class Form1 : Form
    {
 
        public static string pasw= "123",str="",encstr="",destr="";
        public Form1()
        {
            
            InitializeComponent();
            pasw = ribbonTextBox1.TextBoxText;
            str = textBox1.Text;
          
        }
   
        public delegate void ProgressHandler(object sender, ProgressEventArgs e);
        #region AES加密

        public static byte[] TextEncrypt(string content, string secretKey)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);
            byte[] key = Encoding.UTF8.GetBytes(secretKey);

            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }

            return data;
        }

        #endregion AES加密

        #region AES解密

        public static string TextDecrypt(byte[] data, string secretKey)
        {
            byte[] key = Encoding.UTF8.GetBytes(secretKey);

            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i % key.Length];
            }

            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        #endregion AES解密
        private void ribbonButton3_Click(object sender, EventArgs e)
        {
            str = textBox1.Text;
            pasw = ribbonTextBox1.TextBoxText;
            if (pasw.Length != 8)
            {
                MessageBox.Show("密钥必须为8位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            progressBar1.Style = ProgressBarStyle.Marquee;
            edc();
            textBox2.Text = destr;
            progressBar1.Style = ProgressBarStyle.Continuous;
            for (int i = 0; i < 100; i++)
            {
                progressBar1.Value = i;
            }
        }
        
        public static byte[] mkbyteArray = System.Text.Encoding.Default.GetBytes(str);
        public static int i=0;
        private void ribbonTextBox1_TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < '0' || e.KeyChar > '9')
            {
                e.Handled = true;
            }
            else
            {
                i++;
                if (i > 8)
                {
                    e.Handled = true;//不处理
                }
            }



        }

        private void ribbonButton5_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBox1.SelectedText);
        }

        private void ribbonButton6_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBox1.SelectedText);
            if (textBox1.SelectedText==null)
            {
                return;
            }
            textBox1.Text = null;
        }

        private void ribbonButton4_Click(object sender, EventArgs e)
        {
          IDataObject iData = Clipboard.GetDataObject();
           
              int  idx = textBox1.SelectionStart;
        
            if (iData.GetDataPresent(DataFormats.Text))
            {
                textBox2.Text.Insert(idx,(String)iData.GetData(DataFormats.Text));
            }




        }

        private void ribbonButton7_Click(object sender, EventArgs e)
        {
            textBox1.Text = null;
        }

        private void ribbonButton8_Click(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Value = 0;
            for (int i = 0; i < 101; i++)
            {
                progressBar1.Value = i;
                Thread.Sleep(5);
            }
            progressBar1.Value = 0;
            textBox2.Text=System.Text.Encoding.Default.GetString(  TextEncrypt(textBox1.Text, ribbonTextBox2.TextBoxText));
        }

        public static void edc()
        {
            destr = CryptClass.DecryptDES(str, pasw);
        }
        public  void sten()
        {
           
           encstr= CryptClass.EncryptDES(str, pasw);
            VMware_tools__encrypter.Form1.CheckForIllegalCrossThreadCalls = false;
            Action act = delegate () {
                
};
            this.Invoke(act);
        }
        private void ribbonButton1_Click(object sender, EventArgs e)
        {
            pasw = ribbonTextBox1.TextBoxText;
            if (pasw.Length!=8)
            {
                MessageBox.Show("密钥必须为8位", "提示", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }
            str = textBox1.Text;
            progressBar1.Style = ProgressBarStyle.Marquee;
            /* Thread encr = new Thread(new ThreadStart(sten));
             encr.Start();*/
            sten();
            progressBar1.Value = 0;
            textBox2.Text = encstr;
            progressBar1.Style = ProgressBarStyle.Continuous;
            for (int i = 0; i < 101; i++)
            {
                progressBar1.Value = i;
                Thread.Sleep(5);
            }
        }

    }
    /// <summary>
    /// 文本加密解密类
    /// </summary>
    public class CryptClass
    {
        // 默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                VMware_tools__encrypter.Form1.encstr = Convert.ToBase64String(mStream.ToArray());
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return "无法加密\nEncryption failed !";
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            if (decryptString == "")
                return "";
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                VMware_tools__encrypter.Form1.destr = Encoding.UTF8.GetString(mStream.ToArray());
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "无法解密\nDecryption failed !";
            }
        }
    }

}
