<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/GadgetSite2.Master" CodeBehind="ManageProductInformation.aspx.cs" Inherits="GadgetFox.ManageProductInformation" %>


<asp:Content ID="Content1" runat="server" contentplaceholderid="ContentPlaceHolder1" >
    
 
    <table>

        <tr >
            <td >
                <h3><asp:Label ID="labelManageProduct" runat="server" >Manage Product</asp:Label></h3>
            </td>
        </tr>

        <tr>
            <td><asp:Label ID="labelProductID" runat="server">Product ID:</asp:Label></td>
            <td>
                <asp:TextBox ID="textBoxProductID" runat="server" Enabled="False" ReadOnly="True"></asp:TextBox>
                
            </td>
        </tr>

        <tr>
            <td><asp:Label ID="labelAddName" runat="server">Name:</asp:Label></td>
            <td>
                <asp:TextBox ID="textBoxAddProductName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Text="*" ControlToValidate="textBoxAddProductName" SetFocusOnError="true" Display="Static" ErrorMessage="Product name required." ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td><asp:Label ID="labelAddCategory" runat="server">Category:</asp:Label></td>
            <td>
                <asp:DropDownList ID="dropDownAddCategory" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropDownAddCategory_SelectedIndexChanged" AppendDataBoundItems="True" DataSourceID="ObjectDataSource1" DataTextField="Name" >
                    <asp:ListItem Selected="True">--Select--</asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetCategories" TypeName="GadgetFox.Products"></asp:ObjectDataSource>
            </td>
        </tr>
        <tr >
            <td style="height: 26px"><asp:Label ID="labelAddSubCategory" runat="server">Sub Category:</asp:Label></td>
            <td style="height: 26px">
                <asp:DropDownList ID="dropDownAddSubCategory" runat="server" AppendDataBoundItems="True" DataSourceID="ObjectDataSource2" DataTextField="Name" Enabled="False" >
                    <asp:ListItem>--Select--</asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSubCategories" TypeName="GadgetFox.Products">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="dropDownAddCategory" Name="categoryName" PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>
        <tr >
            <td style="height: 29px"><asp:Label ID="labelAddDescription" runat="server">Description:</asp:Label></td>
            <td style="height: 29px">
                <asp:TextBox ID="textBoxAddProductDescription" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Text="*" ControlToValidate="textBoxAddProductDescription" SetFocusOnError="true" Display="Static" ErrorMessage="Description required." ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr >
            <td><asp:Label ID="labelAddPrice" runat="server">Price:</asp:Label></td>
            <td>
                <asp:TextBox ID="textBoxAddProductPrice" runat="server" AutoPostBack="True" OnTextChanged="textBoxAddProductPrice_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Text="*" ControlToValidate="textBoxAddProductPrice" SetFocusOnError="true" Display="Static" ErrorMessage="Price required." ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Text="*" ErrorMessage = "Must be a valid price without $." ControlToValidate="textBoxAddProductPrice" SetFocusOnError="True" Display="Static" ValidationExpression="^[0-9]*(\.)?[0-9]?[0-9]?$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr >
            <td><asp:Label ID="labelInSale" runat="server"> In Sale ?:</asp:Label></td>
            <td>
                <asp:CheckBox ID="checkBoxInSale" runat="server" OnCheckedChanged="checkBoxInSale_CheckedChanged" AutoPostBack="True" />
            </td>
         </tr>
        <tr>    
            <td><asp:Label ID="labelProductSalePrice" runat="server">Sale Price:</asp:Label></td>
            <td>
                <asp:TextBox ID="textBoxAddProductSalePrice" runat="server" OnTextChanged="textBoxAddProductSalePrice_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Text="*" ControlToValidate="textBoxAddProductSalePrice" SetFocusOnError="true" Display="Static" ErrorMessage="Sale Price required." ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" Text="*" ControlToValidate="textBoxAddProductSalePrice" SetFocusOnError="True" Display="Static" ValidationExpression="^[0-9]*(\.)?[0-9]?[0-9]?$" ErrorMessage="Must be a valid price without $." ForeColor="Red"></asp:RegularExpressionValidator>
            </td>
        </tr>
                <tr>    
            <td><asp:Label ID="labelProductQuantity" runat="server">Quantity:</asp:Label></td>
            <td>
                <asp:TextBox ID="textBoxProductQuantity" runat="server" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" Text="*" ControlToValidate="textBoxProductQuantity" SetFocusOnError="true" Display="Static" ErrorMessage="Sale Price required." ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" Text="*" ControlToValidate="textBoxAddProductSalePrice" SetFocusOnError="True" Display="Static" ValidationExpression="^[0-9]*" ErrorMessage="Must be a valid number." ForeColor="Red"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr >
            <td><asp:Label ID="labelProductColor" runat="server">Color:</asp:Label></td>
            <td>
                <asp:TextBox ID="textBoxAddProductColor" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Text="*" ControlToValidate="textBoxAddProductColor" SetFocusOnError="true" Display="Static" ErrorMessage="Product color required." ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr >
            <td style="height: 29px"><asp:Label ID="AddProductWeight" runat="server">Weight:</asp:Label></td>
            <td style="height: 29px">
                <asp:TextBox ID="textBoxAddProductWeight" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" Text="*" ControlToValidate="textBoxAddProductWeight" SetFocusOnError="True" Display="Static" ValidationExpression="^[0-9]*" ErrorMessage="Must be a valid weight."></asp:RegularExpressionValidator>
                <asp:DropDownList ID="dropDownWeightUnit" runat="server">
                    <asp:ListItem Selected="True">pounds</asp:ListItem>
                    <asp:ListItem>ounces</asp:ListItem>
                </asp:DropDownList>
                
            </td>
        </tr>
        <tr >
            <td><asp:Label ID="labelAddImageFile" runat="server">Image File:</asp:Label></td>
            <td>
                <asp:FileUpload ID="fileUploadProductImage" runat="server" />
                
            </td>
        </tr>
    </table>
    <p>
        <table>
            <tr>
                <td>
                    <asp:Button ID="buttonCreateProduct" runat="server" OnClick="buttonCreateProduct_Click" Text="Create Product" Width="144px" style="height: 26px" />
                </td>

                <td>
                    <asp:Button ID="buttonReset" runat="server" CausesValidation="False" OnClick="buttonReset_Click" Text="Reset" Width="144px" />
                </td>
            </tr>  

       </table>
    </p> 
    <p> 
        <table>
            <tr>
                
                <td> 
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Please correct the following errors to create account" />
                </td>
                
            </tr>
           </table>
      </p>
</asp:Content>



