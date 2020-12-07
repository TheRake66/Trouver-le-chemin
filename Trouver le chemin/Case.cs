using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trouver_le_chemin
{
    class Case
    {
        // -------------------------------------------
        // Static
        static readonly Color ColorVide = Color.White;
        static readonly Color ColorCheck = Color.Yellow;
        static readonly Color ColorCurrent = Color.Orange;
        static readonly Color ColorValide = Color.Lime;
        static readonly Color ColorObstacle = Color.Gray;
        static readonly Color ColorDebut = Color.Green;
        static readonly Color ColorFin = Color.Red;

        public enum CaseType
        {
            Vide,
            Obstacle,
            Debut,
            Fin
        }

        public static Case[,] LesCases;
        public static Case LeDebut;
        public static Case LaFin;
        // -------------------------------------------



        // -------------------------------------------
        // Proptiétés
        public Label LaGui;
        public CaseType LeType;
        public int X;
        public int Y;
        public int H;
        public int G;
        public int F { get { return this.H + this.G; } }
        // -------------------------------------------



        // -------------------------------------------
        // Constructor
        Case(Label LaGui, int X, int Y, CaseType LeType)
        {
            this.LaGui = LaGui;
            this.X = X;
            this.Y = Y;
            this.LeType = LeType;

            if (LeType == CaseType.Debut) Case.LeDebut = this;
            else if (LeType == CaseType.Fin) Case.LaFin = this;
        }
        // -------------------------------------------



        // -------------------------------------------
        // Méthode
        public static void EffacerCases()
        {
            foreach (Case UneCase in Case.LesCases)
            {
                UneCase.LeType = CaseType.Vide;
            }
        }
        public static void InitialiserCases(int Largeur, int Hauteur, int Taille)
        {
            Case.LesCases = new Case[Hauteur, Largeur];

            for (int i = 0; i < Hauteur; i++)
            {
                for (int j = 0; j < Largeur; j++)
                {
                    Label UnLabel = new Label();
                    UnLabel.AutoSize = false;
                    UnLabel.TextAlign = ContentAlignment.MiddleCenter;
                    UnLabel.Font = new Font("Consolas", 6F, FontStyle.Regular);
                    UnLabel.BackColor = Case.ColorVide;
                    UnLabel.Size = new Size(Taille, Taille);
                    UnLabel.Location = new Point(Taille * j, Taille * i);

                    // Evite les ref memoire au dernier i et j
                    int iCopy = i;
                    int jCopy = j;

                    // Creer une case
                    Case LaCase = new Case(UnLabel, jCopy, iCopy, CaseType.Vide);

                    UnLabel.MouseClick += new MouseEventHandler((a, b) =>
                    {
                        switch (b.Button)
                        {
                            // Point debut
                            case MouseButtons.Left:
                                if (Case.LeDebut != null) Case.LeDebut.LeType = CaseType.Vide;
                                LaCase.LeType = CaseType.Debut;
                                Case.LeDebut = LaCase;
                                break;

                            // Obstacle
                            case MouseButtons.Middle:
                                LaCase.LeType = LaCase.LeType == CaseType.Obstacle ? CaseType.Vide : CaseType.Obstacle;
                                break;

                            // Point fin
                            case MouseButtons.Right:
                                if (Case.LaFin != null) Case.LaFin.LeType = CaseType.Fin;
                                LaCase.LeType = CaseType.Fin;
                                Case.LaFin = LaCase;
                                break;
                        }
                    });

                    // Ajoute la case créer aux listes
                    Case.LesCases[i, j] = LaCase;
                }
            }
        }
        // -------------------------------------------



        // -------------------------------------------
        // Fonctions
        public int CalculerDistance_Manhattan()
        {
            int dx = Math.Abs(Case.LeDebut.X - Case.LaFin.X);
            int dy = Math.Abs(Case.LeDebut.Y - Case.LaFin.Y);
            int h = dx + dy;
            return h;
        }
        public int CalculerDistance_Diagonal()
        {
            int dx = Math.Abs(Case.LeDebut.X - Case.LaFin.X);
            int dy = Math.Abs(Case.LeDebut.Y - Case.LaFin.Y);
            int h = Math.Max(dx, dy);
            return h;
        }
        public double CalculerDistance_Diagonal2()
        {
            int dx = Math.Abs(Case.LeDebut.X - Case.LaFin.X);
            int dy = Math.Abs(Case.LeDebut.Y - Case.LaFin.Y);
            double h = (dx + dy) + (Math.Sqrt(2) - 2) * Math.Min(dx, dy);
            return h;
        }
        public double CalculerDistance_Euclidean()
        {
            int dx = (Case.LaFin.X - Case.LeDebut.X);
            int dy = (Case.LaFin.Y - Case.LeDebut.Y);
            double h = Math.Sqrt(dx * dx + dy * dy);
            return h;
        }
        public int CalculerDistance_Hex()
        {
            int dx = Case.LeDebut.X - Case.LaFin.X;
            int dy = Case.LeDebut.Y - Case.LaFin.Y;
            int dz = dx - dy;
            int h = Math.Max(Math.Abs(dx), Math.Max(Math.Abs(dy), Math.Abs(dz)));
            return h;
        }
        // -------------------------------------------
    }
}
