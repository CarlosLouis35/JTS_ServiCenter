using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

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

        SqlConnection dbcon = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarlosDB;Persist Security Info=True;");

        public void clearData() //Limpia los textbox
        {
            fName_txt.Clear();
            lName_txt.Clear();
            serv_txt.Clear();

            servPrice_txt.Clear();
            servDesc_txt.Clear();
        }

        public void LoadGrid() //Carga la base de datos para visualizarse en datagrid
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM servi_CENTER", dbcon); //Comando para leer todo de la tabla
                DataTable dt = new DataTable();
                dbcon.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                dt.Load(sdr);
                dbcon.Close();
                datagrid.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (dbcon.State == ConnectionState.Open)
                {
                    dbcon.Close();
                }
            }
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e) //Funcion para limpiar los textbox
        {
            clearData();
        }

        public bool isValid() //Validacion de espacios ocupados.
                              //Si esta alguno vacio no procede, de lo contrario, validara la informacion y procedera 
        {

            if (fName_txt.Text == string.Empty)
            {
                MessageBox.Show("First name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (lName_txt.Text == string.Empty)
            {
                MessageBox.Show("Last name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (serv_txt.Text == string.Empty)
            {
                MessageBox.Show("Service is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
           
            if (servPrice_txt.Text == string.Empty)
            {
                MessageBox.Show("Service price is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (servDesc_txt.Text == string.Empty)
            {
                MessageBox.Show("Service description is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            return true;

        }

        private void createBtn_Click(object sender, RoutedEventArgs e) //Creacion de nueva informacion
        {
            try
            {
                if (isValid())
                {
                    //Conexion a base de datos y comando a insertar nueva informacion en la tabla con las columnas asignadas
                    SqlCommand cmd = new SqlCommand("INSERT INTO servi_CENTER VALUES(@FIRSTNAME, @LASTNAME, @SERV, @SERV_DATE,@SERV_PRICE, @SERV_DESC)", dbcon); 
              
                    //Especifica la interpretacion de comando de String. 
                    cmd.CommandType = CommandType.Text;

                    //Atrapa informacion insertada en textbox y la agrega en la base de dato. (Setter & Getter)
                    cmd.Parameters.AddWithValue("@FIRSTNAME", fName_txt.Text);
                    cmd.Parameters.AddWithValue("@LASTNAME", lName_txt.Text);
                    cmd.Parameters.AddWithValue("@SERV", serv_txt.Text);
                    cmd.Parameters.AddWithValue("@SERV_DATE", DateTime.Now);
                    cmd.Parameters.AddWithValue("@SERV_PRICE", servPrice_txt.Text);
                    cmd.Parameters.AddWithValue("SERV_DESC", servDesc_txt.Text);
                    dbcon.Open();
                    cmd.ExecuteScalar(); 
                    dbcon.Close();
                    LoadGrid();
                    MessageBox.Show("Registrado exitosamente", "Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearData();

                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally { dbcon.Close(); }
        }



        private void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            dbcon.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM servi_CENTER WHERE Id = " + customerFinder_txt.Text + " ", dbcon);

            try 
            {
                MessageBox.Show("Registo removido exitosamente", "Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                cmd.ExecuteNonQuery();
                dbcon.Close();
                clearData();
                LoadGrid();
                dbcon.Close();
            }
            
            catch(SqlException ex)
            {
                MessageBox.Show("No fue removido" + ex.Message);
            }

            finally 
            { 
                dbcon.Close(); 
            
            }

        }


        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            dbcon.Open();
            SqlCommand cmd = new SqlCommand("UPDATE servi_CENTER SET FIRSTNAME = '" + fName_txt.Text+"', LASTNAME = '"+lName_txt.Text+ "',SERV = '"+serv_txt.Text+"', SERV_PRICE = '"+servPrice_txt.Text+ "',SERV_DESC = '"+servDesc_txt.Text+"' WHERE Id = '"+customerFinder_txt.Text+"' ",dbcon);
            try
            {
                MessageBox.Show("Registo actualizado exitosamente", "Actualizado", MessageBoxButton.OK, MessageBoxImage.Information);
                cmd.ExecuteNonQuery();
     
            }

            catch (SqlException ex)
            {
                MessageBox.Show("No fue actualizado" + ex.Message);
            }

            finally
            {
                dbcon.Close();
                clearData();
                LoadGrid();
            }
        }
    }
}
