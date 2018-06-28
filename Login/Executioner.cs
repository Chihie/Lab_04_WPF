using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using MySql.Data.MySqlClient;

namespace Login
{
    //TODO: Handle db connections in a better, more optimized way, e.g. create separate method for connecting
    //TODO: on_delete=CASCADE
    //TODO: Make a moderator acc type or change 'privelages' (varchar) to 'is_admin' (tinyint)
    //TODO: Imperatively merge methods to decrease their amount, after that consider breaking this class into more making-logical-sense pieces
    public class Executioner
    {
        private static readonly MySqlConnection Conn = new MySqlConnection(Connector.ConnValue("projectDB"));
        private string _query;
        private MySqlCommand _cmd;
        private static string _login;

        public Boolean LoginSelectQuery(string login, string pass)
        {
            UInt16 counter = 0;
            string passHashed = "";
            try {
                Conn.Open();
                _query = "select password from login where login='" + login + "';";
                _cmd = new MySqlCommand(_query, Conn);
                _cmd.ExecuteNonQuery();
                MySqlDataReader dataReader = _cmd.ExecuteReader();
                while (dataReader.Read()) {
                    passHashed = dataReader.GetString(0);
                    counter++;
                }
                Conn.Close();
                if (counter == 1) {
                    _login = login;
                    return new Cryptography().RetrivePassword(pass, passHashed);
                }
            } catch (Exception e) { MessageBox.Show(e.Message); }
            return false;
        }

        public Boolean RegisterInsertQuery(string userData)
        {
            Boolean operationSuccesfull = false;
             try {
                Conn.Open();
                _query = "insert into login (name, surname, login, password, email, privelages) values ('"  +
                    userData + "', 'user');";
                try { if (new MySqlCommand(_query, Conn).ExecuteNonQuery() == 1) operationSuccesfull = true; }
                catch (Exception e) { MessageBox.Show(e.Message); }
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
            return operationSuccesfull;
        }

        public Boolean LoginCheckPrivelages(string login)
        {
            string isAdmin = "user";
            try {
                Conn.Open();
                _query = "select privelages from login where login='" + login + "';";
                isAdmin = (string) new MySqlCommand(_query, Conn).ExecuteScalar();
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
            if (isAdmin == "user") return false; //login as user
            return true; //login as admin
        }

        public void FillComboboxWithEvents(ComboBox box)
        {
            try {
                Conn.Open();
                _query = "select * from event";
                MySqlDataReader dataReader = new MySqlCommand(_query, Conn).ExecuteReader();
                while (dataReader.Read()) box.Items.Add(dataReader.GetString(1));
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void FillTextBlocks(TextBlock agenda, TextBlock date, string eventName)
        {
            try {
                Conn.Open();
                _query = "select * from event where name='" + eventName.Replace("'", "''") + "';";
                MySqlDataReader dataReader = new MySqlCommand(_query, Conn).ExecuteReader();
                dataReader.Read();
                agenda.Text = dataReader.GetString(2);
                date.Text = dataReader.GetString(3);
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
        }

        private Int32 GetIdEvent(string eventName)
        {
            Int32 idEvent = -1;
            try {
                Conn.Open();
                _query = "select id from event where name='" + eventName.Replace("'", "''") + "';";
                MySqlDataReader dataReader = new MySqlCommand(_query, Conn).ExecuteReader();
                while (dataReader.Read()) idEvent = (Int32) dataReader["id"];
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
            return idEvent;
        }

        public string GetEmailUser(string login)
        {
            string email = "";
            try {
                Conn.Open();
                _query = "select email from login where login='" + login + "';";
                email = (string) new MySqlCommand(_query, Conn).ExecuteScalar();
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
            return email;
        }

        private Int32 GetIdUser()
        {
            Int32 idUser = -1;
            try {
                Conn.Open();
                _query = "select id from login where login='" + _login + "';";
                MySqlDataReader dataReader = new MySqlCommand(_query, Conn).ExecuteReader();
                while (dataReader.Read()) idUser = (Int32) dataReader["id"];
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
            return idUser;
        }

        public void AddParticipant(string eventName, string attendantType, string cateringType)
        {
            try {
                Int32 idEvent = GetIdEvent(eventName), idUser = GetIdUser();
                Conn.Open();
                _query = "select * from participant where id_event=" + idEvent + " and id_user=" + idUser + 
                         " and attendant_type='" + attendantType + "';";
                _cmd = new MySqlCommand(_query, Conn);
                _cmd.ExecuteNonQuery();
                MySqlDataReader dataReader = _cmd.ExecuteReader();
                UInt16 counter = 0;
                while (dataReader.Read()) counter++;
                Conn.Close();
                if (counter == 1) MessageBox.Show("You already signed in to this event as chosen attendant type.");
                else {
                    Conn.Open();
                    _query = "insert into participant (id_event,id_user,attendant_type,catering) values (" +
                             Convert.ToInt32(idEvent) + "," + Convert.ToInt32(idUser) + ",'" + attendantType + "','" +
                             cateringType + "');";
                    dataReader = new MySqlCommand(_query, Conn).ExecuteReader();
                    dataReader.Read();
                    Conn.Close();
                    MessageBox.Show("Succesfully signed in!\nAwaiting for admin's confirmation.");
                }
            } catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void ListAllNonAdminUsers(ComboBox comboBox)
        {
            try {
                Conn.Open();
                _query = "select * from login where privelages='user'";
                MySqlDataReader dataReader =new MySqlCommand(_query, Conn).ExecuteReader();
                comboBox.Items.Clear();
                while (dataReader.Read()) if(!comboBox.Items.Contains(dataReader.GetString(3))) 
                    comboBox.Items.Add(dataReader.GetString(3));
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void FillDataGridPending(DataGrid dataGrid, Boolean showAll)
        {
            try {
                Conn.Open();
                _query = !showAll 
                    ? "select confirmed as 'CONFIRM?', id as ID, (select name from event where event.id=participant.id_event) " +
                      "as EVENT, (select login from login where login.id=participant.id_user) as ATTENDANT, " +
                      "attendant_type as TYPE from participant where blocked='0' and confirmed='0';"
                    : "select confirmed as 'CONFIRM?', id as ID, (select name from event where event.id=participant.id_event) " +
                      "as EVENT, (select login from login where login.id=participant.id_user) as ATTENDANT, " +
                      "attendant_type as TYPE from participant;";
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(new MySqlCommand(_query, Conn));
                DataTable pending = new DataTable("Pending for confirmation");
                dataAdapter.Fill(pending);
                dataAdapter.Update(pending);
                Conn.Close();
                dataGrid.ItemsSource = pending.DefaultView;
            } catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void UpdateParticipant(string approved, string id)
        {
            try {
                Conn.Open();
                _query = approved == "True"
                    ? "update participant set confirmed='1', blocked='0' where id='" + id + "';"
                    : "update participant set confirmed='0', blocked='1' where id='" + id + "';";
                new MySqlCommand(_query, Conn).ExecuteNonQuery();
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void ResetPassword(string login, string password)
        {
            try {
                Conn.Open();
                _query = "update login set password='" + password + "' where login='" +
                         login + "';";
                new MySqlCommand(_query, Conn).ExecuteNonQuery();
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void DeleteUser(string login)
        {
            try {
                Conn.Open();
                _query = "delete from login where login='" + login + "';";
                new MySqlCommand(_query, Conn).ExecuteNonQuery();
                Conn.Close();
            } catch(Exception e) { MessageBox.Show(e.Message); }
        }
        
        public Boolean AddUserInsertQuery(string userData)
        {
            Boolean operationSuccesfull = false;
            try {
                Conn.Open();
                _query = "insert into login (name, surname, login, password, email, privelages) values ('"  +
                         userData + "');";
                _cmd = new MySqlCommand(_query, Conn);
                try { if (_cmd.ExecuteNonQuery() == 1) operationSuccesfull = true; }
                catch (Exception e) { MessageBox.Show(e.Message); }
                Conn.Close();
            } catch (Exception e) { MessageBox.Show(e.Message); }
            return operationSuccesfull;
        }
    }
}