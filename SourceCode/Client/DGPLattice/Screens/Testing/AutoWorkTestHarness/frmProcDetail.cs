using System.Windows.Forms;

namespace DGPLattice.Screens.Testing
{
    public partial class frmProcDetail : Form
    {
        public frmProcDetail(string procSteps)
        {
            InitializeComponent();

            rtbxProcDetail.Text = procSteps;
        }
    }
}
