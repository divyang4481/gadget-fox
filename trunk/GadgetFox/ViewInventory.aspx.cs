using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using AjaxControlToolkit;
using System.IO;

namespace GadgetFox
{
    public partial class ViewInventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=ViewInventory.aspx");
            }
            else if (Session["userID"] != null && Session["userRole"].Equals("1"))
            {
                Response.Redirect("~/Forbidden.aspx");
            }
        }

        protected string getPrice(Decimal price, Decimal salePrice, bool onSale)
        {
            return String.Format("{0:c}", (onSale ? salePrice : price));
        }

        protected string getImage(string imageId)
        {
            return "Image.aspx?ImageID=" + imageId;
        }

        protected void btnExportToExcel_Click1(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=InventoryReport.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            gdvMonitorInventory.AllowPaging = false;
            gdvMonitorInventory.DataBind();

            //Change the Header Row back to white color
            gdvMonitorInventory.HeaderRow.Style.Add("background-color", "#FFFFFF");

            //Apply style to Individual Cells
            gdvMonitorInventory.HeaderRow.Cells[0].Style.Add("background-color", "green");
            gdvMonitorInventory.HeaderRow.Cells[1].Style.Add("background-color", "green");
            gdvMonitorInventory.HeaderRow.Cells[2].Style.Add("background-color", "green");
            gdvMonitorInventory.HeaderRow.Cells[3].Style.Add("background-color", "green");  

            for (int i = 0; i < gdvMonitorInventory.Rows.Count;i++ )
                {
                    GridViewRow row = gdvMonitorInventory.Rows[i];
                    //Change Color back to white
                    row.BackColor = System.Drawing.Color.White;
                    //Apply text style to each Row
                    row.Attributes.Add("class", "textmode");
   
                    //Apply style to Individual Cells of Alternating Row
                    if (i % 2 != 0)
                        {
                            row.Cells[0].Style.Add("background-color", "#C2D69B");
                            row.Cells[1].Style.Add("background-color", "#C2D69B");
                            row.Cells[2].Style.Add("background-color", "#C2D69B");
                            row.Cells[3].Style.Add("background-color", "#C2D69B");  
                        }
                }
            
            gdvMonitorInventory.RenderControl(hw);

            //style to format numbers to string
            string style = @"<style> .textmode { mso-number-format:\@; } </style>";
            Response.Write(style);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        protected void btnExportToWord_Click(object sender, EventArgs e)
        {
            gdvMonitorInventory.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "InventoryReport.doc"));
            Response.Charset = "";
            Response.ContentType = "application/ms-word";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gdvMonitorInventory.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
        }
    }
}