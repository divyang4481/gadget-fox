<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="GadgetFox.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlCustomerHome" runat="server">

        <p>
            <asp:LinkButton ID="editPILinkButton" runat="server" Text="Edit Personal Information" PostBackUrl="~/EditPersonalInformation.aspx"> </asp:LinkButton>
        </p>
        <p>
            <asp:LinkButton ID="editSALinkButton" runat="server" Text="Edit Shipping Address" PostBackUrl="~/EditShippingAddress.aspx"> </asp:LinkButton>
        </p>
        <p>
            <asp:LinkButton ID="editAEDCLinkButton" runat="server" Text="Edit Default Card" PostBackUrl="~/AddEditDefaultCard.aspx"> </asp:LinkButton>
        </p>
        <p>
            <asp:LinkButton ID="productsPurchaseLinkButton" runat="server" PostBackUrl="~/ProductsPurchase.aspx">Products Purchase</asp:LinkButton>
        </p>
        <p>
            <asp:LinkButton ID="addToCartLinButton" runat="server" PostBackUrl="~/AddToCart.aspx">Add To Cart</asp:LinkButton>
        </p>
    </asp:Panel>
    <asp:Panel ID="pnlInventoryManager" runat="server">
        <asp:LinkButton ID="btnMonitorInventory" runat="server" PostBackUrl="~/MonitorInventory.aspx">Monitor Inventory</asp:LinkButton> <br />
        <asp:LinkButton ID="btnViewInventory" runat="server" PostBackUrl="~/ViewInventory.aspx">View Inventory</asp:LinkButton>
    </asp:Panel>
</asp:Content>
