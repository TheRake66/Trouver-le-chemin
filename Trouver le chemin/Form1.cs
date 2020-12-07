using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trouver_le_chemin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            initialiseCases();
        }
        private void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Autorise que les chiffres
            if (!char.IsDigit(e.KeyChar) && (Keys)e.KeyChar != Keys.Back)
            {
                e.Handled = true;
            }
        }
        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Verifi la taille
                if (Convert.ToInt32(((ToolStripTextBox)sender).Text) < 15)
                {
                    ((ToolStripTextBox)sender).Text = "15";
                }
                // Init les cases avec les nouvelles valeur
                initialiseCases();
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // Si aucun debut ni fin selectionnée
            if (Case.LeDebut == null || Case.LaFin == null)
            {
                MessageBox.Show("Veuillez choisir un point de début et un point de fin. Pour ce faire :" + Environment.NewLine +
                                "     • Clique gauche : Placer le point de début." + Environment.NewLine +
                                "     • Clique molette : Placer un obstacle." + Environment.NewLine +
                                "     • Clique droit : Placer le point de fin.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Lance une recherche
                effacerCases();
                pathFinding();
            }
        }
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            this.toolStripLabel4.Checked = !this.toolStripLabel4.Checked;
        }

        private void effacerCases()
        {

        }
        private void initialiseCases()
        {
            // Defini les tailles avec les textbox
            int Hauteur = Convert.ToInt32(this.toolStripTextBox1.Text);
            int Largeur = Convert.ToInt32(this.toolStripTextBox2.Text);
            int Taille = Convert.ToInt32(this.toolStripTextBox3.Text);

            // Redimenssione la fenetre
            this.Width += this.panel1.Width - Largeur * Taille;
            this.Height += this.panel1.Height - Hauteur * Taille;

            // Efface et reset les cases
            this.panel1.Controls.Clear();
            Case.InitialiserCases(Largeur, Hauteur, Taille);
            foreach (Case UneCase in Case.LesCases)
            {
                this.panel1.Controls.Add(UneCase.LaGui);
            }
        }
        private void changeCaseColor(Label uneCase, Color uneColor)
        {
            /*
            // Change la couleur d'une case en verifiant si pas debut ni fin ni obstacle
            if (uneCase != Case.LeDebut && uneCase != Case.LaFin && uneCase.BackColor != this.colorObstacle)
            {
                uneCase.BackColor = uneColor;
                if (this.toolStripLabel4.Checked) uneCase.Refresh();
            }*/
        }

        private void checkVoisin(int x, int y)
        {
        }
        private void pathFinding()
        {
            /*
             * 

                // On vérifie toute les cases voisines
                if (y - 1 >= 0 && x + 1 < this.lesCases.GetLength(1)) checkVoisinFounded(x + 1, y - 1);                          // Haut droit
                if (x - 1 >= 0 && y - 1 >= 0) checkVoisinFounded(x - 1, y - 1);                                                  // Haut gauche
                if (x - 1 >= 0 && y + 1 < this.lesCases.GetLength(0)) checkVoisinFounded(x - 1, y + 1);                          // Bas gauche
                if (x + 1 < this.lesCases.GetLength(1) && y + 1 < this.lesCases.GetLength(0)) checkVoisinFounded(x + 1, y + 1);  // Bas droit
                if (y - 1 >= 0) checkVoisinFounded(x, y - 1);                                                                    // Haut
                if (x - 1 >= 0) checkVoisinFounded(x - 1, y);                                                                    // Gauche
                if (x + 1 < this.lesCases.GetLength(1)) checkVoisinFounded(x + 1, y);                                            // Droite
                if (y + 1 < this.lesCases.GetLength(0)) checkVoisinFounded(x, y + 1);*/

        }
    }
}
