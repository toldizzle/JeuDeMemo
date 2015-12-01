using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace JeuDeMemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Model context = new Model();
        //Tours
        bool _bTourSysteme = false;
        bool _bContreSysteme = false;
        bool _bTourJ1 = false;
        bool _bTourJ2 = false;

        byte _bGrandeur = 0;
        int _premierChoix = 0;
        int _deuxiemeChoix = 0;
        //Noms des boutons
        string _btn1;
        string _btn2;
        bool _bTheme = false; //false = fruit      true = voiture
        MediaPlayer _player = new MediaPlayer();
        List<int> _lstImages = new List<int>();
        Random rand = new Random();
        bool _bFinDePartie = false;

        //Base de données
        List<string> _lstChoixSysteme = new List<string>();
        List<string> _lstChoixJoueur1 = new List<string>();
        int _pointageJ1 = 0;
        int _pointageJ2 = 0;
        string _prenomJ1 = "";
        string _nomJ1 = "";
        string _prenomJ2 = "";
        string _nomJ2 = "";

        public MainWindow()
        {
            InitializeComponent();
        }
        public async void JeuMemoire()
        {

            txtPointJ1.Text = _pointageJ1.ToString();
            txtPointJ2.Text = _pointageJ2.ToString();
            //Match des cartes (empêche l'erreur de 2 cartes mélanges joker ou unique)
            if (_premierChoix == _deuxiemeChoix && _premierChoix != 35 && _premierChoix != 32 && _premierChoix != 33)
            {
                MatchCartes();
            }

            //Différentes cartes
            else if (_premierChoix != 0 && _deuxiemeChoix != 0)
            {
                await DifferentesCartes();
            }
            txtPointJ1.Text = _pointageJ1.ToString();
            txtPointJ2.Text = _pointageJ2.ToString();
            //Tour de l'ordinateur
            if (_bTourSysteme && !_bFinDePartie)
                ChoixOrdinateur();
        }

        private void MatchCartes()
        {
            lblEtat.Content = "";
            SonTournerCarte(3);

            //Retire les deux cartes
            _lstImages.Remove(_premierChoix);
            _lstImages.Remove(_deuxiemeChoix);

            //Désactive les boutons qui match
            foreach (Button item in Jeu.Children)
            {
                if (item.Name == _btn1)
                    item.IsEnabled = false;
                if (item.Name == _btn2)
                    item.IsEnabled = false;
            }
            //Gain de point
            if (_bTourJ1)
            {
                _pointageJ1++;
                _lstChoixJoueur1.Add(_btn1);
                _lstChoixJoueur1.Add(_btn2);
            }
            else
            {
                _pointageJ2++;
                _lstChoixSysteme.Add(_btn1);
                _lstChoixSysteme.Add(_btn2);
            }
            lblEtat.Content = ((_bTourJ1 ? txtNomJ1.Text : txtNomJ2.Text) + " marque un point!");
            _premierChoix = 0;
            _deuxiemeChoix = 0;
            DetectionFinDeJeu();
            if (_bFinDePartie)
            {

                //Fin de partie

                SonTournerCarte(2);
                Jeu.IsEnabled = false;
                ////Test de BD
                string combinaison1 = "";
                for (int i = 0; i < _lstChoixJoueur1.Count; i++)
                {
                    combinaison1 += _lstChoixJoueur1[i] + ",";
                }
                string combinaison2 = "";
                for (int i = 0; i < _lstChoixSysteme.Count; i++)
                {
                    combinaison2 += _lstChoixSysteme[i] + ",";
                }
                Partie partie = new Partie { dateHeurePartie = DateTime.Now.ToString(), idPartie = context.Partie.Count() };
                Utilisateur utilisateur1 = new Utilisateur { nomUser = _nomJ1, prenomUser = _prenomJ1, idUser = context.Utilisateur.Count()};
                context.Utilisateur.AddOrUpdate(utilisateur1);
                context.SaveChanges();
                Utilisateur utilisateur2 = new Utilisateur { nomUser = _nomJ2, prenomUser = _prenomJ2, idUser = context.Utilisateur.Count() +1};
                context.Utilisateur.AddOrUpdate(utilisateur2);
                context.SaveChanges();
                Etat etat;
                Etat etat2;
                //État
                if (_pointageJ1 > _pointageJ2)
                {
                    etat = new Etat { nomEtat = "J1Gagné", idEtat = 1 };
                    etat2 = new Etat { nomEtat = "J2Perdu", idEtat = 2 };
                }
                else if (_pointageJ1 < _pointageJ2)
                {
                    etat = new Etat { nomEtat = "J1Perdu", idEtat = 2 };
                    etat2 = new Etat { nomEtat = "J2Gagné", idEtat = 1 };
                }
                else
                {
                    etat = new Etat { nomEtat = "J1Nul", idEtat = 3 };
                    etat2 = new Etat { nomEtat = "J2Nul", idEtat = 3 };
                }
                context.Partie.Add(partie);
                context.Etat.AddOrUpdate(etat);
                context.Etat.AddOrUpdate(etat2);
                Jouer jouer = new Jouer { listeCombine = combinaison1, idEtat = etat.idEtat, idPartie = partie.idPartie, idUser = utilisateur1.idUser, Etat = etat, Partie=partie, Utilisateur = utilisateur1 };
                Jouer jouer2 = new Jouer { listeCombine = combinaison2, idEtat = etat2.idEtat, idPartie = partie.idPartie, idUser = utilisateur2.idUser, Etat = etat2, Partie = partie, Utilisateur = utilisateur2 };
                context.Jouer.Add(jouer);
                context.Jouer.Add(jouer2);
                context.SaveChanges();
                var query = context.Jouer.Where(u => u.idUser == utilisateur1.idUser).Select(z => z.idEtat).First();

                if (query == 1)
                    MessageBox.Show("Le vainqueur est: " + (_pointageJ1 > _pointageJ2 ? txtNomJ1.Text : txtNomJ2.Text) + "!");
                //MessageBox.Show("Le vainqueur est: " + (context.Utilisateur.Select(u => u.prenomUser == utilisateur1.prenomUser)) + (context.Utilisateur.Select(u => u.nomUser == utilisateur1.nomUser)) + "!");
                else if (query == 2)
                    //MessageBox.Show("Le vainqueur est: " + (context.Utilisateur.Select(u => u.prenomUser == utilisateur2.prenomUser)) + (context.Utilisateur.Select(u => u.nomUser == utilisateur2.nomUser)) + "!");
                    MessageBox.Show("Le vainqueur est: " + (_lstChoixJoueur1.Count < _lstChoixSysteme.Count ? txtNomJ1.Text : txtNomJ2.Text) + "!");
                else if (query == 3)
                {
                    //Liste des combinaisons
                    var listeCombi1 = context.Jouer.Where(u => u.idUser == utilisateur1.idUser).Select(l => l.listeCombine).First();
                    var listeCombi2 = context.Jouer.Where(u => u.idUser == utilisateur2.idUser).Select(l => l.listeCombine).First();
                    int nbPair = 0;
                    int nbPair2 = 0;
                    foreach (char item in listeCombi1)
                    {
                        if (item == ',')
                            nbPair++;
                    }
                    foreach (char item in listeCombi2)
                    {
                        if (item == ',')
                            nbPair2++;
                    }
                    if (nbPair != nbPair2)
                        MessageBox.Show(nbPair > nbPair2 ? txtNomJ1.Text : txtNomJ2.Text);
                    else
                        MessageBox.Show("Le match est nul");
                }
            }
        }

        private async Task DifferentesCartes()
        {
            lblEtat.Content = "";
            //Désactive les boutons
            ArretJeu();
            #region MÉLANGE 35
            if (_premierChoix == 35 || _deuxiemeChoix == 35 && (_premierChoix != 34 && _deuxiemeChoix != 34))
            {
                SonTournerCarte(35);
                lblEtat.Content += ("MÉLANGE: Le jeu se mélange!");
                _lstImages = _lstImages.OrderBy(c => rand.Next()).Select(c => c).ToList();
                MelangeCarte();
            }
            #endregion
            #region MAUDITE 30
            if (_premierChoix == 30 || _deuxiemeChoix == 30 && (_premierChoix != 34 && _deuxiemeChoix != 34))
            {
                SonTournerCarte(30);
                lblEtat.Content += ("MAUDITE: " + (_bTourJ1 ? txtNomJ1.Text : txtNomJ2.Text) + " perd 1 point et le jeu se mélange!");
                if (_bTourJ1 && _pointageJ1 > 0)
                    _pointageJ1--;
                else if ((_bTourJ2 || _bTourSysteme) && _pointageJ2 > 0)
                    _pointageJ2--;
                MelangeCarte();
            }
            #endregion
            #region MAUDITE 31
            if (_premierChoix == 31 || _deuxiemeChoix == 31 && (_premierChoix != 34 && _deuxiemeChoix != 34))
            {
                SonTournerCarte(31);
                lblEtat.Content += ("MAUDITE: " + (_bTourJ1 ? txtNomJ1.Text : txtNomJ2.Text) + " perd 1 point et le jeu se mélange!");
                if (_bTourJ1 && _pointageJ1 > 0)
                    _pointageJ1--;
                else if ((_bTourJ2 || _bTourSysteme) && _pointageJ2 > 0)
                    _pointageJ2--;
                MelangeCarte();
            }
            #endregion
            await Task.Delay(1000);
            {
                foreach (Button item in Jeu.Children)
                {
                    //Retourne les cartes
                    if (item.Name == _btn1)
                        item.Background = RecevoirCarteDefaut();

                    if (item.Name == _btn2)
                        item.Background = RecevoirCarteDefaut();
                }

            };
            #region JOKER 34
            if (_premierChoix == 34)
            {
                SonTournerCarte(34);
                lblEtat.Content += ("JOKER: " + (_bTourJ1 ? txtNomJ1.Text : txtNomJ2.Text) + " peut rejouer!");
                foreach (Button item in Jeu.Children)
                {
                    if (item.Name == _btn1)
                    {
                        item.IsEnabled = false;
                        break;
                    }
                }
                _lstImages.Remove(_premierChoix);
            }
            else if (_deuxiemeChoix == 34)
            {
                SonTournerCarte(34);
                lblEtat.Content += ("JOKER: " + (_bTourJ1 ? txtNomJ1.Text : txtNomJ2.Text) + " peut rejouer!");
                foreach (Button item in Jeu.Children)
                {
                    if (item.Name == _btn2)
                    {
                        item.IsEnabled = false;
                        break;
                    }
                }
                _lstImages.Remove(_deuxiemeChoix);
            }
            #endregion
            else
                ChangementTour();
            //Active les boutons
            ActivationJeu();
            _premierChoix = 0;
            _deuxiemeChoix = 0;
        }


        #region Boutons
        private void chk8x8_Checked(object sender, RoutedEventArgs e)
        {
            chk9x9.IsChecked = false;
        }

        private void chk9x9_Checked(object sender, RoutedEventArgs e)
        {
            chk8x8.IsChecked = false;
        }

        private void chkJoueur1_Checked(object sender, RoutedEventArgs e)
        {
            chkJoueur2.IsChecked = false;
            lblJ2.Content = "Système";
            txtNomJ2.Text = "Système";

        }

        private void chkJoueur2_Checked(object sender, RoutedEventArgs e)
        {
            chkJoueur1.IsChecked = false;
            lblJ2.Content = "Joueur 2";
            txtNomJ2.Text = "Joueur 2";
        }

        private void chkFruits_Checked(object sender, RoutedEventArgs e)
        {
            chkVoitures.IsChecked = false;
            _bTheme = false;
        }

        private void chkVoitures_Checked(object sender, RoutedEventArgs e)
        {
            chkFruits.IsChecked = false;
            _bTheme = true;
        }

        private void chkDebut2_Checked(object sender, RoutedEventArgs e)
        {
            chkDebut1.IsChecked = false;
        }

        private void chkDebut1_Checked(object sender, RoutedEventArgs e)
        {
            chkDebut2.IsChecked = false;
        }

        private void btnDemarrer_Click(object sender, RoutedEventArgs e)
        {

            InputBox.Visibility = System.Windows.Visibility.Visible;
            if ((chk8x8.IsChecked == true || chk9x9.IsChecked == true) && (chkDebut1.IsChecked == true || chkDebut2.IsChecked == true) && (chkFruits.IsChecked == true || chkVoitures.IsChecked == true) && (chkJoueur1.IsChecked == true || chkJoueur2.IsChecked == true))
            {
                btnRecommencer.IsEnabled = true;
                Options.IsEnabled = false;
                btnDemarrer.IsEnabled = false;

                if (chk8x8.IsChecked == true)
                    _bGrandeur = 8;
                else
                    _bGrandeur = 9;

                //1 à 29 pour les cartes, 30-31 pour maudites, 32-33 pour unique, 34 joker,35 aléatoire,
                for (int x = 1; x <= 29; x++)
                {
                    _lstImages.Add(x);
                    _lstImages.Add(x);
                }
                for (int i = 30; i <= 35; i++)
                {
                    _lstImages.Add(i);
                }
                if (_bGrandeur == 9)
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        _lstImages.Add(i);
                        _lstImages.Add(i);
                    }
                    _lstImages.Add(35);
                }

                //Mélange la liste
                _lstImages = _lstImages.OrderBy(c => rand.Next()).Select(c => c).ToList();
                txtPointJ1.Text = _pointageJ1.ToString();
                txtPointJ2.Text = _pointageJ2.ToString();

                //Création des boutons
                int iTag = 0;
                for (int x = 0; x < _bGrandeur; ++x)
                {
                    for (int y = 0; y < _bGrandeur; ++y)
                    {
                        string sNom = (x) + (y).ToString();
                        Button button = new Button()
                        {
                            Tag = _lstImages[iTag],
                            Name = "btn" + sNom,
                            Background = RecevoirInfoBouton(_lstImages[iTag]),
                            Focusable = false
                        };

                        button.Click += new RoutedEventHandler(button_Click);

                        Jeu.Children.Add(button);
                        iTag++;
                    }
                }
                //Tour de jeu
                if (chkJoueur1.IsChecked == true)
                    _bContreSysteme = true;
                else
                    _bContreSysteme = false;
                if (chkDebut1.IsChecked == true)
                    _bTourJ1 = true;
                else if (chkJoueur2.IsChecked == true && _bContreSysteme == false)
                    _bTourJ2 = true;
                else
                {
                    _bTourSysteme = true;
                    ChoixOrdinateur();
                }

            }
            else
                MessageBox.Show("Vous n'avez pas rempli la grille d'option.");


        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            SonTournerCarte(1);
            if (!_bTourSysteme)
            {
                _lstChoixJoueur1.Add((sender as Button).Name);
                int iTag = int.Parse(string.Format("{0}", (sender as Button).Tag));
                if (_premierChoix == 0)
                {
                    _premierChoix = int.Parse(string.Format("{0}", (sender as Button).Tag));
                    _btn1 = (sender as Button).Name;
                }
                else if (_deuxiemeChoix == 0)
                {
                    _deuxiemeChoix = int.Parse(string.Format("{0}", (sender as Button).Tag));
                    _btn2 = (sender as Button).Name;
                }
                (sender as Button).Background = RecevoirInfoBouton(iTag);
                (sender as Button).IsHitTestVisible = false;
                JeuMemoire();
            }
        }

        private void btnRecommencer_Click(object sender, RoutedEventArgs e)
        {
            ArretJeu();
            _prenomJ1 = "";
            _nomJ1 = "";
            _prenomJ2 = "";
            _nomJ2 = "";
            this.Jeu.Children.Clear();
            btnDemarrer.IsEnabled = true;
            Options.IsEnabled = true;
            _bTourSysteme = false;
            _bTourJ1 = false;
            _bTourJ2 = false;
            _deuxiemeChoix = 0;
            _premierChoix = 0;
            _pointageJ1 = 0;
            _pointageJ2 = 0;
            //Textboxes
            txtPointJ1.Text = "0";
            txtPointJ2.Text = "0";
            txtNomJ1.Background = Brushes.White;
            txtNomJ2.Background = Brushes.White;
            List<int> _lstImages = new List<int>();
            List<int> _lstRandom = new List<int>();

            //Liste des choix
            _lstChoixJoueur1 = new List<string>();
            _lstChoixSysteme = new List<string>();

        }
        #endregion
        private ImageBrush RecevoirInfoBouton(int tagBouton)
        {
            string sUri;
            if (!_bTheme)
            {
                sUri = "Images/Fruits/f";
            }
            else
            {
                sUri = "Images/Voitures/v";
            }
            sUri += tagBouton.ToString() + ".jpg";

            Uri resourceUri = new Uri(sUri, UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;
            return brush;
        }
        private ImageBrush RecevoirCarteDefaut()
        {
            Uri resourceUri = new Uri("Images/Cartes/HLion.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;
            return brush;
        }
        private void ArretJeu()
        {
            //Désactive les boutons
            foreach (Button item in Jeu.Children)
            {
                item.IsHitTestVisible = false;
            }

        }
        private void ActivationJeu()
        {
            //Désactive les boutons
            foreach (Button item in Jeu.Children)
            {
                item.IsHitTestVisible = true;
            }
        }
        private void ChangementTour()
        {
            if (_bContreSysteme)
            {
                _bTourSysteme = !_bTourSysteme;
                _bTourJ1 = !_bTourJ1;
                if (_bTourJ1)
                {
                    txtNomJ1.Background = Brushes.Red;
                    txtNomJ2.Background = Brushes.White;
                    ActivationJeu();
                }
                else
                {
                    txtNomJ1.Background = Brushes.White;
                    txtNomJ2.Background = Brushes.Red;
                }

            }
            else
            {
                if (_bTourJ1)
                {
                    txtNomJ1.Background = Brushes.White;
                    txtNomJ2.Background = Brushes.Red;
                }
                else
                {
                    txtNomJ1.Background = Brushes.Red;
                    txtNomJ2.Background = Brushes.White;
                }
                _bTourJ1 = !_bTourJ1;
                _bTourJ2 = !_bTourJ2;
            }
        }
        private async void ChoixOrdinateur()
        {
            ArretJeu();
            //Construit une liste des boutons disponibles
            List<Button> lstBoutonDisponible = new List<Button>();
            foreach (Button item in Jeu.Children)
            {
                if (item.IsEnabled)
                {
                    lstBoutonDisponible.Add(item);
                }
            }
            bool bDifferentBoutonsChoisis = true;
            _btn1 = lstBoutonDisponible.ElementAt(rand.Next(0, lstBoutonDisponible.Count)).Name;
            //Empêche qu'il choisisse le même bouton.
            while (bDifferentBoutonsChoisis)
            {
                _btn2 = lstBoutonDisponible.ElementAt(rand.Next(0, lstBoutonDisponible.Count)).Name;
                if (_btn1 != _btn2)
                    bDifferentBoutonsChoisis = false;
            }

            //Cherche la valeur des boutons choisis
            foreach (Button item in Jeu.Children)
            {
                if (item.Name == _btn1)
                {
                    await Task.Delay(1000);
                    {
                        int iTag = int.Parse(string.Format("{0}", item.Tag));
                        item.Background = RecevoirInfoBouton(iTag);
                        _premierChoix = iTag;
                        _lstChoixSysteme.Add(_btn1);
                    }
                }
                if (item.Name == _btn2)
                {
                    await Task.Delay(1000);
                    {
                        int iTag = int.Parse(string.Format("{0}", item.Tag));
                        item.Background = RecevoirInfoBouton(iTag);
                        _deuxiemeChoix = iTag;
                        _lstChoixSysteme.Add(_btn2);
                    }
                }

            }
            JeuMemoire();
        }
        private void MelangeCarte()
        {
            int i = 0;
            _lstImages = _lstImages.OrderBy(c => rand.Next()).Select(c => c).ToList();
            foreach (Button item in Jeu.Children)
            {
                if (item.IsEnabled)
                {
                    item.Tag = _lstImages[i];
                    ++i;
                }
            }
        }
        private void DetectionFinDeJeu()
        {
            int iDetectionFin = 0;
            for (int x = 1; x <= 29; x++)
            {
                if (_lstImages.Contains(x))
                {
                    iDetectionFin++;
                }
            }
            if (iDetectionFin == 0)
            {
                _bFinDePartie = true;
            }
        }
        private void SonTournerCarte(int idCarte)
        {
            if (idCarte == 1)
                _player.Open(new Uri("Sounds/card-flip.wav", UriKind.Relative));
            else if (idCarte == 30 || idCarte == 31)
                _player.Open(new Uri("Sounds/evil-laugh.mp3", UriKind.Relative));
            else if (idCarte == 34)
                _player.Open(new Uri("Sounds/joker.wav", UriKind.Relative));
            else if (idCarte == 35)
                _player.Open(new Uri("Sounds/melange.mp3", UriKind.Relative));
            else if (idCarte == 2)
                _player.Open(new Uri("Sounds/percussion-choir-final.wav", UriKind.Relative));
            else if (idCarte == 3)
                _player.Open(new Uri("Sounds/point.mp3", UriKind.Relative));
            _player.Play();
        }
        private void ConfirmerButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = System.Windows.Visibility.Collapsed;

            _prenomJ1 = txtJ1Prenom.Text;
            _nomJ1 = txtJ1Nom.Text;
            if (!_bContreSysteme)
            {
                txtInputJ2.Visibility = Visibility.Visible;
                txtJ2Nom.Visibility = Visibility.Visible;
                txtJ2Prenom.Visibility = Visibility.Visible;
                _nomJ2 = txtJ2Nom.Text;
                _prenomJ2 = txtJ2Prenom.Text;
            }
            else
            {
                _nomJ2 = "Système";
                _prenomJ2 = "Système";
            }
            //// Clear InputBox.
            //txtJ1InputNom.Text = String.Empty;
        }
    }
}

