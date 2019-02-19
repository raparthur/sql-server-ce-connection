using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GestorSqlServerCE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_executar_Click(object sender, EventArgs e)
        {
            Connection conn = new Connection("C:/csharp/novo.sdf");
            conn.createDb();
            //conn.dropTable("contatos");
            //conn.createTable("contatos",
            //    "id int primary key",
            //    "nome nvarchar(20)",
            //    "nasc datetime");
            //MessageBox.Show(conn.insert("insert into contatos values (?,?,?)",2,"joao", new DateTime(2018, 11, 22)).ToString());
            //conn.select("select * from contatos where nasc = ?", new DateTime(2019, 02, 19));
            //MessageBox.Show(conn.lastInsertedId("contatos", "id").ToString());
            //MessageBox.Show(conn.simpleQuery("update contatos set nome = ?, nasc = ? where id = ?", "joaninha", new DateTime(2005, 10, 05), 1).ToString());
        }
    }
}
