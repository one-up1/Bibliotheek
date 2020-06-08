using System;
using System.Windows;
using System.Windows.Input;

namespace Bibliotheek
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbIsbn.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Calc();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Calc();
        }

        private void Calc()
        {
            // Valideer boeknummer.
            string isbn = tbIsbn.Text;
            try
            {
                if (isbn.Length != 4)
                {
                    throw new FormatException("Invalid ISBN");
                }
                int.Parse(isbn);
            }
            catch
            {
                MessageBox.Show("Het boeknummer moet bestaan uit 4 cijfers", "Bibliotheek",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                tbIsbn.Focus();
                return;
            }

            // Valideer uitleendatum/retourdatum.
            if (dpRentalDate.SelectedDate == null)
            {
                dpRentalDate.Focus();
                return;
            }
            if (dpReturnDate.SelectedDate == null)
            {
                dpReturnDate.Focus();
                return;
            }
            DateTime rentalDate = dpRentalDate.SelectedDate.Value;
            DateTime returnDate = dpReturnDate.SelectedDate.Value;

            // Bepaal aantal dagen.
            double days = (returnDate - rentalDate).TotalDays;
            if (days <= 0)
            {
                MessageBox.Show("De retourdatum moet na de uitleendatum liggen", "Bibliotheek",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                dpReturnDate.Focus();
                return;
            }

            // Bereken aantal dagen te laat en eventuele boete.
            double daysOverdue, fine = 0;
            if (isbn[0] == '9')
            {
                // Romans: Als dagen > 21 een boete van € 0,25 per dag.
                daysOverdue = days - 21;
                if (daysOverdue > 0)
                {
                    fine = daysOverdue * 0.25;
                }
            }
            else
            {
                // Studieboeken: Als dagen > 30 een boete van € 1,= per week.
                // Een deel van een week telt hierbij als volle week:
                // stel dagen = 39, dus 9 dagen te laat, 2 x € 1,= boete.
                daysOverdue = days - 30;
                if (daysOverdue > 0)
                {
                    fine = Math.Ceiling(daysOverdue / 7);
                }
            }

            // Toon resultaat.
            string caption = FormatDayCount(days);
            if (fine == 0)
            {
                MessageBox.Show("Geen boete!", caption,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(FormatDayCount(daysOverdue) + " te laat, € " +
                    fine.ToString("0.00") + " boete!", caption,
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static string FormatDayCount(double days)
        {
            string ret = days.ToString() + " dag";
            if (days != 1)
            {
                ret += "en";
            }
            return ret;
        }
    }
}
