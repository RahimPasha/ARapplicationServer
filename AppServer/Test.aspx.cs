using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace ARApplicationServer
{
    public partial class Test : System.Web.UI.Page
    {
        bool flag;
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                flag = false;
                Label1.Text = "Text Changed from Button";
                Label1.ForeColor = Color.Yellow;
            }
            else
            {
                flag = true;
                Label1.Text = "Text Changed from Button";
                Label1.ForeColor = Color.Green;
            }
        }
    }
}