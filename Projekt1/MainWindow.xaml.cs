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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Drawing;
using System.IO;

namespace Projekt1
{
    public partial class MainWindow : Window
    {
        public string filename; //ścieżka zdjęcia użytkownika
        System.Drawing.Image image1; //zdjęcie użytkownika
        System.Drawing.Image image2 = System.Drawing.Image.FromFile(@"..\..\images\text1.png"); //napis Wesołych Świąt
        System.Drawing.Image image2smaller; //pomniejszony image2
        int vAlign = 0; //położenie w pionie: 0 - góra, 1 - środek, 2 - dół
        int hAlign = 1; //położenie w poziomie: 0 - lewo, 1 - środek, 2 - prawo
        double size = 1; //rozmiar zdjęcia (slider) z zakresu 0.1-1 z krokiem 0.1
        string[] osoby = System.IO.File.ReadAllLines(@"..\..\wykazOsob.txt"); //tablica z wykazem osób, sczytanym z pliku tekstowego
        string osoba; //string zawierający imię i nazwisko wybranej osoby
        bool displayWishes = true; //czy wyswietlać napis "życzy ..."

        public MainWindow()
        {
            InitializeComponent();
            listBoxOsoby.ItemsSource = osoby;
            osoba = osoby[0];
        }

        //Przycisk "Otwórz"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Okno dialogowe
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //Ustawienie filtrów rozszerzeń
            dlg.Filter = "Pliki obrazów (*png, *.jpg, *.bmp, *.jpeg)|*png; *.jpg; *.bmp; *.jpeg";

            //Wyświetlenie okna dialogowego
            Nullable<bool> result = dlg.ShowDialog();

            //Wczytanie zdjęcia
            if (result == true)
            {
                filename = dlg.FileName;
                textBox1.Text = filename;
                Uri uri = new Uri(filename, UriKind.Absolute);
                ImageSource imgSource = new BitmapImage(uri);
                podglad.Source = imgSource;

                addButton.IsEnabled = true;
            }
        }

        //Po przeciągnięciu zdjęcia nad okno
        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                if (fileNames.Length == 1)
                {
                    var file = new FileInfo(fileNames[0]);
                    if (file.Extension.ToLower() == ".png" ||
                        file.Extension.ToLower() == ".bmp" ||
                        file.Extension.ToLower() == ".jpg" ||
                        file.Extension.ToLower() == ".jpeg")
                    {
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        return;
                    }
                }
            }
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        //Po upuszczeniu zdjęcia
        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                filename = fileNames[0];
                textBox1.Text = filename;
                podglad.Source = new BitmapImage(new Uri(filename, UriKind.Absolute));
                Application.Current.MainWindow.Activate();
                addButton.IsEnabled = true;
            }
        }

        //Przycisk "Dodaj życzenia"
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            image1 = System.Drawing.Image.FromFile(filename);
            Bitmap bitmap = new Bitmap(image1.Width, image1.Height);

            //Pomniejszenie napisu, jeśli jest większy od zdjęcia
            image2smaller = image2;
            Bitmap objBitmap;
            while (image2smaller.Width > image1.Width || image2smaller.Height > image1.Height)
            {
                objBitmap = new Bitmap(image2smaller, new System.Drawing.Size((int)(image2smaller.Width / 1.2), (int)(image2smaller.Height / 1.2)));
                image2smaller = objBitmap;
            }

            //Pomniejszenie napisu sliderem
            objBitmap = new Bitmap(image2smaller, new System.Drawing.Size((int)(image2smaller.Width * size), (int)(image2smaller.Height * size)));
            image2smaller = objBitmap;

            Bitmap bitmapa1 = new Bitmap(image1);
            Bitmap bitmapa2 = new Bitmap(image2smaller);
            int horiAlign = 0;
            int vertAlign = 0;

            //Położenie pionowe napisu
            if (vAlign == 0)
                vertAlign = 0;
            else if (vAlign == 1)
                vertAlign = image1.Height / 2 - image2smaller.Height / 2;
            else if (vAlign == 2)
                vertAlign = image1.Height - image2smaller.Height;

            //Położenie poziome napisu
            if (hAlign == 0)
                horiAlign = 0;
            else if (hAlign == 1)
                horiAlign = (image1.Width / 2) - (image2smaller.Width / 2);
            else if (hAlign == 2)
                horiAlign = image1.Width - image2smaller.Width;

            //Napis "życzy [osoba]"
            string wishes;
            float x = 0;
            float y = image1.Height - (int)(image1.Height / 10);
            //Jeśli "Wesołych Świąt" na dole, to napis "życzy [osoba]" na górę
            if (vAlign == 2)
            {
                wishes = osoba + " życzy";
                y = 0;
            }
            else
            {
                wishes = "życzy " + osoba;
                y = image1.Height - (int)(image1.Height / 13);
            }
            System.Drawing.Font drawFont = new System.Drawing.Font("Arial", (int)(image1.Height / 20));
            System.Drawing.Color c = bitmapa1.GetPixel((int)x, (int)y);
            System.Drawing.SolidBrush drawBrush;
            //Automatyczna zmiana koloru napisu "życzy [osoba]" na podstawie koloru tła
            if (c.R>100 && c.G>100 && c.B>100)
                drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            else
                drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();

            //Rysowanie elementów
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(bitmapa1, 0, 0);
                g.DrawImage(bitmapa2, horiAlign, vertAlign);
                if (displayWishes)
                    g.DrawString(wishes, drawFont, drawBrush, x, y, drawFormat);
            }

            //Wyswietlenie gotowej kartki
            podglad.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        //Przycisk "Zapisz jako"
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "Image (.jpg)|*.jpg|Image (.png)|*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)podglad.Source));
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
        }

        //ComboBox z wyborem napisu
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)stringTypes.SelectedValue;
            var content = (string)item.Content;
            if (content == "Prosty")
                image2 = System.Drawing.Image.FromFile(@"..\..\images\text1.png");
            if (content == "Z prezentami")
                image2 = System.Drawing.Image.FromFile(@"..\..\images\text2.png");
            if (content == "Z mikołajem")
                image2 = System.Drawing.Image.FromFile(@"..\..\images\text3.png");
            if (content == "Z dzwonkami")
                image2 = System.Drawing.Image.FromFile(@"..\..\images\text4.png");
            if (content == "Z saniami")
                image2 = System.Drawing.Image.FromFile(@"..\..\images\text5.png");
        }

        //ComboBox z wyborem umiejscowienia w pionie
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)vertAlignBox.SelectedValue;
            var content = (string)item.Content;
            if (content == "Góra")
                vAlign = 0;
            else if (content == "Środek")
                vAlign = 1;
            else if (content == "Dół")
                vAlign = 2;
        }

        //ComboBox z wyborem umiejscowienia w poziomie
        private void horiAlignBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)horiAlignBox.SelectedValue;
            var content = (string)item.Content;
            if (content == "Lewo")
                hAlign = 0;
            else if (content == "Środek")
                hAlign = 1;
            else if (content == "Prawo")
                hAlign = 2;
        }

        //Slider z wielkością napisu
        private void sizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            size = sizeSlider.Value / 100;
        }

        //ListBox z wykazem osób
        private void listBoxOsoby_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            osoba = listBoxOsoby.SelectedItem.ToString();
        }

        //Przycisk do dodania osoby
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string text = "\n" + nowaOsobaBox.Text;
            System.IO.File.AppendAllText(@"..\..\wykazOsob.txt", text);
            osoby = System.IO.File.ReadAllLines(@"..\..\wykazOsob.txt");
            listBoxOsoby.ItemsSource = osoby;
            nowaOsobaBox.Text = "";
        }

        //Czyszczenie pola "Dodaj..." po kliknięciu
        private void nowaOsobaBox_GotFocus(object sender, RoutedEventArgs e)
        {
            nowaOsobaBox.Text = "";
        }

        //Dodadawanie osoby przyciskiem Enter
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string text = "\n" + nowaOsobaBox.Text;
                System.IO.File.AppendAllText(@"..\..\wykazOsob.txt", text);
                osoby = System.IO.File.ReadAllLines(@"..\..\wykazOsob.txt");
                listBoxOsoby.ItemsSource = osoby;
                nowaOsobaBox.Text = "";
            }
        }

        //Checkbox odznaczony
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            displayWishes = false;
        }

        //Checkbox zaznaczony
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            displayWishes = true;
        }
    }
}
