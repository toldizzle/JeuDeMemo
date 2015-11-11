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

namespace JeuDeMemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte _bGrandeur = 0;
        int _premierChoix = 0;
        int _deuxiemeChoix = 0;
        List<int> _lstImages = new List<int>();
        List<int> _lstRandom = new List<int>();
        Random rand = new Random();
        public MainWindow()
        {
            InitializeComponent();
        }
        public void JeuMemoire()
        {
            bool bFin = false;
            
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
            //while (!bFin)
            //{

            //}
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
        }

        private void chkJoueur2_Checked(object sender, RoutedEventArgs e)
        {
            chkJoueur1.IsChecked = false;
        }

        private void chkFruits_Checked(object sender, RoutedEventArgs e)
        {
            chkVoitures.IsChecked = false;
        }

        private void chkVoitures_Checked(object sender, RoutedEventArgs e)
        {
            chkFruits.IsChecked = false;
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
            int iTag = 0;
            Options.IsEnabled = false;
            btnDemarrer.IsEnabled = false;
            Uri resourceUri = new Uri("Images/Cartes/HLion.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;

            if (chk8x8.IsChecked == true)
            {
                _bGrandeur = 8;
            }
            else if (chk9x9.IsChecked == true)
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
                        Name = "btn" + sNom,
                        Tag = iTag,
                        Background = brush
                    };
                    button.Click += new RoutedEventHandler(button_Click);
                    this.Jeu.Children.Add(button);
                    iTag++;
                }
            }
            JeuMemoire();

        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            int iTag = int.Parse(string.Format("{0}", (sender as Button).Tag));
            if (_premierChoix == 0)
                _premierChoix = int.Parse(string.Format("{0}", (sender as Button).Tag));
            else
                _deuxiemeChoix = int.Parse(string.Format("{0}", (sender as Button).Tag));
            if(_premierChoix != 0 && _deuxiemeChoix != 0)
            {
                System.Threading.Thread.Sleep(50);
                _premierChoix = 0;
                _deuxiemeChoix = 0;
            }
            //Change l'image
            Uri resourceUri = new Uri(RecevoirInfoBouton(iTag), UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;
            (sender as Button).Background = brush;
        }

        private void btnRecommencer_Click(object sender, RoutedEventArgs e)
        {
            this.Jeu.Children.Clear();
            btnDemarrer.IsEnabled = true;
            Options.IsEnabled = true;
        }
        #endregion
        private string RecevoirInfoBouton(int tagBouton)
        {
            string sUri = "Images/Fruits/f";
            int nbImage = tagBouton;
            sUri += _lstRandom[nbImage].ToString() + ".jpg";
            return sUri;
        }
    }
}

