using System.Windows.Forms;

namespace Warenlager.Windows
{
    public partial class ProgressForm : Form
    {
        public ProgressForm(int numberOfItems, string headerText, string labelText)
        {
            InitializeComponent();
            progressBar1.Visible = true;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = numberOfItems;
            progressBar1.Value = 1;
            progressBar1.Step = 1;

            label1.Text = labelText;
            ControlBox = false;
            Text = headerText;
        }

        public void Reset(int numberOfItems, string labelText)
        {
            progressBar1.Visible = true;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = numberOfItems;
            progressBar1.Value = 1;
            progressBar1.Step = 1;

            label1.Text = labelText;
            ControlBox = false;
        }

        public void PerformStep()
        {
            var value = progressBar1.Value;
            value++;
            if (value > progressBar1.Maximum) return;
            progressBar1.Value = value;
            progressBar1.Value = value - 1;
            progressBar1.Value = value;
        }

        public void PerformStepAndUpdateText(string text)
        {
            var value = progressBar1.Value;
            value++;
            label1.Text = text;
            if (value > progressBar1.Maximum) return;
            progressBar1.Value = value;
            progressBar1.Value = value - 1;
            progressBar1.Value = value;
        }

        public void UpdateText(string text)
        {
            label1.Text = text;
        }
    }
}
