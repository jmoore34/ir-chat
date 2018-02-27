using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;

namespace IR_Chat_Client
{
    public partial class Form1 : Form
    {
        SerialPort port = new SerialPort();
        bool receivingMessage = false;
        HtmlElement currentReceivingMsgSpan;
        
        const int READ_INTERVAL = 1000;

        public Form1()
        {
            InitializeComponent();
            refreshPorts();
            portComboBox.SelectedIndex = portComboBox.Items.Count - 1;
            webBrowser1.DocumentText = "<html></html>";

            
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void submit()
        {
            if (textBox1.Text != String.Empty && port.IsOpen)
            {
                
                bool success = true;
                try
                {
                    port.Write(textBox1.Text + '\0'); //append a null to signify the end
                }
                catch (Exception ex)
                {
                    success = false;
                }
                if (success)
                {
                    appendMsg("Sent: ", textBox1.Text);
                    textBox1.Text = String.Empty;
                }
                
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                submit();
            }
        }


        private void connectButtonClick(object sender, EventArgs e)
        {
            
            
            if (!portComboBox.SelectedItem.ToString().Equals(String.Empty) && !port.IsOpen)
            {
                port.PortName = portComboBox.SelectedItem.ToString();
                port.BaudRate = 9600;
                bool success = true;
                try
                {
                    port.Open();
                }
                catch (Exception ex)
                {
                    success = false;
                    HtmlElement errorMsg = webBrowser1.Document.CreateElement("span");
                    errorMsg.InnerText = "Unable to connect to " + portComboBox.SelectedItem.ToString() + ".\n";
                    webBrowser1.Document.Body.AppendChild(errorMsg);
                }

                if (success)
                {

                    connectButton.Enabled = false;
                    disconnectButton.Enabled = true;
                    HtmlElement successMsg = webBrowser1.Document.CreateElement("span");
                    successMsg.InnerText = "Connected to " + portComboBox.SelectedItem.ToString() + ".\n";
                    webBrowser1.Document.Body.AppendChild(successMsg);

                    readInterval.Enabled = true;
                    
                }
            }
            
        }

        public void processByte()
        {


            if (port.BytesToRead > 0)
            {
                int readChar = port.ReadByte();
                bool valid = readChar >= 32 && readChar <= 127;
                if (valid)
                {
                    if (!receivingMessage)
                    {
                        currentReceivingMsgSpan = appendMsg("Received: ", "");
                        receivingMessage = true;
                    }
                    if (readChar >= 32 && readChar <= 127)
                    {
                        currentReceivingMsgSpan.InnerText += (char)readChar;
                    }
                }
                else
                {
                    receivingMessage = false;
                }
            
            }
        }

        private void refreshPortButtonClick(object sender, EventArgs e)
        {
            refreshPorts();
        }

        private void refreshPorts()
        {
            portComboBox.Items.Clear();
            portComboBox.Items.AddRange(SerialPort.GetPortNames());
        }

        public HtmlElement appendMsg(string sender, string msg)
        {
            HtmlElement senderSpan = null;
            HtmlElement msgSpan = null;
            //MessageBox.Show(sender + msg);
            try
            {
                senderSpan = webBrowser1.Document.CreateElement("span");
                senderSpan.InnerText = sender;
                senderSpan.Style = "font-weight:bold;";
                msgSpan = webBrowser1.Document.CreateElement("span");
                msgSpan.InnerText = msg;
                HtmlElement br = webBrowser1.Document.CreateElement("br");

                webBrowser1.Document.Body.AppendChild(senderSpan);
                webBrowser1.Document.Body.AppendChild(msgSpan);
                webBrowser1.Document.Body.AppendChild(br);
            }
            catch (InvalidCastException ex)
            {

            }
            return msgSpan;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)
            {
                bool success = true;
                try
                {
                    port.Close();
                }
                catch (Exception ex)
                {
                    success = false;
                }
                if (success)
                {
                    readInterval.Enabled = false;
                    connectButton.Enabled = true;
                    disconnectButton.Enabled = false;
                    HtmlElement successMsg = webBrowser1.Document.CreateElement("span");
                    successMsg.InnerText = "Disconnected from " + portComboBox.SelectedItem.ToString() + ".\n";
                    webBrowser1.Document.Body.AppendChild(successMsg);
                }
            }
        }



        private void readInterval_Tick(object sender, EventArgs e)
        {
            processByte();
        }

        private void portComboBox_MouseClick(object sender, MouseEventArgs e)
        {
            refreshPorts();
        }
    }
}
