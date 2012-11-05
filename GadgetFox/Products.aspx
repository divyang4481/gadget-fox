<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="GadgetFox.Products1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" />
<div>
<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
SelectCommand="SELECT [ProductID], [Name], [Description], [Price], [ImageUrl] FROM [Products]">
</asp:SqlDataSource>
</div>

<asp:DataList ID="DataList1" runat="server" 
              DataSourceID="SqlDataSource1" 
              RepeatColumns="4"
              RepeatDirection="Horizontal">
<ItemTemplate>
<asp:ImageButton ID="ImageButton1" runat="server" 
ImageUrl='<%# Eval("ImageUrl", "Images\\thumb_{0}") %>' 
PostBackUrl='<%# Eval("ProductID", 
"ProductDetails.aspx?ProductID={0}") %>' />
<br />
<asp:Label ID="NameLabel" runat="server" 
           Text='<%# Eval("Name") %>'>
</asp:Label>
<asp:Label ID="PriceLabel" runat="server" 
           Text='<%# Eval("Price", "{0:C}") %>'>
</asp:Label><br />
<br />
<br />
</ItemTemplate>
</asp:DataList><br />
<asp:HyperLink ID="CartLink" runat="server" 
               NavigateUrl="~/UserCart.aspx">
               View Shopping Cart
</asp:HyperLink><br />
</form>
</asp:Content>
