using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleNFCReaderWriter
{
    public partial class StartScreen : Form
    {
        public StartScreen()
        {
            InitializeComponent();
        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            Write write = new Write();
            write.Show();
        }

        private void btn_ReadDelete_Click(object sender, EventArgs e)
        {
            ReadDelete rd = new ReadDelete();
            rd.Show();
        }
    }
}
