using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Universo
{
    
    public partial class Form1 : Form
    {
        private Graphics g;
        private Universo U, Uinicial;
        int numCorpos;
        int numInterac;
        int numTempoInterac;
       

        public Form1()
        {
            InitializeComponent();            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int xMax, yMax, mMin, mMax;

            numCorpos = Convert.ToInt32(qtdCorpos.Text);
            U = new Universo();

            progressBar1.Value = 0;
            xMax = Convert.ToInt32(valXMax.Text);
            yMax = Convert.ToInt32(valYMax.Text);
            mMin = Convert.ToInt32(masMin.Text);
            mMax = Convert.ToInt32(masMax.Text);

            U.carCorp(numCorpos,0,xMax, 0, yMax, mMin, mMax);

            Uinicial = new Universo();
            Uinicial.copiaUniverso(U);

            Form1.ActiveForm.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            numInterac = Convert.ToInt32(qtdInterac.Text);
            numTempoInterac = Convert.ToInt32(qtdTempoInterac.Text);

            progressBar1.Maximum = numInterac;
            progressBar1.Minimum = 0;

            if (radioButton1.Checked) 
            {
                for (int i = 0; i <= numInterac; i++)
                {
                    U.interCorp(numTempoInterac);
                    progressBar1.Value = i;


                    
                    if ((i % 100 == 0) && (Form1.ActiveForm != null))
                    {
                        Form1.ActiveForm.Refresh();
                    }
                }
            }
            else if (radioButton2.Checked) 
            {
                for (int i = 0; i <= numInterac; i++)
                {
                    U.interCorp(numTempoInterac);
                    progressBar1.Value = i;
                }

                if (Form1.ActiveForm != null)
                {
                    Form1.ActiveForm.Refresh();
                }
            }
            else if (radioButton3.Checked) 
            {
                string texto;
                Corpos cp;

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Arquivos Universo|*.uni|Todos os arquivos|*.*";
                saveFileDialog1.Title = "Salvar arquivo";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

                    sw.WriteLine(U.qtdCorp() + ";" + numInterac);

                    for (int i = 0; i <= numInterac; i++)
                    {
                        U.interCorp(numTempoInterac);
                        progressBar1.Value = i;

                        if(i % 10 == 0){
                            texto = "** Interacao " + i + " ************";
                            sw.WriteLine(texto);

                            for (int j = 0; j < U.qtdCorp(); j++)
                            {
                                cp = U.getCorpo(j);
                                if (cp != null)
                                {
                                    texto = cp.getNome() + ";"
                                            + cp.getMassa() + ";"
                                            + cp.getPosX() + ";"
                                            + cp.getPosY() + ";"
                                            + cp.getPosZ() + ";"
                                            + cp.getVelX() + ";"
                                            + cp.getVelY() + ";"
                                            + cp.getVelZ() + ";"
                                            + cp.getDensidade();

                                    sw.WriteLine(texto);
                                }
                            }
                        }
                    }

                    sw.Close();
                    fs.Close();
                }
            }
        }

        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs pe)
        {
            Corpos cp;
            float prop = 1, propX = 1, propY = 1;
            float deslocX = 0;
            float deslocY = 0;
            float maxX = 0, W = 0, H = 0, maxY = 0;
            double posX = 0, posY = 0;
            int qtdCp;

            if (Form1.ActiveForm != null)
            {

                if (valXMax.Text == "") {
                    valXMax.Text = Form1.ActiveForm.Size.Width.ToString();
                    valYMax.Text = Form1.ActiveForm.Size.Height.ToString();
                }

                W = Form1.ActiveForm.Size.Width - 50;
                H = Form1.ActiveForm.Size.Height - 50;                
            }

            if (U != null)
            {
                g = pe.Graphics;
                qtdCp = U.qtdCorp();


                for (int i = 0; i < qtdCp; i++)
                {
                    cp = U.getCorpo(i);
                    if (cp != null)
                    {
                        posX = cp.getPosX();
                        posY = cp.getPosY();

                        if (posX < deslocX)
                        {
                            deslocX = (float)posX;
                        }
                        if (posY < deslocY)
                        {
                            deslocY = (float)posY;
                        }

                        if (posY > maxY)
                        {
                            maxY = (float)posY;
                        }
                        if (posX > maxX)
                        {
                            maxX = (float)posX;
                        }
                    }
                }

                if ((maxX - deslocX) > W)
                {
                    propX = (maxX - deslocX) / W;
                }
                if((maxY - deslocY) > H)
                {
                    propY = (maxY - deslocY) / H;
                }

                if (propX > propY)
                {
                    prop = propX;
                } else
                {
                    prop = propY;
                }

                txtProporcao.Text = (1/prop).ToString();
                qtdCorposAtual.Text = qtdCp.ToString();

                for (int i = 0; i < qtdCp; i++)
                {
                    cp = U.getCorpo(i);
                    if (cp != null)
                    {
                        posX = cp.getPosX() - deslocX;
                        posY = cp.getPosY() - deslocY;

                        g.DrawEllipse(new Pen(Color.FromArgb((int)cp.getDensidade(), 0, 0)),
                            (float)(posX - cp.getRaio()) / prop,
                            (float)(posY - cp.getRaio()) / prop,
                            (float)(cp.getRaio() * 2) / prop,
                            (float)(cp.getRaio() * 2) / prop);

                        g.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                            (float)(posX) / prop,
                            (float)(posY) / prop,
                            (float)(posX + (cp.getForcaX() * 50)) / prop,
                            (float)(posY) / prop);
                        g.DrawLine(new Pen(Color.FromArgb(0, 0, 255)),
                            (float)posX / prop,
                            (float)posY / prop,
                            (float)(posX) / prop,
                            (float)(posY + (cp.getForcaY() * 50)) / prop);
                    }
                }
            }
        }

        private void btn_grava_Click(object sender, EventArgs e)
        {
            Corpos cp;
            int i;
            string texto;

            if (U != null)
            {

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Arquivos Universo|*.uni|Todos os arquivos|*.*";
                saveFileDialog1.Title = "Salvar arquivo";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

                    sw.WriteLine(U.qtdCorp());
                    for (i = 0; i < U.qtdCorp(); i++)
                    {
                        cp = U.getCorpo(i);
                        if (cp != null)
                        {
                            texto = cp.getNome() + ";"
                                  + cp.getMassa() + ";"
                                  + cp.getPosX() + ";"
                                  + cp.getPosY() + ";"
                                  + cp.getPosZ() + ";"
                                  + cp.getVelX() + ";"
                                  + cp.getVelY() + ";"
                                  + cp.getVelZ() + ";"
                                  + cp.getDensidade();

                            sw.WriteLine(texto);
                        }
                    }

                    sw.Close();
                    fs.Close();
                }
            }
            else 
            {
                MessageBox.Show("Não há corpos no Universo a serem salvos", "Atenção");
            }
        }

        private void btn_grava_ini_Click(object sender, EventArgs e)
        {
            Corpos cp;
            int i;
            string texto;

            if (Uinicial != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Arquivos Universo|*.uni|Todos os arquivos|*.*";
                saveFileDialog1.Title = "Salvar arquivo";
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

                    sw.WriteLine(Uinicial.qtdCorp());
                    for (i = 0; i < Uinicial.qtdCorp(); i++)
                    {
                        cp = Uinicial.getCorpo(i);
                        if (cp != null)
                        {
                            texto = cp.getNome() + ";"
                                  + cp.getMassa() + ";"
                                  + cp.getPosX() + ";"
                                  + cp.getPosY() + ";"
                                  + cp.getPosZ() + ";"
                                  + cp.getVelX() + ";"
                                  + cp.getVelY() + ";"
                                  + cp.getVelZ() + ";"
                                  + cp.getDensidade();

                            sw.WriteLine(texto);
                        }
                    }

                    sw.Close();
                    fs.Close();
                }
            }
            else 
            {
                MessageBox.Show("Não há corpos no Universo a serem salvos", "Atenção");
            }

        }

        private void btn_carrega_Click(object sender, EventArgs e)
        {
            string texto;
            int controle;
            Corpos cp;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Arquivos Universo|*.uni|Todos os arquivos|*.*";
            openFileDialog1.Title = "Abrir arquivo";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
                controle = 0;

                while (!sr.EndOfStream)
                {
                    texto = sr.ReadLine();
                    if(controle != 0)
                    {
                        string[] valores = texto.Split(';');

                        cp = new Corpos(valores[0],
                                       Convert.ToDouble(valores[1]),
                                       Convert.ToDouble(valores[2]),
                                       Convert.ToDouble(valores[3]),
                                       Convert.ToDouble(valores[4]),
                                       Convert.ToDouble(valores[5]),
                                       Convert.ToDouble(valores[6]),
                                       Convert.ToDouble(valores[7]),
                                       Convert.ToDouble(valores[8]));
                        U.setCorpo(cp,controle - 1);
                    }
                    else
                    {
                        qtdCorpos.Text = texto;

                        numCorpos = Convert.ToInt32(qtdCorpos.Text);
                        U = new Universo();
                    }
                    controle++;
                }

                sr.Close();

                progressBar1.Value = 0;

                Uinicial = new Universo();
                Uinicial.copiaUniverso(U);

                Form1.ActiveForm.Refresh();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btn_carregaSimulacao_Click(object sender, EventArgs e)
        {
            string texto;
            string[] valores;
            int controle;
            Corpos cp;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Arquivos Universo|*.uni|Todos os arquivos|*.*";
            openFileDialog1.Title = "Abrir arquivo";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);

                texto = sr.ReadLine();

                valores = texto.Split(';');

                qtdCorpos.Text = valores[0]; 
                qtdInterac.Text = valores[1]; 
                progressBar1.Maximum = (Convert.ToInt32(qtdInterac.Text) / 10) + 1; 

                numCorpos = Convert.ToInt32(qtdCorpos.Text);
                
                controle = 0;
                progressBar1.Value = 0;

                while (!sr.EndOfStream)
                {
                    texto = sr.ReadLine();
                    
                    
                    if(texto[0] == '*')
                    {
                        controle = 0;
                        progressBar1.Value++;

                        if (Form1.ActiveForm != null)
                        {
                            Form1.ActiveForm.Refresh();
                        }

                        U = new Universo();
                    }
                    else 
                    {
                        valores = texto.Split(';');

                        cp = new Corpos(valores[0],
                                       Convert.ToDouble(valores[1]),
                                       Convert.ToDouble(valores[2]),
                                       Convert.ToDouble(valores[3]),
                                       Convert.ToDouble(valores[4]),
                                       Convert.ToDouble(valores[5]),
                                       Convert.ToDouble(valores[6]),
                                       Convert.ToDouble(valores[7]),
                                       Convert.ToDouble(valores[8]));
                        U.setCorpo(cp, controle);
                        controle++;
                    }
                }

                sr.Close();

                if (Form1.ActiveForm != null)
                {
                    Form1.ActiveForm.Refresh();
                }
            }
        }
    }
}
