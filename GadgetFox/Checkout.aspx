<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="GadgetFox.Checkout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:GridView ID="Basket" runat="server" AutoGenerateColumns="False" 
    GridLines="None" EnableViewState="False" ShowFooter="True" 
    DataKeyNames="ProductID" OnRowCreated="Basket_RowCreated">
    <Columns>
      <asp:TemplateField HeaderText="Remove">
        <ItemTemplate>
          <asp:CheckBox ID="RemovedProducts" runat="server" />
        </ItemTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Product" SortExpression="ProductName">
        <ItemTemplate>
          <asp:Label ID="ProductName" runat="server" Text='<%# Eval("ProductName") %>' />
        </ItemTemplate>
        <FooterTemplate>
          <strong>
            Total Price:
          </strong>
        </FooterTemplate>
      </asp:TemplateField>
      <asp:TemplateField HeaderText="Price" SortExpression="UnitPrice">
        <ItemTemplate>
          <asp:Label ID="Price" runat="server" Text='<%# Eval("Price", "{0:c}") %>' />
        </ItemTemplate>
        <FooterTemplate>
          <strong>
            <asp:Literal ID="TotalPrice" runat="server" />
          </strong>
        </FooterTemplate>
      </asp:TemplateField>
    </Columns>
  </asp:GridView>
  <asp:Button ID="RemoveProduct" runat="server" 
    Text="Remove" OnClick="RemoveProduct_Click" />
  &nbsp;&nbsp;&nbsp;&nbsp;
  <asp:Button ID="ConfirmPurchase" runat="server" Text="Confirm Purchase" />
  <asp:SqlDataSource ID="BasketData" runat="server" 
    ConnectionString="<%$ ConnectionStrings:NorthwindConnectionString %>">
  </asp:SqlDataSource>
</asp:Content>
