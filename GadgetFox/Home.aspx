<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="GadgetFox.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlCustomerHome" runat="server">

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
            <asp:LinkButton ID="purchaseProductsLB" runat="server" PostBackUrl="~/PurchaseProducts.aspx">Purchase Products</asp:LinkButton>
        </p>
    </asp:Panel>
    <asp:Panel ID="pnlInventoryManager" runat="server">
        <asp:LinkButton ID="btnMonitorInventory" runat="server" PostBackUrl="~/MonitorInventory.aspx">Monitor Inventory</asp:LinkButton> <br />
        <asp:LinkButton ID="btnViewInventory" runat="server" PostBackUrl="~/ViewInventory.aspx">View Inventory</asp:LinkButton>
    </asp:Panel>
</asp:Content>
