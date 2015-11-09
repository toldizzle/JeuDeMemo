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

namespace JeuDeMemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }




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
            btnDemarrer.IsEnabled = false;
            Uri resourceUri = new Uri("Images/Cartes/HLion.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
            var brush = new ImageBrush();
            brush.ImageSource = temp;

            byte bGrandeur = 0;
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
                    string sNom = "btn" + x + y;
                    Button button = new Button()
                    {
                        Name = sNom,
                        Tag = sNom,
                        Background = brush
                    };
                    button.Click += new RoutedEventHandler(button_Click);
                    this.Jeu.Children.Add(button);
                }
            }
        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format("{0}", (sender as Button).Tag));
        }
    }
}
