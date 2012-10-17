using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class ManageProductInformation : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            textBoxAddProductSalePrice.Enabled = false;
            Products prodObj = new Products();
            textBoxProductID.Text = prodObj.fn_getNextProductId();

        }

        protected void checkBoxInSale_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxInSale.Checked)
            {
                textBoxAddProductSalePrice.Enabled = true;
            }
            else
            {
                textBoxAddProductSalePrice.Enabled = false;
                textBoxAddProductSalePrice.Text = textBoxAddProductPrice.Text;

            }

        }

        protected void textBoxAddProductSalePrice_TextChanged(object sender, EventArgs e)
        {

        }

        protected void dropDownAddCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dropDownAddCategory.SelectedValue != "--Select--")
            {
                dropDownAddSubCategory.Enabled = true;
                
            }
        }

        protected void textBoxAddProductPrice_TextChanged(object sender, EventArgs e)
        {
            if (!checkBoxInSale.Checked)
            {
                textBoxAddProductSalePrice.Text = textBoxAddProductPrice.Text;
            }
        }

        protected void buttonCreateProduct_Click(object sender, EventArgs e)
        {

            Session["ProductID"] = textBoxProductID.Text;
            Session["ProductName"] = textBoxAddProductName.Text;
            Session["ProductDescription"] = textBoxAddProductDescription.Text;
            Session["ProductCategory"] = dropDownAddCategory.SelectedItem;
            Session["ProductSubCategory"] = dropDownAddSubCategory.SelectedValue;
            Session["ProductPrice"] = Convert.ToDouble(textBoxAddProductPrice.Text);
            
            Session["ProductSalePrice"] = Convert.ToDouble(textBoxAddProductSalePrice.Text);
            Session["ProductQuantity"] = Convert.ToInt32(textBoxProductQuantity.Text);
            Session["ProductColor"] = textBoxAddProductColor.Text;
            Session["ProductWeight"] = textBoxAddProductWeight.Text + dropDownWeightUnit.Text;
            //Session["ProductImage"] = fileUploadProductImage.PostedFile;

            if ( checkBoxInSale.Checked)
            {
                Session["ProductOnSale"] = 1;
            }else{
                Session["ProductOnSale"] = 0;
            }


            if (checkBoxInSale.Checked = true && textBoxAddProductSalePrice.Text == "" || textBoxAddProductSalePrice.Text == " ")
            {
                Session["ProductSalePrice"] = textBoxAddProductPrice.Text;
            }

            int status;
            string ProductID = "0000";

            Products prodObj = new Products();
            
            prodObj.fn_InsertProducts(Session["ProductName"].ToString(), Session["ProductDescription"].ToString(), Convert.ToDecimal(Session["ProductPrice"]), Convert.ToDecimal(Session["ProductSalePrice"]), Convert.ToInt16(Session["ProductOnSale"]), Convert.ToInt32(Session["ProductQuantity"]), Session["ProductCategory"].ToString(), Session["ProductSubCategory"].ToString(), Session["ProductColor"].ToString(), Session["ProductWeight"].ToString(), out status ,out ProductID );

            Session["ProductID"] = ProductID;
            
            Response.Write("<script>alert('Product added successfully" +ProductID  + "')</script>");

            //Server.Transfer("DisplayFlightDetails.aspx");




        }

        protected void buttonReset_Click(object sender, EventArgs e)
        {
            textBoxAddProductName.Text ="" ;
            textBoxAddProductDescription.Text = "";
            dropDownAddCategory.SelectedIndex = 0;
            dropDownAddSubCategory.SelectedIndex = 0;
            textBoxAddProductPrice.Text = "";
            checkBoxInSale.Checked = false;
            textBoxAddProductSalePrice.Text = "";
            textBoxAddProductColor.Text = "" ;
            textBoxAddProductWeight.Text = "";
        }

        
    }
}