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
    public partial class MainWindow : Window
    {
        DataBase conn = new DataBase();
        DataTable table = new DataTable();

        public MainWindow()
        {
            InitializeComponent();
            conn.OpenConn();
            table.Columns.Add("Код 'Прихода'".ToString());  //DataGridView
            table.Columns.Add("Сумма".ToString());
            table.Columns.Add("Остаток (До)".ToString());
            table.Columns.Add("Остаток (После)".ToString());
            table.Columns.Add("Код заказа".ToString());
            table.Columns.Add("Сумма заказа".ToString());
            table.Columns.Add("Сумма оплаты (До)".ToString());
            table.Columns.Add("Сумма оплаты (После)".ToString());
            Upd();
        }

        private void Upd()
        {
            CMB1.Items.Clear(); CMB2.Items.Clear();             //обновление данных ComboBox1 (доступных для использования 'Приходов денег')
            SqlCommand cmd = new SqlCommand("select Add_ID, Add_Sum, Add_Minus from AddMoney_db where Add_Minus>0;", conn.GetConn());
            SqlCommand cmd2 = new SqlCommand("select Ord_ID, Ord_Sum, Ord_Plus from Orders_db where Ord_Sum>Ord_Plus;", conn.GetConn());
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows) {  while (rdr.Read()) {
                    CMB1.Items.Add(rdr.GetInt32(0).ToString() + ") Сумма: " + rdr.GetInt32(1).ToString() + ", Остаток: " + rdr.GetInt32(2).ToString()); } }
            rdr.Close();

            rdr = cmd2.ExecuteReader();                        //обновление данных ComboBox2 (доступных для оплаты заказов)
            if (rdr.HasRows) { while (rdr.Read()){
                    CMB2.Items.Add(rdr.GetInt32(0).ToString() + ") Сумма: " + rdr.GetInt32(1).ToString() + ", Оплачено: " + rdr.GetInt32(2).ToString()); } }
            rdr.Close();
        }

        private void Butt1_Click(object sender, RoutedEventArgs e) //добавить Приход денег
        {
            SqlCommand Insert1 = new SqlCommand("insert into AddMoney_db values ('" + DateTime.Today.Date.ToString("d") + "', " + TBX_Money.Text.ToString() + ", " + TBX_Money.Text.ToString() + ")", conn.GetConn());

            try { int qq = Insert1.ExecuteNonQuery(); MessageBox.Show("Запись добавлена"); }
            catch (SqlException ex) { MessageBox.Show("Получена ошибка: " + ex.Message); }

            Upd();
            TBX_Money.Clear();
        }

        private void Butt2_Click(object sender, RoutedEventArgs e) //добавить заказ
        {
            SqlCommand Insert2 = new SqlCommand("insert into Orders_db values ('" + DateTime.Today.Date.ToString("d") + "', " + TBX_Order.Text.ToString() + ", 0)", conn.GetConn());
            
            try { int qq = Insert2.ExecuteNonQuery(); MessageBox.Show("Запись добавлена"); }
            catch (SqlException ex) { MessageBox.Show("Получена ошибка: " + ex.Message); }

            Upd();
            TBX_Order.Clear();
        }

        private void Butt3_Click(object sender, RoutedEventArgs e)
        {
            if (CMB1.SelectedIndex > -1 && CMB2.SelectedIndex > -1)
            {
                var CMB1_ID = CMB1.SelectedItem.ToString().Split(new[] { ')' }, 2); // получение выбранных в ComboBox номеров
                var CMB2_ID = CMB2.SelectedItem.ToString().Split(new[] { ')' }, 2);

                SqlCommand Updd = new SqlCommand("insert into Payments_db values (" + CMB2_ID[0] + ", " + CMB1_ID[0] + ", " + TBX3.Text.ToString() + ");", conn.GetConn());

                try 
                {
                    int qq = Updd.ExecuteNonQuery();

                    var num1 = CMB1_ID[1].Split(new[] { ' ' }, 6);
                    var num2 = CMB2_ID[1].Split(new[] { ' ' }, 6);

                    DataRow dr = table.NewRow();                                       //добавление данных DataGridView
                    dr["Код 'Прихода'"] = CMB1_ID[0].ToString();
                    dr["Сумма"] = num1[2].Remove(num1[2].Length - 1);
                    dr["Остаток (До)"] = num1[4];
                    dr["Остаток (После)"] = int.Parse(num1[4]) - int.Parse(TBX3.Text);
                    dr["Код заказа"] = CMB2_ID[0].ToString();
                    dr["Сумма заказа"] = num2[2].Remove(num2[2].Length - 1);
                    dr["Сумма оплаты (До)"] = num2[4];
                    dr["Сумма оплаты (После)"] = int.Parse(num2[4]) + int.Parse(TBX3.Text);
                    table.Rows.Add(dr);
                    DataGRD.ItemsSource = table.DefaultView;

                    TBX3.Clear();
                    Upd();
                }
                catch (SqlException ex) { MessageBox.Show("Получена ошибка: " + ex.Message); }

            }
            else { MessageBox.Show("Выберите денные"); }
        }
    }
}
