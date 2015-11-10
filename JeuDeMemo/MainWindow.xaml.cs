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
        byte bGrandeur = 0;
        int premierChoix = 0;
        int deuxiemeChoix = 0;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void JeuMemoire()
        {
            bool bFin = false;
            List<int> lstImages = new List<int>();
            //1 à 29 pour les cartes, 30 pour maudites, 31 pour unique, 32 joker,33 aléatoire,
            for (int x = 1; x <= 31; x++)
            {
                lstImages.Add(x);
                lstImages.Add(x);
            }
            lstImages.Add(32);
            lstImages.Add(33);
            Random rand = new Random();
            var lstRandom = lstImages.OrderBy(c => rand.Next()).Select(c => c).ToList();
            //MessageBox.Show(lstImages[33].ToString());
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
            Options.IsEnabled = false;
            btnDemarrer.IsEnabled = false;
            Uri resourceUri = new Uri("Images/Cartes/HLion.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;

            if (chk8x8.IsChecked == true)
            {
                bGrandeur = 8;
            }
            else if (chk9x9.IsChecked == true)
            {
                bGrandeur = 9;
            }
            for (int x = 1; x <= bGrandeur; x++)
            {
                for (int y = 1; y <= bGrandeur; y++)
                {
                    string sNom = (x) + (y).ToString();
                    Button button = new Button()
                    {
                        Name = "btn" + sNom,
                        Tag = sNom,
                        Background = brush
                    };
                    button.Click += new RoutedEventHandler(button_Click);
                    this.Jeu.Children.Add(button);
                }
            }
            JeuMemoire();

        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            if (premierChoix == 0)
                premierChoix = int.Parse(string.Format("{0}", (sender as Button).Tag));
            else
                deuxiemeChoix = int.Parse(string.Format("{0}", (sender as Button).Tag));
            if(premierChoix != 0 && deuxiemeChoix != 0)
            {
                System.Threading.Thread.Sleep(50);
                premierChoix = 0;
                deuxiemeChoix = 0;
            }
            //Créer une méthode pour recevoir un uri.
            Uri resourceUri = new Uri("Images/Cartes/HLion.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;
            (sender as Button).Background = brush;
            //MessageBox.Show(string.Format("{0}", (sender as Button).Tag));
        }

        private void btnRecommencer_Click(object sender, RoutedEventArgs e)
        {
            this.Jeu.Children.Clear();
            btnDemarrer.IsEnabled = true;
            Options.IsEnabled = true;
        }
        #endregion

    }
}

