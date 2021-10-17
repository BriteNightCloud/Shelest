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
using Shelest.Utils;

namespace Shelest.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для pMain.xaml
    /// </summary>
    public partial class pMain : Page
    {
        public pMain()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = "Введите для поиска";
            GetData();

            for (int i = 0; i < AppData.DB.orgtypes.Count(); i++)
            {
                cbFilter.Items.Add(AppData.DB.orgtypes.ToList()[i].Type);
            }

            CreatePages();
        }

        private int _page;
        private int pages;

        private int page
        {
            get
            {
                return _page;
            }
            set
            {
                if (value < 0)
                    _page = 0;
                else if (value > pages-1)
                    _page = pages-1 >= 0 ? pages-1 : 0;
                else
                    _page = value;
                CreatePages();
            }
        }

        private void CreatePages()
        {
            if (_page > pages-1)
                _page = 0;
            spPages.Children.Clear();
            TextBlock temp = new TextBlock();
            temp.Text = "<";
            temp.Padding = new Thickness(3);
            temp.MouseLeftButtonDown += PageChange_MouseLeftButtonDown;
            spPages.Children.Add(temp);

            for (int i = 1; i <= pages; i++)
            {
                if ((page - 1 <= i && page + 3 >= i) || i == 1 || i == pages)
                {
                    temp = new TextBlock();
                    temp.Text = i.ToString();
                    temp.Padding = new Thickness(3);
                    if (i == page + 1)
                        temp.TextDecorations = TextDecorations.Underline;
                    else
                    {
                        temp.MouseLeftButtonDown += PageChange_MouseLeftButtonDown;
                    }
                    spPages.Children.Add(temp);
                }
                else if (i == 2 || i == pages - 1)
                {
                    temp = new TextBlock();
                    temp.Text = "...";
                    temp.Padding = new Thickness(3);
                    spPages.Children.Add(temp);
                }
            }

            temp = new TextBlock();
            temp.Text = ">";
            temp.Padding = new Thickness(3);
            temp.MouseLeftButtonDown += PageChange_MouseLeftButtonDown;
            spPages.Children.Add(temp);
        }

        private void PageChange_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (((TextBlock)sender).Text)
            {
                case "<":
                    page -= 1;
                    break;
                case ">":
                    page += 1;
                    break;
                default:
                    page = int.Parse(((TextBlock)sender).Text) - 1;
                    break;
            }
            GetData();
        }

        private void tbSearch_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(tbSearch.Text == "Введите для поиска")
                tbSearch.Text = "";
        }

        private void tbSearch_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (tbSearch.Text.Trim() == "")
                tbSearch.Text = "Введите для поиска";
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetData();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSort.SelectedIndex == 1)
                cbSort.SelectedIndex = 0;
            GetData();
        }

        private void cbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFilter.SelectedIndex == 1)
                cbFilter.SelectedIndex = 0;
            GetData();
        }

        enum cbSortBy
        {
            A = 2,
            Z = 3,
            PrioMax = 4,
            PrioMin = 5,
            SaleMax = 6,
            SaleMin = 7
        }

        private void GetData()
        {
            if (tbSearch == null || cbSort == null || cbFilter == null || lvAgents == null)
                return;

            var data = AppData.DB.agents.ToList();
            int countBefore = data.Count;

            if (tbSearch.Text != "Введите для поиска")
            {
                data = data.Where(c => c.Name.ToLower().Contains(tbSearch.Text.ToLower())
                || c.Email.ToLower().Contains(tbSearch.Text.ToLower())
                || c.Phone.ToLower().Contains(tbSearch.Text.ToLower())).ToList();
            }
            if (cbSort.SelectedIndex > 1)
            {
                int sortBy = cbSort.SelectedIndex;

                switch (sortBy)
                {
                    case (int)cbSortBy.SaleMax:
                        data.Sort((x, y) => y.discount.CompareTo(x.discount));
                        break;
                    case (int)cbSortBy.SaleMin:
                        data.Sort((x, y) => x.discount.CompareTo(y.discount));
                        break;
                    case (int)cbSortBy.PrioMax:
                        data.Sort((x, y) => y.Priority.CompareTo(x.Priority));
                        break;
                    case (int)cbSortBy.PrioMin:
                        data.Sort((x, y) => x.Priority.CompareTo(y.Priority));
                        break;
                    case (int)cbSortBy.A:
                        data.Sort((x, y) => x.Name.CompareTo(y.Name));
                        break;
                    case (int)cbSortBy.Z:
                        data.Sort((x, y) => y.Name.CompareTo(x.Name));
                        break;
                    default:
                        break;
                }
            }
            if (cbFilter.SelectedIndex > 1)
            {
                data = data.Where(c => c.OrgType_Type == cbFilter.Items[cbFilter.SelectedIndex].ToString()).ToList();
            }

            int countBeforeX2 = data.Count;
            pages = (int)Math.Ceiling(data.Count / 10.0);

            int temp1 = page * 10 <= data.Count() ? page * 10 : 0;
            int temp2 = data.Count() - temp1 >= 10 ? 10 : data.Count() - temp1;

            data = data.GetRange(temp1, temp2);

            int countAfter = data.Count;
            lvAgents.ItemsSource = data;
            CreatePages();
            lvCount.Text = (page * 10 + 1 > countBeforeX2 ? countBeforeX2 : page * 10 + 1) + $" - {page * 10 + countAfter} из {countBeforeX2}. Всего: {countBefore}";
        }
    }
}
