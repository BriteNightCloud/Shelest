using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shelest.Utils;
using Shelest.View.Windows;

namespace Shelest.Models
{
    partial class agent
    {
        public int saleCount
        {
            get
            {
                int sum = 0;
                var sales = AppData.DB.productsales.ToList();

                sales = sales.Where(c => c.Agent.Equals(Name) 
                && (c.Date.Year == DateTime.Today.Year ? true :
                c.Date.Year < DateTime.Today.Year - 1 ? false :
                c.Date.Month < DateTime.Today.Month ? false :
                c.Date.Day >= DateTime.Today.Day)).ToList();

                foreach (var i in sales)
                {
                    sum += i.Amount;
                }

                return sum;
            }
        }

        private string _disc = null;

        public int discount
        {
            get
            {
                if (string.IsNullOrEmpty(_disc))
                    _disc = d().ToString();

                return int.Parse(_disc);
            }
        }

        private int d()
        {
            int sum = 0;
            var sales = AppData.DB.productsales.ToList();

            sales = sales.Where(c => c.Agent.Equals(Name)).ToList();

            foreach (var i in sales)
            {
                sum += i.Amount;
            }

            if (sum < 10000)
                return 0;
            else if (sum < 50000)
                return 5;
            else if (sum < 150000)
                return 10;
            else if (sum < 500000)
                return 20;
            else
                return 25;
        }

        public string logoPath
        {
            get
            {
                return string.IsNullOrEmpty(Logo) ? "/picture.png" : Logo ;
            }
        }

        public string backColor
        {
            get
            {
                return discount >= 25 ? "LightGreen" : "Transparent";
            }
        }
    }
}
