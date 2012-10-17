<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/GadgetSite2.Master" CodeBehind="SalesDashboard.aspx.cs" Inherits="GadgetFox.SalesDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>
&nbsp;<br />
        <asp:LinkButton ID="addProductLinkButton" runat="server" Text="Add Product Information" PostBackUrl="~/ManageProductInformation.aspx"> </asp:LinkButton>
    </p>
    <p>
        <asp:LinkButton ID="editProductLinkButton" runat="server" Text="Edit Product Information" PostBackUrl="~/EditShippingAddress.aspx"> </asp:LinkButton>
    </p>
    <p>
        <asp:LinkButton ID="removeProductLinkButton" runat="server" Text="Remove Product" PostBackUrl="~/EditDefaultCard.aspx"> </asp:LinkButton>
    </p>
    <p>
    </p>
    </asp:Content>
