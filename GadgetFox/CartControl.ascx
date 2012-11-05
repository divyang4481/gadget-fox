<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartControl.ascx.cs" Inherits="GadgetFox.CartControl" %>
<asp:GridView ID="grdCart" runat="server" 
              AutoGenerateColumns="False" 
              DataKeyNames="ProductID" 
              OnRowCancelingEdit="grdCart_RowCancelingEdit" 
              OnRowDeleting="grdCart_RowDeleting" 
              OnRowEditing="grdCart_RowEditing" 
              OnRowUpdating="grdCart_RowUpdating">
<Columns>
<asp:TemplateField>
    <ItemTemplate>
    <asp:Image ID="Image1" runat="server" 
    ImageUrl='<%#Eval("ImageUrl","~/Images/thumb_{0}")%>'/>
    </ItemTemplate>
</asp:TemplateField>
<asp:BoundField DataField="ProductName" 
                HeaderText="Product" ReadOnly="True"/>
<asp:BoundField DataField="Quantity" HeaderText="Quantity"/>
<asp:BoundField DataField="Price" DataFormatString="{0:c}" 
                HeaderText="Price" ReadOnly="True" />
<asp:BoundField DataField="SubTotal" DataFormatString="{0:c}" 
                HeaderText="Total" ReadOnly="True" />
<asp:CommandField ShowDeleteButton="True" 
                  ShowEditButton="True"/>
</Columns>
<EmptyDataTemplate>
Your Shopping Cart is empty, add items
<a href="Products.aspx">Add Products</a>
</EmptyDataTemplate>
</asp:GridView>
<asp:Label ID="TotalLabel" runat="server"></asp:Label>