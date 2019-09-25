using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ValideCode : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ValideCodeHelper vc = new ValideCodeHelper();
        string code = Request.Params["code"];
        vc.CreateImageOnPage(code, this.Context);       // 输出图片
    }
}