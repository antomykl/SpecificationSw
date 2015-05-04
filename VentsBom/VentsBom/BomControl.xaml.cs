using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataGridFilterLibrary;
using SwBomDll;

namespace VentsBom
{

    delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);

    /// <summary>
    /// Interaction logic for BomControl.xaml
    /// </summary>
    public partial class BomControl
    {
        private readonly BackgroundWorker _backgroundWorker;

        public BomControl()
        {
            InitializeComponent();

            GenerateBom.Visibility = Visibility.Collapsed;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += BackgroundWorkerDoWork;

            ProgressBar1.Visibility = Visibility.Collapsed;
            SettingsOther.Visibility = Visibility.Collapsed;
            FontsGroupBox.Visibility = Visibility.Collapsed;

            //GroupTab.IsEnabled = false;
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            LoadBom1();
        }

        private BomSpec _bomSpec;

        private List<BomSpec.BomData> ListAllBom1 { get; set; }

        private List<List<BomSpec.BomData>> ListDivBom1 { get; set; }

        private static void NumberValidationTextBox(TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Sp1Textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            NumberValidationTextBox(e);
        }

        private void Sp1Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Sp1Textbox.Text)) return;
            BomSpec.Settings.КолСтрокА41 = Convert.ToInt32(Sp1Textbox.Text);
            FullBom.IsChecked = true;
        }

        private void Sp2Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Sp2Textbox.Text)) return;
            BomSpec.Settings.КолСтрокА42 = Convert.ToInt32(Sp2Textbox.Text);
            FullBom.IsChecked = true;
        }

        private void Sp1TemplateTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Sp1TemplateTextbox.Text)) return;
            BomSpec.Settings.Sp1 = Sp1TemplateTextbox.Text;
        }

        private void Sp2TemplateTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Sp2TemplateTextbox.Text)) return;
            BomSpec.Settings.Sp2 = Sp2TemplateTextbox.Text;
        }

        private void GenerateBom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadBom1();
                FullBom.IsChecked = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        public static DependencyProperty DefaultFilterProperty =
         DependencyProperty.RegisterAttached("DefaultFilter",
             typeof(string), typeof(DataGridColumnExtensions),
               new FrameworkPropertyMetadata(string.Empty));

        public static string GetDefaultFilter(DependencyObject target)
        {
            return (string)target.GetValue(DefaultFilterProperty);
        }

        public static void SetDefaultFilter(DependencyObject target, string value)
        {
            target.SetValue(DefaultFilterProperty, value);
        }

        private void LoadBom1()
        {
            try
            {
                _bomSpec = new BomSpec();
                Settings_Loaded(null, null);
                _bomSpec.PrepereBomTableOnSheet();
                ListAllBom1 = _bomSpec.GetBomFromTableAnnotaiton;
                BomTable1All.ItemsSource = ListAllBom1;

                #region to delete
                //var newStyle = new Style
                //{
                //    TargetType = typeof (DataGridColumnHeader),
                //    BasedOn = (Style)FindResource(typeof(DataGridHeaderFilterControl))
                //};
                
                //var setter = new Setter
                //{
                //    Property = BackgroundProperty,
                //    Value = new SolidColorBrush(
                //    Color.FromArgb(0x20, 0x80, 0x80, 0x80))
                //};
                //newStyle.Setters.Add(setter);

                //BomTable1All.ColumnHeaderStyle = newStyle;
                #endregion

                BomTable1All1.ItemsSource = ListAllBom1;
                if (ListAllBom1 != null) ListDivBom1 = _bomSpec.BomsByPage(ListAllBom1);
                Sheets.Visibility = Visibility.Hidden;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.StackTrace + " LoadBom1");
            }
        }

        private void UpdateBom1()
        {
            BomTable1All.ItemsSource = ListAllBom1;
            //if (ListAllBom1 != null) ListDivBom1 = _bomSpec.BomsByPage(ListAllBom1);
            if (Sheets != null) Sheets.Visibility = Visibility.Hidden;
        }

        private void AddToDocument_Click(object sender, RoutedEventArgs e)
        {
            // Settings_Loaded(null, null);
            try
            {
                _bomSpec.InsertBomTableOnSheet(5);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Sheets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = Sheets.SelectedItem as ComboBoxItem;
            if (item == null) return;
            try
            {
                var sheetN = Convert.ToInt32(item.Content.ToString().Replace("Лист", ""));
                BomTable1All.ItemsSource = ListDivBom1[sheetN - 1];
            }
            catch (Exception)
            {
                BomTable1All.ItemsSource = ListAllBom1;
            }
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var source = (List<BomSpec.BomData>) BomTable1All.ItemsSource;
                source.Insert(BomTable1All.SelectedIndex,
                    new BomSpec.BomData());
                ListAllBom1 = source;
                BomTable1All.ItemsSource = null;
                BomTable1All.ItemsSource = ListAllBom1;
                BomTable1All.ScrollIntoView(new object());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var source = (List<BomSpec.BomData>) BomTable1All.ItemsSource;
                source.RemoveAt(BomTable1All.SelectedIndex);
                ListAllBom1 = source;
                BomTable1All.ItemsSource = null;
                BomTable1All.ItemsSource = ListAllBom1;
                BomTable1All.ScrollIntoView(new object());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Sp1Textbox.Text = Convert.ToString(BomSpec.Settings.КолСтрокА41);
                Sp2Textbox.Text = Convert.ToString(BomSpec.Settings.КолСтрокА42);
                Sp1TemplateTextbox.Text = BomSpec.Settings.Sp1;
                Sp2TemplateTextbox.Text = BomSpec.Settings.Sp2;


                CharSpasingFactor.Text = Convert.ToString(BomSpec.Settings.ПлотностьШрифта);
                FontHeight.Text = Convert.ToString(BomSpec.Settings.ВысотаШрифта);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        private void FullBom_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateBom1();
                if (FullBom.IsChecked != true) return;
                if (GenerateBom != null) GenerateBom.IsEnabled = true;
                if (AddRow != null) AddRow.IsEnabled = true;
                if (DeleteRow != null) DeleteRow.IsEnabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void FullBom_Unchecked(object sender, RoutedEventArgs e)
        {
            if (FullBom.IsChecked != false) return;
            GenerateBom.IsEnabled = false;
            AddRow.IsEnabled = false;
            DeleteRow.IsEnabled = false;
        }

        private void DivededBom_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                BomSpec.Settings.КолСтрокА41 = Convert.ToInt32(Sp1Textbox.Text);
                BomSpec.Settings.КолСтрокА42 = Convert.ToInt32(Sp2Textbox.Text);

                if (ListAllBom1 != null) ListDivBom1 = _bomSpec.BomsByPage(ListAllBom1);

                if (ListDivBom1 == null) return;
                if (ListDivBom1.Count <= 0) return;
                for (var i = 1; i < ListDivBom1.Count + 1; i++)
                {
                    Sheets.Items.Add(new ComboBoxItem {Content = "Лист" + i});
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                Sheets.SelectedIndex = 0;
            }
            Sheets.Visibility = Visibility.Visible;
        }

        private void DivededBom_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                Sheets.Items.Clear();
                Sheets.Visibility = Visibility.Hidden;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            //ProgressBar1.Visibility = Visibility.Visible;
            //ProgressBar1.Maximum = 100;
            //ProgressBar1.Value = 0;
            //_backgroundWorker.RunWorkerAsync();

            LoadBom1();
        }

        private void АвтопроставлениеПозиций_Checked(object sender, RoutedEventArgs e)
        {
            BomSpec.Settings.АвтоПроставлениеПозиций = Convert.ToBoolean(АвтопроставлениеПозиций.IsChecked);
            FullBom.IsChecked = true;
        }

        private void АвтопроставлениеПозиций_Unchecked(object sender, RoutedEventArgs e)
        {
            BomSpec.Settings.АвтоПроставлениеПозиций = Convert.ToBoolean(АвтопроставлениеПозиций.IsChecked);
            FullBom.IsChecked = true;
        }

        private void BomTable1All_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = (BomSpec.BomData) BomTable1All.SelectedItem;
            if (row == null) return;
            DeleteRow.IsEnabled = row.МожноУдалять;
            AddRow.IsEnabled = (row.Раздел == "Прочие изделия" || row.Раздел == "Стандартные изделия");
        }

        private void GetSpec_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bomSpec = new BomSpec();
                _bomSpec.AddBaseSpecification();
                //_bomSpec.PrepereBomTableOnSheet();
                //var bomFromTableAnnotaiton = _bomSpec.GetBomFromTableAnnotaiton;
                //_bomSpec.InsertBomTableOnSheet(5);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.StackTrace, "edg");
            }
            
        }
    }
}
