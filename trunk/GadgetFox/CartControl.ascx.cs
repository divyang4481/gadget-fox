using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class CartControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
    {
        if (Profile.SCart == null)
        {
            Profile.SCart = new GadgetFox.Cart();
        }
        if (!Page.IsPostBack)
        {
            ReBindGrid();
        }
        if(Profile.SCart.Items == null)
        {
            TotalLabel.Visible = false;
        }
    }
    protected void grdCart_RowEditing
                 (object sender, GridViewEditEventArgs e)
    {
        grdCart.EditIndex = e.NewEditIndex;
        ReBindGrid();
    }
    protected void grdCart_RowUpdating
               (object sender, GridViewUpdateEventArgs e)
    {
        TextBox txtQuantity = (TextBox)
        grdCart.Rows[e.RowIndex].Cells[2].Controls[0];
        int Quantity = Convert.ToInt32(txtQuantity.Text);
        if (Quantity == 0)
        {
            Profile.SCart.Items.RemoveAt(e.RowIndex);
        }
        else
        {
            Profile.SCart.Items[e.RowIndex].Quantity 
                                         = Quantity;
        }
        grdCart.EditIndex = -1;
        ReBindGrid();
    }
    protected void grdCart_RowCancelingEdit
             (object sender, GridViewCancelEditEventArgs e)
    {
        grdCart.EditIndex = -1;
        ReBindGrid();
    }
    protected void grdCart_RowDeleting
                 (object sender, GridViewDeleteEventArgs e)
    {
        Profile.SCart.Items.RemoveAt(e.RowIndex);
        ReBindGrid();
    }
    private void ReBindGrid()
    {
        grdCart.DataSource = Profile.SCart.Items;
        DataBind();
        TotalLabel.Text = string.Format("Total:{0,19:C}", 
                                     Profile.SCart.Total);
    }
}