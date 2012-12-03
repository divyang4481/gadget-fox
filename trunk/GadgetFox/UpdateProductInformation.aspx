<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/GadgetSite2.Master" CodeBehind="UpdateProductInformation.aspx.cs" Inherits="GadgetFox.UpdateProductInformation" %>


<asp:Content ID="Content1" runat="server" contentplaceholderid="ContentPlaceHolder1" >
    
 
    <table>

        <tr >
            <td >
                <h3><asp:Label ID="labelManageProduct" runat="server" >Edit Product</asp:Label></h3>
            </td>
        </tr>

        <tr>
            <td><asp:Label ID="labelProductID" runat="server">Product ID:</asp:Label></td>
            <td style="width: 224px">
                <asp:TextBox ID="textBoxProductID" runat="server" AutoPostBack="True" OnTextChanged="textBoxProductID_TextChanged"></asp:TextBox>

            </td>
        </tr>

        <tr>
            <td><asp:Label ID="labelAddName" runat="server">Name:</asp:Label></td>
            <td style="width: 224px">
                <asp:TextBox ID="textBoxAddProductName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Text="*" ControlToValidate="textBoxAddProductName" SetFocusOnError="true" Display="Static" ErrorMessage="Product name required." ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td><asp:Label ID="labelAddCategory" runat="server">Category:</asp:Label></td>
            <td style="width: 224px">
                <asp:DropDownList ID="dropDownAddCategory" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropDownAddCategory_SelectedIndexChanged" AppendDataBoundItems="True" DataSourceID="ObjectDataSource1" DataTextField="Name" >
                    <asp:ListItem Selected="True">--Select--</asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetCategories" TypeName="GadgetFox.Products"></asp:ObjectDataSource>
            </td>
        </tr>
        <tr >
            <td style="height: 26px"><asp:Label ID="labelAddSubCategory" runat="server">Sub Category:</asp:Label></td>
            <td style="height: 26px; width: 224px;">
                <asp:DropDownList ID="dropDownAddSubCategory" runat="server" DataSourceID="ObjectDataSource2" DataTextField="Name" Enabled="False" OnSelectedIndexChanged="dropDownAddSubCategory_SelectedIndexChanged" >
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
            <td style="height: 29px; width: 224px;">
                <asp:TextBox ID="textBoxAddProductDescription" runat="server" Height="93px" TextMode="MultiLine" Width="219px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Text="*" ControlToValidate="textBoxAddProductDescription" SetFocusOnError="true" Display="Static" ErrorMessage="Description required." ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr >
            <td class="auto-style1"><asp:Label ID="labelAddPrice" runat="server">Price:</asp:Label></td>
            <td class="auto-style2">
                <asp:TextBox ID="textBoxAddProductPrice" runat="server" AutoPostBack="True" OnTextChanged="textBoxAddProductPrice_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Text="*" ControlToValidate="textBoxAddProductPrice" SetFocusOnError="true" Display="Static" ErrorMessage="Price required." ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Text="*" ErrorMessage = "Must be a valid price without $." ControlToValidate="textBoxAddProductPrice" SetFocusOnError="True" Display="Static" ValidationExpression="^[0-9]*(\.)?[0-9]?[0-9]?$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr >
            <td><asp:Label ID="labelInSale" runat="server"> In Sale ?:</asp:Label></td>
            <td style="width: 224px">
                <asp:CheckBox ID="checkBoxInSale" runat="server" OnCheckedChanged="checkBoxInSale_CheckedChanged" AutoPostBack="True" />
            </td>
         </tr>
        <tr>    
            <td><asp:Label ID="labelProductSalePrice" runat="server">Sale Price:</asp:Label></td>
            <td style="width: 224px">
                <asp:TextBox ID="textBoxAddProductSalePrice" runat="server" OnTextChanged="textBoxAddProductSalePrice_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Text="*" ControlToValidate="textBoxAddProductSalePrice" SetFocusOnError="true" Display="Static" ErrorMessage="Sale Price required." ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" Text="*" ControlToValidate="textBoxAddProductSalePrice" SetFocusOnError="True" Display="Static" ValidationExpression="^[0-9]*(\.)?[0-9]?[0-9]?$" ErrorMessage="Must be a valid price without $." ForeColor="Red"></asp:RegularExpressionValidator>
            </td>
        </tr>
                <tr>    
            <td><asp:Label ID="labelProductQuantity" runat="server">Quantity:</asp:Label></td>
            <td style="width: 224px">
                <asp:TextBox ID="textBoxProductQuantity" runat="server" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" Text="*" ControlToValidate="textBoxProductQuantity" SetFocusOnError="true" Display="Static" ErrorMessage="Quantity required." ForeColor="Red"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" Text="*" ControlToValidate="textBoxProductQuantity" SetFocusOnError="True" Display="Static" ValidationExpression="^[0-9]*" ErrorMessage="Must be a valid number." ForeColor="Red"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr >
            <td><asp:Label ID="labelProductColor" runat="server">Color:</asp:Label></td>
            <td style="width: 224px">
                <asp:TextBox ID="textBoxAddProductColor" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Text="*" ControlToValidate="textBoxAddProductColor" SetFocusOnError="true" Display="Static" ErrorMessage="Product color required." ForeColor="Red"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr >
            <td style="height: 29px"><asp:Label ID="AddProductWeight" runat="server">Weight:</asp:Label></td>
            <td style="height: 29px; width: 224px;">
                <asp:TextBox ID="textBoxAddProductWeight" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr >
            <td><asp:Label ID="labelAddImageFile" runat="server">Image File:</asp:Label></td>
            <td style="width: 224px">
                <asp:FileUpload ID="fileUploadProductImage" runat="server" />
                
            </td>
        </tr>
    </table>
    <p>
        <table>
            <tr>
                <td>
                    <asp:Button ID="buttonUpdateProduct" runat="server" OnClick="buttonUpdateProduct_Click" Text="Update Product" Width="144px" style="height: 26px" />
                </td>

                <td>
                    <asp:Button ID="buttonReset" runat="server" CausesValidation="False" OnClick="buttonReset_Click" Text="Reset" Width="144px" />
                </td>
            </tr>  

       </table>
    </p> 
    <p> 
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Please correct the following errors to create account" />
                </asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="headContent">
    <style type="text/css">
        .auto-style1 {
            height: 29px;
        }
        .auto-style2 {
            width: 224px;
            height: 29px;
        }
    </style>
</asp:Content>
