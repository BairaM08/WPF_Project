using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace Wpf__Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase conn = new DataBase();
        

        public MainWindow()
        {
            InitializeComponent();
            conn.OpenConn();
            Upd();
        }

        private void Upd()
        {

            CMB1.Items.Clear(); CMB2.Items.Clear();
            SqlCommand cmd = new SqlCommand("select Add_ID, Add_Sum, Add_Minus from AddMoney_db;", conn.GetConn());
            SqlCommand cmd2 = new SqlCommand("select Ord_ID, Ord_Sum, Ord_Plus from Orders_db;", conn.GetConn());

            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows) {
                while (rdr.Read()) {
                    CMB1.Items.Add(rdr.GetInt32(0).ToString() + ") Сумма: " + rdr.GetInt32(1).ToString() + ", Остаток: " + rdr.GetInt32(2).ToString()); } }
            else { CMB1.Text = "Не найдено данных"; }
            rdr.Close();

            rdr = cmd2.ExecuteReader();
            if (rdr.HasRows) {
                while (rdr.Read()){
                    CMB2.Items.Add(rdr.GetInt32(0).ToString() + ") Сумма: " + rdr.GetInt32(1).ToString() + ", Оплата: " + rdr.GetInt32(2).ToString()); } }
            else { CMB2.Text = "Не найдено данных"; }
            rdr.Close();
            SqlCommand GRD = new SqlCommand("select Add_Date AS 'Дата', Add_Sum AS 'Приход денег (Всего)', Add_Minus 'Остаток', Ord_Sum " + 
                "'Сумма заказа (Всего)', Ord_Plus 'Оплачено', Payment_Sum 'Сумма оплаты' from AddMoney_db a join Payments_db b on a.Add_ID = b.Add_ID " +
                "join Orders_db c on b.Ord_ID = c.Ord_ID", conn.GetConn());

            GRD.ExecuteNonQuery();
            SqlDataAdapter dataAdp = new SqlDataAdapter(GRD);
            DataTable dt = new DataTable("Students"); // В скобках указываем название таблицы
            dataAdp.Fill(dt);
            DataGRD.ItemsSource = dt.DefaultView; // Сам вывод 

            //conn.CloseConn();
        }

        private void Butt1_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand Insert1 = new SqlCommand("insert into AddMoney_db values ('" + DateTime.Today.Date.ToString("d") + "', " + TBX_Money.Text.ToString() + ", " + TBX_Money.Text.ToString() + ")", conn.GetConn());
            bool ch = true; 
            foreach (char c in TBX_Order.Text.ToString()) { if (c < '0' || c > '9') { ch = false; } }

            if (ch) 
            {
                int qq = Insert1.ExecuteNonQuery();
                Upd();
                MessageBox.Show("Приход денег = " + TBX_Money.Text.ToString() + " добавлен");
                TBX_Money.Text = "";
            }
            else { MessageBox.Show("Пожалуйста, введите целое число"); }
        }

        private void Butt2_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand Insert2 = new SqlCommand("insert into Orders_db values ('" + DateTime.Today.Date.ToString("d") + "', " + TBX_Order.Text.ToString() + ", 0)", conn.GetConn());
            bool ch = true;
            foreach (char c in TBX_Order.Text.ToString()) {  if (c < '0' || c > '9') { ch = false; } }
            
            if (ch)
            {
                int qq = Insert2.ExecuteNonQuery();
                Upd();
                MessageBox.Show("Заказ с суммой = " + TBX_Order.Text.ToString() + " добавлен");
                TBX_Order.Text = "";
            }
            else { MessageBox.Show("Пожалуйста, введите целое число"); }
        }

        private void Butt3_Click(object sender, RoutedEventArgs e)
        {
            var CMB1_ID = CMB1.SelectedItem.ToString().Split(new[] { ' ' }, 6);
            var CMB2_ID = CMB2.SelectedItem.ToString().Split(new[] { ' ' }, 6);

            SqlCommand Updd = new SqlCommand("insert into Payments_db values (" + (CMB2_ID[0].Remove(CMB2_ID[0].Length-1)) + ", " + (CMB1_ID[0].Remove(CMB1_ID[0].Length-1)) + ", " + TBX3.Text.ToString() + ");", conn.GetConn());

            int qq1 = int.Parse(CMB1_ID[2].Remove(CMB1_ID[2].Length - 1));
            int qq2 = int.Parse(CMB2_ID[2].Remove(CMB2_ID[2].Length - 1));

            if (int.Parse(CMB1_ID[4].ToString()) < int.Parse(CMB2_ID[4])) { MessageBox.Show("Оплата не должна превышать остатка"); }
            else if (qq2 < int.Parse(CMB2_ID[4])) { MessageBox.Show("Сумма оплаты превышает оплату"); }
            else if (qq1 < int.Parse(CMB1_ID[4])) { MessageBox.Show("Сумма остатка превышает оплату"); ; }
            else
            {
                int qq = Updd.ExecuteNonQuery();
                MessageBox.Show("Оплата на сумму " + TBX3.Text.ToString() + " выполнена");
                TBX3.Text = "";
                CMB1.SelectedItem = 0;
                Upd();
            }
        }
    }
}
