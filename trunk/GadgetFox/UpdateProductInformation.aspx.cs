using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class UpdateProductInformation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=UpdateProductInformation.aspx");
            }
            else if (Session["userID"] != null && Session["userRole"].Equals("1"))
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Home.aspx");
            }

            // Retreive product info
            String pid = Request.QueryString["pid"];
            if (pid != null && pid.Length > 0) {
                textBoxProductID.Text = pid;
                textBoxProductID_TextChanged(this.textBoxProductID, EventArgs.Empty);
            }
        }

        protected void textBoxProductID_TextChanged(object sender, EventArgs e)
        {
            string ProductID = textBoxProductID.Text;
            string Name = "";
            string Description = "";
            decimal Price;
            decimal SalePrice;
            int InSale;
            int Quantity;
            string CategoryName = "";
            string SubCategoryName = "";
            string Color = "";
            string Weight = "";
            //int ProductStatus;


            Products prodObj = new Products();
            prodObj.fnGetProductDetails(ProductID, out  Name, out  Description, out  Price, out  SalePrice, out  InSale,
                                        out  Quantity, out  CategoryName, out  SubCategoryName, out  Color, out  Weight);
            //out  ProductStatus);

            textBoxAddProductName.Text = Name;
            textBoxAddProductDescription.Text = Description;
            textBoxAddProductPrice.Text = Price.ToString();
            textBoxAddProductSalePrice.Text = SalePrice.ToString();
            textBoxProductQuantity.Text = Quantity.ToString();
            textBoxAddProductColor.Text = Color;
            dropDownAddCategory.Text = CategoryName;
            textBoxAddProductWeight.Text = Weight;
            

            Session["ProductID"] = textBoxProductID.Text;
            Session["ProductName"] = textBoxAddProductName.Text;
            Session["ProductDescription"] = textBoxAddProductDescription.Text;
            Session["ProductCategory"] = dropDownAddCategory.SelectedItem;
            Session["ProductSubCategory"] = SubCategoryName;
            Session["ProductPrice"] = Convert.ToDouble(textBoxAddProductPrice.Text);
            Session["ProductSalePrice"] = Convert.ToDouble(textBoxAddProductSalePrice.Text);
            Session["ProductQuantity"] = Convert.ToInt32(textBoxProductQuantity.Text);
            Session["ProductColor"] = textBoxAddProductColor.Text;
            Session["ProductWeight"] = textBoxAddProductWeight.Text;


            if (InSale == 1)
            {
                checkBoxInSale.Checked = true;
            }
            else
            {
                checkBoxInSale.Checked = false;
            }

            dropDownAddCategory.SelectedValue = Session["ProductCategory"].ToString();
            dropDownAddCategory_SelectedIndexChanged(this.dropDownAddCategory, EventArgs.Empty);
            
        }

        protected void dropDownAddCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            dropDownAddSubCategory_SelectedIndexChanged(this.dropDownAddCategory, EventArgs.Empty);

            if (dropDownAddCategory.Text != "--Select--")
            {
                dropDownAddSubCategory.Enabled = true;

            }

            //Products prodObj = new Products();

            //dropDownAddSubCategory.SelectMethod = prodObj.GetSubCategories(Session["ProductCategory"].ToString());

        }

        protected void dropDownAddSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

           
        }

        protected void checkBoxInSale_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void textBoxAddProductPrice_TextChanged(object sender, EventArgs e)
        {
            if (!checkBoxInSale.Checked)
            {
                textBoxAddProductSalePrice.Text = textBoxAddProductPrice.Text;
            }
        }

        protected void textBoxAddProductSalePrice_TextChanged(object sender, EventArgs e)
        {

        }

        protected void buttonUpdateProduct_Click(object sender, EventArgs e)
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
            Session["ProductWeight"] = textBoxAddProductWeight.Text;
            //Session["ProductImage"] = fileUploadProductImage.PostedFile;

            if (checkBoxInSale.Checked)
            {
                Session["ProductOnSale"] = 1;
            }
            else
            {
                Session["ProductOnSale"] = 0;
            }


            if (checkBoxInSale.Checked = true && textBoxAddProductSalePrice.Text == "" || textBoxAddProductSalePrice.Text == " ")
            {
                Session["ProductSalePrice"] = textBoxAddProductPrice.Text;
            }

            int status;
            string PID = "0000";

            Products prodObj = new Products();

            prodObj.fn_UpdateProductDetails(Session["ProductID"].ToString(), Session["ProductName"].ToString(), Session["ProductDescription"].ToString(), Convert.ToDecimal(Session["ProductPrice"]), Convert.ToDecimal(Session["ProductSalePrice"]), Convert.ToInt16(Session["ProductOnSale"]), Convert.ToInt32(Session["ProductQuantity"]), Session["ProductCategory"].ToString(), Session["ProductSubCategory"].ToString(), Session["ProductColor"].ToString(), Session["ProductWeight"].ToString(), fileUploadProductImage, out status, out PID);

            Session["ProductID"] = PID;

            Response.Write("<script>alert('Product " + PID + " was updated successfully!')</script>");
            Server.Transfer("SalesDashboard.aspx");

        }

        protected void buttonReset_Click(object sender, EventArgs e)
        {
            if (Session["ProductID"] != null && Session["ProductID"].ToString().Length > 0)
            {
                textBoxProductID.Text = Session["ProductID"].ToString();
                textBoxProductID_TextChanged(this.textBoxProductID, EventArgs.Empty);
            }
        }
    }
}