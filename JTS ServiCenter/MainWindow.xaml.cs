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
using System.Data.SqlClient;
using System.Data;

namespace JTS_ServiCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadGrid();
        }

        //Conexion a base de datos

        SqlConnection con = new SqlConnection("Data Source=DESKTOP-U56OJS9\\CPLSQLSERVER;Initial Catalog=DemoDB;Persist Security Info=True;User ID=sa;Password=data123");

        public void clearData() //Limpia los textbox
        {
            fName_txt.Clear();
            lName_txt.Clear();
            serv_txt.Clear(); 
            servDate_txt.Clear();
            servPrice_txt.Clear();
            servDesc_txt.Clear();
            customerID.Clear();

        }

        public void LoadGrid() //Carga la base de datos para visualizarse en datagrid
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM ServiCENTER", con );
            DataTable dt = new DataTable();
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load( sdr );
            con.Close();
            datagrid.ItemsSource = dt.DefaultView;
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e) //Funcion para limpiar los textbox
        {
            clearData();
        }
    }
}
