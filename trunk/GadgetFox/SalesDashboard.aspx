<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/GadgetSite2.Master" CodeBehind="SalesDashboard.aspx.cs" Inherits="GadgetFox.SalesDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="height: 0px; margin-bottom: 50px; font-style: normal; font-size: large; top: auto;">
        Sales & Marketing
    </p>
    <asp:LinkButton style="display: block; color: blue;" ID="addProductLinkButton" runat="server" Text="Add Product Information" PostBackUrl="~/ManageProductInformation.aspx"> </asp:LinkButton>
    <asp:LinkButton style="display: block; color: blue;" ID="editProductLinkButton" runat="server" Text="Edit Product Information" PostBackUrl="~/UpdateProductInformation.aspx"> </asp:LinkButton>
</asp:Content>
