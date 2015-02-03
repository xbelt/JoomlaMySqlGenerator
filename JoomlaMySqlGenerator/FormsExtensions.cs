using System.Windows.Forms;

namespace Warenlager.API
{
    static class FormsExtensions
    {
        public static string Text(this ComboBox combobox)
        {
            if (combobox == null)
            {
                return "";
            }
            if (combobox.Text == null)
            {
                return "";
            }
            return combobox.Text;
        }
    }
}
