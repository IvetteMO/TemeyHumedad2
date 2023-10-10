using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace TemeyHumedad2
{
    public partial class Form1 : Form
    {
        double temperatura = 0, humdad = 0;
        bool actDato = false;
        public Form1()
        {
            InitializeComponent();  
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnAbrir.Enabled = true;
            btnCerrar.Enabled = false;

            chart1.Series["Temperatura"].Points.AddXY(1, 1);
            chart1.Series["Humedad"].Points.AddXY(1, 1);
        }

        private void txtCom_DropDown(object sender, EventArgs e)
        {
            string[] portList = SerialPort.GetPortNames();
            txtCom.Items.Clear();
            txtCom.Items.AddRange(portList);
        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = txtCom.Text;
                serialPort1.BaudRate = Convert.ToInt32(txtBaud.Text);
                serialPort1.Open();
                btnAbrir.Enabled = false;
                btnCerrar.Enabled = true;

                chart1.Series["Temperatura"].Points.Clear();
                chart1.Series["Humedad"].Points.Clear();

                MessageBox.Show("Conexion exitosa a la tarjeta arduino");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();
                btnAbrir.Enabled = true;
                btnCerrar.Enabled = false;


                MessageBox.Show("Desconectado de la tarjeta arduino");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                serialPort1.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataIn = serialPort1.ReadTo("\n");
            Data_Parsing(dataIn);
            this.BeginInvoke(new EventHandler(Show_Data));
        }

        private void Show_Data(object sender, EventArgs e)
        {
            if (actDato == true)
            {
                lblTemp.Text = string.Format("Temperatura= {0}°C", temperatura.ToString());
                lblHumedad.Text = string.Format("Humedad= {0}%HR", humdad.ToString());

                chart1.Series["Temperatura"].Points.Add(temperatura);
                chart1.Series["Humedad"].Points.Add(humdad);
            }
        }

        private void Data_Parsing(string dato)
        {
            sbyte indexOf_startDataCharacter = (sbyte)dato.IndexOf("@");
            sbyte indexOfA = (sbyte)dato.IndexOf("A");
            sbyte indexOfB = (sbyte)dato.IndexOf("B");

            //Si el caracter es A o B, y la "@" existe in el paquete de datos
            if (indexOfA != -1 && indexOfB != -1 && indexOf_startDataCharacter != -1)
            {
                try
                {
                    string str_Temperatura = dato.Substring(indexOf_startDataCharacter + 1,
                        (indexOfA - indexOf_startDataCharacter) - 1);

                    string str_humedad = dato.Substring(indexOfA + 1, (indexOfB - indexOfA) - 1);

                    temperatura = Convert.ToDouble(str_Temperatura);
                    humdad = Convert.ToDouble(str_humedad);

                    actDato = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                actDato = false;
            }

        }
    }
}
        