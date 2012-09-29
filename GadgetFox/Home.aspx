<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="GadgetFox.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>
&nbsp;<br />
        <asp:LinkButton ID="editPILinkButton" runat="server" Text="Edit Personal Information" PostBackUrl="~/EditPersonalInformation.aspx"> </asp:LinkButton>
    </p>
    <p>
        <asp:LinkButton ID="editSALinkButton" runat="server" Text="Edit Shipping Address" PostBackUrl="~/EditShippingAddress.aspx"> </asp:LinkButton>
    </p>
    <p>
        <asp:LinkButton ID="editDCLinkButton" runat="server" Text="Edit Default Card" PostBackUrl="~/EditDefaultCard.aspx"> </asp:LinkButton>
    </p>
    <p>
    </p>
    </asp:Content>
