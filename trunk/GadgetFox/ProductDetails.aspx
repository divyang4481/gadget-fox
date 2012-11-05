<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="GadgetFox.ProductDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
ConnectionString="<%$ ConnectionStrings:ConnectionString %>"
SelectCommand="SELECT [ProductID], [Name], [Description], [Price], [ImageUrl] FROM [Products] WHERE ([ProductID] = @ProductID)">
    <SelectParameters>
    <asp:QueryStringParameter Name="ProductID" 
                              QueryStringField="ProductID" 
                              Type="Decimal" />
    </SelectParameters>
</asp:SqlDataSource>
</div>

<asp:DataList ID="DataList1" runat="server" 
              DataSourceID="SqlDataSource1">
<ItemTemplate>
  <asp:Image ID="Image1" runat="server" 
       ImageUrl='<%# Eval("ImageUrl","~/Images\\{0}") %>'/>
  <asp:Label ID="ImageUrlLabel" runat="server" 
             Text='<%# Eval("ImageUrl") %>' 
             Visible="False">
  </asp:Label><br />
  <asp:Label ID="NameLabel" runat="server" 
             Text='<%# Eval("Name") %>'>
  </asp:Label><br />
  <asp:Label ID="DescriptionLabel" runat="server" 
             Text='<%# Eval("Description") %>'>
  </asp:Label><br />
  <asp:Label ID="PriceLabel" runat="server" 
             Text='<%# Eval("Price", "{0:##0.00}" ) %>'>
  </asp:Label><br />
</ItemTemplate>
</asp:DataList><br />
<asp:Button ID="btnAdd" runat="server" OnClick="Button1_Click" 
                        Text="Add to Cart" /><br /><br />
<asp:HyperLink ID="HyperLink1" runat="server" 
               NavigateUrl="~/Products.aspx">
               Return to Products Page
</asp:HyperLink>
</asp:Content>
