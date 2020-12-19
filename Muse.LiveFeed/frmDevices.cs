using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Muse.LiveFeed
{
    public partial class frmDevices : Form
    {
        public frmDevices()
        {
            InitializeComponent();
        }

        public void AddDeviceToList(string name)
        {
            lisDevices.Items.Add(name);
        }
    }
}
