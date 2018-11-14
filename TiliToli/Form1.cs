using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TiliToli
{
    public partial class Form1 : Form
    {
        struct Poz
        {
            public string Text;
            public int Sor;
            public int Oszlop;
        }

        struct Pont
        {
            public int LepesSz;
            public string Ido;
        }
        
        bool Game = false;
        int lepesek = 0;
        DateTime ido;
        Poz[] Poziciok;
        List<Pont> Pontszamok = new List<Pont>();

        public Form1()
        {
            InitializeComponent();
        }

        private void MozgasfokBeallitas()
        {
            foreach (object item in jatekGbox.Controls)
            {
                if (item is Label lab)
                {
                    int rOszlop = lab.Top - uresLb.Top;
                    int rSor = lab.Left - uresLb.Left;
                    lab.Tag = (rOszlop == 92 && rSor == 0) ? 1 : lab.Tag = (rOszlop == -92 && rSor == 0) ? 2 : lab.Tag = (rOszlop == 0 && rSor == 92) ? 8 : lab.Tag = (rOszlop == 0 && rSor == -92) ? 4 : 0;  // mozgásirány beállítása
                }
            }
        }

        private void GameEndCheck()
        {
            int p = 0;
            int talalat = 0;
            foreach (object item in jatekGbox.Controls)  // pozíciók ellenőrzése a kirakáshoz
            { 
                if (item is Label lab)
                {
                    if (Poziciok[p].Text == lab.Text && Poziciok[p].Sor == lab.Top && Poziciok[p].Oszlop == lab.Left)
                    {
                        talalat++;
                    }
                }
                p++;
            }
            MozgasfokBeallitas();
            if (talalat == 16 && Game)
            {
                timer1.Enabled = false;
                Label lbUzenet = new Label();
                lbUzenet.Parent = jatekGbox;
                lbUzenet.AutoSize = false;
                lbUzenet.Location = new Point(6, 141);
                lbUzenet.Size = new Size(355, 59);
                lbUzenet.Font = new Font("Monotype Corsiva", 26);
                lbUzenet.Text = "Játék vége! Gratulálok!";
                lbUzenet.TextAlign = ContentAlignment.MiddleCenter; 
                lbUzenet.ForeColor = Color.Red;
                lbUzenet.BackColor = Color.Gainsboro;
                lbUzenet.Visible = true;
                lbUzenet.BringToFront();
                timer2.Enabled = true;
                jatekGbox.Enabled = false;
                Pont ment = new Pont();
                ment.LepesSz = lepesek;
                ment.Ido = tbTime.Text;
                Pontszamok.Add(ment);
            }
        }

        private void label_Click(object sender, EventArgs e)
        {
            if (sender is Label lbklikked)
            {
                int mozgFok = Convert.ToInt32(lbklikked.Tag);  // aktuális kocka mozgásfoka
                if (mozgFok != 0)
                {
                    Point hely = uresLb.Location;
                    uresLb.Location = lbklikked.Location;
                    lbklikked.Location = new Point(hely.X, hely.Y);
                    lepesek++;
                    tbLepesSz.Text = lepesek.ToString();
                    GameEndCheck();
                }
            }
        }

        private void Inicializal()
        {
            Poziciok = new Poz[jatekGbox.Controls.Count];
            int p = 0;
            foreach (var item in jatekGbox.Controls)  // Labelek nevei és coordinátái tömbbe
            {
                if (item is Label lab)
                {
                    Poziciok[p].Text = lab.Text;
                    Poziciok[p].Sor = lab.Top;
                    Poziciok[p].Oszlop = lab.Left;
                    p++;
                }
            }
            
            jatekGbox.Enabled = false;
            btnUjJatek.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Inicializal();
            string[] eredmenyek;
            if (File.Exists("eredmeny.csv"))
            {
                eredmenyek = File.ReadAllLines("eredmeny.csv", Encoding.UTF8);
                for (int i = 0; i < eredmenyek.Length; i++)
                {
                    string[] er = eredmenyek[i].Split(';');
                    Pont ment = new Pont();
                    ment.LepesSz = Convert.ToInt32(er[0]);
                    ment.Ido = er[1];
                    Pontszamok.Add(ment);
                }
            }
            
        }

        private void btnKilep_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime most = DateTime.Now;
            tbTime.Text = (most - ido).ToString().Substring(0, 8);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Game = false;
            timer2.Enabled = false;
            jatekGbox.Controls.RemoveAt(0);
            btnUjJatek.Enabled = true;
        }

        private void Kever(EventArgs e)
        {
            Random r = new Random();
            while(lepesek < 100)
            {
                label_Click(jatekGbox.Controls[16 - r.Next(1, 16)], e);             
            }           
        }

        private void Alaphelyzet()
        {
            for(var i = 0; i < jatekGbox.Controls.Count; i++)
            {
                if (jatekGbox.Controls[i] is Label lab)
                {
                    lab.Location = new Point(Poziciok[i].Oszlop, Poziciok[i].Sor);
                }
            }
        }

        private void btnUjJatek_Click(object sender, EventArgs e)
        {
            Alaphelyzet();
            Kever(e);
            btnUjJatek.Enabled = false;
            jatekGbox.Enabled = true;
            lepesek = 0;
            tbLepesSz.Text = "0";
            Game = true;
            ido = DateTime.Now;
            timer1.Enabled = true;
        }

        private void btnPontok_Click(object sender, EventArgs e)
        {
            string eredmeny = "Helyezés\tLépés\tIdő\n";
            List<Pont> sorba;
            if (rbLepes.Checked)
            {
                sorba = Pontszamok.OrderBy(x => x.LepesSz).ToList();
            }
            else
            {
                sorba = Pontszamok.OrderBy(x => x.Ido).ToList();
            }
            
            for (int i = 0; i < Pontszamok.Count; i++)
            {
                eredmeny += i+1 + ".\t" + sorba[i].LepesSz + "\t" + sorba[i].Ido + "\n";
            }
            MessageBox.Show(eredmeny);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Pontszamok.Count > 0)
            {
                StreamWriter w = new StreamWriter("eredmeny.csv", false, Encoding.UTF8);
                for (int i = 0; i < Pontszamok.Count; i++)
                {
                    w.WriteLine(Pontszamok[i].LepesSz + ";" + Pontszamok[i].Ido);
                }
                w.Close();
            }
        }
    }
}
