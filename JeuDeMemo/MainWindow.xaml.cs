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
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;
using System.Windows.Controls.Primitives;

namespace JeuDeMemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Tours
        bool _bTourSysteme = false;
        bool _bContreSysteme = false;
        bool _bTourJ1 = false;
        bool _bTourJ2 = false;
        int _pointageJ1 = 0;
        int _pointageJ2 = 0;
        byte _bGrandeur = 0;
        int _premierChoix = 0;
        int _deuxiemeChoix = 0;
        //Noms des boutons
        string _btn1;
        string _btn2;
        bool _bTheme = false; //false = fruit      true = voiture

        List<int> _lstImages = new List<int>();
        List<int> _lstRandom = new List<int>();
        Random rand = new Random();
        int _iChoixSysteme;
        bool _bFinDePartie = false;
        public MainWindow()
        {
            InitializeComponent();

        }
        public async void JeuMemoire()
        {
            //Match des cartes
            if (_premierChoix == _deuxiemeChoix)
            {
                //Retourne les deux cartes
                foreach (var item in _lstRandom)
                {
                    if (item == _premierChoix)
                        _lstImages.Remove(item);
                }
                foreach (var item in Jeu.Children)
                {
                    Button btn = (Button)item;
                    if (btn.Name == _btn1)
                    {
                        btn.IsEnabled = false;
                    }
                    if (btn.Name == _btn2)
                    {
                        btn.IsEnabled = false;
                    }
                }
                //Gain de point
                if (_bTourJ1)
                {
                    _pointageJ1++;
                    lblJ1Point.Content = _pointageJ1;
                }
                else
                {
                    _pointageJ2++;
                    lblJ2Point.Content = _pointageJ2;
                }
                _premierChoix = 0;
                _deuxiemeChoix = 0;
                ChangementTour();
            }
            //Différentes cartes
            else if (_premierChoix != 0 && _deuxiemeChoix != 0)
            {
                //Désactive les boutons
                ArretJeu();

                await Task.Delay(1000);
                {
                    foreach (var item in Jeu.Children)
                    {
                        //Retourne les cartes
                        Button btn = (Button)item;

                        if (btn.Name == _btn1)
                            btn.Background = RecevoirCarteDefaut();

                        if (btn.Name == _btn2)
                            btn.Background = RecevoirCarteDefaut();
                    }
                    //Active les boutons
                    ActivationJeu();
                };
                _premierChoix = 0;
                _deuxiemeChoix = 0;
                ChangementTour();
            }
            if (_bTourSysteme)
                ChoixOrdinateur();
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

        }

        private void chkJoueur2_Checked(object sender, RoutedEventArgs e)
        {
            chkJoueur1.IsChecked = false;
            lblJ2.Content = "Joueur 2";
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
            if ((chk8x8.IsChecked == true || chk9x9.IsChecked == true) && (chkDebut1.IsChecked == true || chkDebut2.IsChecked == true) && (chkFruits.IsChecked == true || chkVoitures.IsChecked == true) && (chkJoueur1.IsChecked == true || chkJoueur2.IsChecked == true))
            {
                btnRecommencer.IsEnabled = true;
                int iTag = 0;
                Options.IsEnabled = false;
                btnDemarrer.IsEnabled = false;

                if (chk8x8.IsChecked == true)
                {
                    _bGrandeur = 8;
                }
                else
                {
                    _bGrandeur = 9;
                }
                for (int x = 1; x <= _bGrandeur; x++)
                {
                    for (int y = 1; y <= _bGrandeur; y++)
                    {
                        string sNom = (x) + (y).ToString();
                        Button button = new Button()
                        {
                            Tag = iTag,
                            Name = "btn" + sNom,
                            Background = RecevoirCarteDefaut(),
                            Focusable = false
                        };

                        button.Click += new RoutedEventHandler(button_Click);

                        Jeu.Children.Add(button);
                        iTag++;
                    }
                }
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
                _lstRandom = _lstImages.OrderBy(c => rand.Next()).Select(c => c).ToList();
                //Tour de jeu
                if (chkJoueur1.IsChecked == true)
                    _bContreSysteme = true;
                if (chkDebut1.IsChecked == true)
                    _bTourJ1 = true;
                else if (chkJoueur2.IsChecked == true && _bContreSysteme == false)
                    _bTourJ2 = true;
                else
                    _bTourSysteme = true;
            }
            else
                MessageBox.Show("Vous n'avez pas rempli la grille d'option.");
            

        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            if (!_bTourSysteme)
            {
                int iTag = int.Parse(string.Format("{0}", (sender as Button).Tag));
                if (_premierChoix == 0)
                {
                    _premierChoix = _lstRandom[int.Parse(string.Format("{0}", (sender as Button).Tag))];
                    _btn1 = (sender as Button).Name;
                }
                else if (_deuxiemeChoix == 0)
                {
                    _deuxiemeChoix = _lstRandom[int.Parse(string.Format("{0}", (sender as Button).Tag))];
                    _btn2 = (sender as Button).Name;
                }
                (sender as Button).Background = RecevoirInfoBouton(iTag);
                (sender as Button).IsHitTestVisible = false;
                JeuMemoire();
            }
        }

        private void btnRecommencer_Click(object sender, RoutedEventArgs e)
        {
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
            List<int> _lstImages = new List<int>();
            List<int> _lstRandom = new List<int>();
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
            int numBouton = tagBouton;
            sUri += _lstRandom[numBouton].ToString() + ".jpg";

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
            foreach (var item in Jeu.Children)
            {
                Button btn = (Button)item;
                btn.IsHitTestVisible = false;
            }
        }
        private void ActivationJeu()
        {
            //Désactive les boutons
            foreach (var item in Jeu.Children)
            {
                Button btn = (Button)item;
                btn.IsHitTestVisible = true;
            }
        }
        private void ChangementTour()
        {
            if(_bContreSysteme)
            {
                _bTourSysteme = !_bTourSysteme;
                _bTourJ1 = !_bTourJ1;
            }
            else
            {
                _bTourJ1 = !_bTourJ1;
                _bTourJ2 = !_bTourJ2;
            }
        }
        private void ChoixOrdinateur()
        {
            _btn1 = "btn" + rand.Next(1,_lstImages.Count);
            foreach (var item in Jeu.Children)
            {
                if ((item as Button).Name == _btn1)
                {
                    int iTag = int.Parse(string.Format("{0}", (item as Button).Tag));
                    (item as Button).Background = RecevoirInfoBouton(iTag);
                    _premierChoix = iTag;
                }
                    
            }

            _btn2 = "btn" + rand.Next(1,_lstImages.Count);
            foreach(var item in Jeu.Children)
            {
                if ((item as Button).Name == _btn2)
                {
                    int iTag = int.Parse(string.Format("{0}", (item as Button).Tag));
                    (item as Button).Background = RecevoirInfoBouton(iTag);
                    _deuxiemeChoix = iTag;
                }

            }
            JeuMemoire();

        }
    }
}

