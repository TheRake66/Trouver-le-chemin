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
        Label[,] lesCases;
            int nbCols;
            int nbLigns;
            int tailleCase;

        Label debut;
            int debutX;
            int debutY;
        Label fin;
            int finX;
            int finY;


        int derniereVoisinDistance;
        int bonX;
        int bonY;
        List<int[]> bonVoisinsTrouves = new List<int[]>();
        List<Label> caseDejaVerifiee = new List<Label>();
        List<Label> bonChemin = new List<Label>();


        int lastVoisinIndex;
        int lastVoisinX;
        int lastVoisinY;
        List<Label> cheminLePlusCourt = new List<Label>();


        // Couleur des cases
        Color colorVide = Color.White;
        Color colorCheck = Color.Yellow;
        Color colorCurrent = Color.Orange;
        Color colorValide = Color.Lime;
        Color colorObstacle = Color.Gray;
        Color colorDebut = Color.Green;
        Color colorFin = Color.Red;


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
            if (this.debut == null || this.fin == null)
            {
                MessageBox.Show("Veuillez choisir un point de début et un point de fin. Pour ce faire :" + Environment.NewLine +
                                "     • Clique gauche : Placer le point de début." + Environment.NewLine +
                                "     • Clique molette : Placer un obstacle." + Environment.NewLine +
                                "     • Clique droit : Placer le point de fin.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Reset les cases
                foreach (Label uneCase in this.lesCases)
                {
                    uneCase.Text = "";
                    changeCaseColor(uneCase, this.colorVide);
                }
                this.toolStripStatusLabel1.Text = "";
                this.bonVoisinsTrouves.Clear();
                this.bonChemin.Clear();
                this.caseDejaVerifiee.Clear();
                this.cheminLePlusCourt.Clear();
                // Lance une recherche
                pathFinding(this.debutX, this.debutY);
            }
        }

        private void initialiseCases()
        {
            // Defini les tailles avec les textbox
            this.nbLigns = Convert.ToInt32(this.toolStripTextBox1.Text);
            this.nbCols = Convert.ToInt32(this.toolStripTextBox2.Text);
            this.tailleCase = Convert.ToInt32(this.toolStripTextBox3.Text);

            // Redimenssione la fenetre
            while (this.panel1.Width < this.tailleCase * this.nbCols) { this.Width++; }
            while (this.panel1.Height < this.tailleCase * this.nbLigns) { this.Height++; }
            while (this.panel1.Width > this.tailleCase * this.nbCols) { this.Width--; }
            while (this.panel1.Height > this.tailleCase * this.nbLigns) { this.Height--; }
            // Recentre la fenetre
            this.Location = new Point(0, 0);

            // Efface et reset les cases
            this.panel1.Controls.Clear();
            this.lesCases = new Label[this.nbLigns, this.nbCols];

            for (int i = 0; i < this.nbLigns; i++)
            {
                for (int j = 0; j < this.nbCols; j++)
                {
                    // Creer une case
                    Label uneCase = new Label();
                    uneCase.AutoSize = false;
                    uneCase.TextAlign = ContentAlignment.MiddleCenter;
                    uneCase.Font = new Font("Consolas", 6F, FontStyle.Regular);
                    uneCase.BackColor = this.colorVide;
                    //uneCase.BorderStyle = BorderStyle.FixedSingle;
                    uneCase.Size = new Size(this.tailleCase, this.tailleCase);
                    uneCase.Location = new Point(this.tailleCase * j, this.tailleCase * i);

                    // Evite les ref memoire au dernier i et j
                    int iCopy = i;
                    int jCopy = j;

                    uneCase.MouseClick += new MouseEventHandler((a, b) =>
                    {
                        switch (b.Button)
                        {
                            // Point debut
                            case MouseButtons.Left:
                                if (this.debut != null) this.debut.BackColor = this.colorVide;
                                uneCase.BackColor = this.colorDebut;
                                this.debut = uneCase;
                                this.debutX = jCopy;
                                this.debutY = iCopy;
                                break;

                                // Obstacle
                            case MouseButtons.Middle:
                                if (uneCase.BackColor == this.colorObstacle) uneCase.BackColor = this.colorVide;
                                else uneCase.BackColor = this.colorObstacle;
                                break;

                            // Point fin
                            case MouseButtons.Right:
                                if (this.fin != null) this.fin.BackColor = this.colorVide;
                                uneCase.BackColor = this.colorFin;
                                this.fin = uneCase;
                                this.finX = jCopy;
                                this.finY = iCopy;
                                break;
                        }
                    });

                    // Ajoute la case créer aux listes
                    this.lesCases[i, j] = uneCase;
                    this.panel1.Controls.Add(uneCase);
                }
            }
        }
        private void changeCaseColor(Label uneCase, Color uneColor)
        {
            // Change la couleur d'une case en verifiant si pas debut ni fin ni obstacle
            if (uneCase != this.debut && uneCase != this.fin && uneCase.BackColor != this.colorObstacle) uneCase.BackColor = uneColor;
        }

        private void checkVoisin(int x, int y)
        {
            // Si la case n'a pas déjà été testée et que c'est pas un obstacle
            if (!this.caseDejaVerifiee.Contains(this.lesCases[y, x]) && this.lesCases[y, x].BackColor != this.colorObstacle)
            {
                changeCaseColor(this.lesCases[y, x], this.colorCheck);
                // Calcul la distance entre deux points sans tenir compte des obstacle
                int calc = Math.Abs(this.finX - x) + Math.Abs(this.finY - y);

                // Si distance plus courte que la precedente ou qu'aucune precedente
                if (calc < this.derniereVoisinDistance || this.derniereVoisinDistance == -1)
                {
                    this.bonVoisinsTrouves.Clear();
                    this.derniereVoisinDistance = calc;
                    this.bonX = x;
                    this.bonY = y;
                }
                // Si la même que precedente on la met dans une liste
                else if (calc == this.derniereVoisinDistance)
                {
                    this.bonVoisinsTrouves.Add(new int[] { x, y });
                }
            }
        }
        private void pathFinding(int x, int y)
        {
            // Chemin trouvé
            if (x == this.finX && y == this.finY)
            {
                pathFounded(this.debutX, this.debutY);
            }
            else
            {
                // Reset
                this.derniereVoisinDistance = -1;

                // Couleur en cours orange
                changeCaseColor(this.lesCases[y, x], this.colorCurrent);

                // On vérifie toute les cases voisines
                if (y - 1 >= 0 && x + 1 < this.lesCases.GetLength(1)) checkVoisin(x + 1, y - 1);                          // Haut droit
                if (x - 1 >= 0 && y - 1 >= 0) checkVoisin(x - 1, y - 1);                                                  // Haut gauche
                if (x - 1 >= 0 && y + 1 < this.lesCases.GetLength(0)) checkVoisin(x - 1, y + 1);                          // Bas gauche
                if (x + 1 < this.lesCases.GetLength(1) && y + 1 < this.lesCases.GetLength(0)) checkVoisin(x + 1, y + 1);  // Bas droit
                if (y - 1 >= 0) checkVoisin(x, y - 1);                                                                    // Haut
                if (x - 1 >= 0) checkVoisin(x - 1, y);                                                                    // Gauche
                if (x + 1 < this.lesCases.GetLength(1)) checkVoisin(x + 1, y);                                            // Droite
                if (y + 1 < this.lesCases.GetLength(0)) checkVoisin(x, y + 1);                                            // Bas

                // Si plusieurs voisins de même longueurs trouvés on en tire un au pif
                if (this.bonVoisinsTrouves.Count > 1)
                {
                    int rnd = (new Random()).Next(0, this.bonVoisinsTrouves.Count);
                    this.bonX = this.bonVoisinsTrouves[rnd][0];
                    this.bonY = this.bonVoisinsTrouves[rnd][1];
                }

                // Bloqué
                if (this.derniereVoisinDistance == -1)
                {
                    // Reviens en arrière
                    if (this.caseDejaVerifiee.Count > 0) this.caseDejaVerifiee.RemoveAt(this.caseDejaVerifiee.Count - 1);
                    else
                    {
                        this.toolStripStatusLabel1.Text = "Aucun chemin !";
                        return;
                    }
                }
                // Voisin le plus proche
                else this.bonChemin.Add(this.lesCases[this.bonY, this.bonX]);


                // Refresh et recursive
                this.caseDejaVerifiee.Add(this.lesCases[y, x]); // Dit que la case vient d'être traitée
                this.Refresh();
                pathFinding(this.bonX, this.bonY);
            }
        }

        private void checkVoisinFounded(int x, int y)
        {
            // Trouve le chemin le plus court partie tous les chemin trouvés
            this.lesCases[y, x].Text = this.bonChemin.IndexOf(this.lesCases[y, x]).ToString();
            if (this.bonChemin.Contains(this.lesCases[y, x]) && (this.bonChemin.IndexOf(this.lesCases[y, x]) > this.lastVoisinIndex || this.lastVoisinIndex == -1))
            {
                this.lastVoisinIndex = this.bonChemin.IndexOf(this.lesCases[y, x]);
                this.lastVoisinX = x;
                this.lastVoisinY = y;
            }
        }
        private void pathFounded(int x, int y)
        {
            // Chemin trouvé
            if (x == this.finX && y == this.finY)
            {
                foreach (Label uneCase in this.cheminLePlusCourt)
                {
                    changeCaseColor(uneCase, this.colorValide);
                }

                this.toolStripStatusLabel1.Text = "Chemin de trouvé de taille : " + this.cheminLePlusCourt.Count;
            }
            else
            {
                this.lastVoisinIndex = -1;

                // On vérifie toute les cases voisines
                if (y - 1 >= 0 && x + 1 < this.lesCases.GetLength(1)) checkVoisinFounded(x + 1, y - 1);                          // Haut droit
                if (x - 1 >= 0 && y - 1 >= 0) checkVoisinFounded(x - 1, y - 1);                                                  // Haut gauche
                if (x - 1 >= 0 && y + 1 < this.lesCases.GetLength(0)) checkVoisinFounded(x - 1, y + 1);                          // Bas gauche
                if (x + 1 < this.lesCases.GetLength(1) && y + 1 < this.lesCases.GetLength(0)) checkVoisinFounded(x + 1, y + 1);  // Bas droit
                if (y - 1 >= 0) checkVoisinFounded(x, y - 1);                                                                    // Haut
                if (x - 1 >= 0) checkVoisinFounded(x - 1, y);                                                                    // Gauche
                if (x + 1 < this.lesCases.GetLength(1)) checkVoisinFounded(x + 1, y);                                            // Droite
                if (y + 1 < this.lesCases.GetLength(0)) checkVoisinFounded(x, y + 1);

                // Prend le voisin avec le plus de poids dans la liste des chemins trouvé
                this.cheminLePlusCourt.Add(this.lesCases[this.lastVoisinY, this.lastVoisinX]);

                // Refresh et recursive
                this.Refresh();
                pathFounded(this.lastVoisinX, this.lastVoisinY);
            }

        }
    }
}
