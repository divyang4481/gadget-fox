<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.Master" AutoEventWireup="true" CodeBehind="Invoice.aspx.cs" Inherits="GadgetFox.Invoice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="height: 0px; margin-bottom: 20px; font-style: normal; font-size: large; top: auto;">
        Invoice
    </p>
    <p style="height: 0px; margin-bottom: 50px; font-style: normal; font-size: small; top: auto;">
        Your invoice is below. Print a copy of it to keep for future reference.
    </p>
    <asp:Panel ID="invoicePanel" runat="server">
        <fieldset style="margin-bottom: 20px;">
            <legend style="font-weight: bold;">Order</legend>
            <asp:Label ID="orderIdLB" runat="server" Text="No info available"></asp:Label>
            <asp:Label ID="purchaseDateLB" runat="server" Text=""></asp:Label>
        </fieldset>
        <fieldset style="margin-bottom: 20px;">
            <legend style="font-weight: bold;">Payment</legend>
            <asp:Label ID="paymentLB" runat="server" Text="No info available"></asp:Label>
        </fieldset>
        <fieldset style="margin-bottom: 20px;">
            <legend style="font-weight: bold;">Ship To</legend>
            <asp:Label ID="shipTypeLB" runat="server" Text=""></asp:Label>
            <asp:Label ID="shipLB" runat="server" Text="No info available"></asp:Label>
        </fieldset>
        <fieldset style="margin-bottom: 20px;">
            <legend style="font-weight: bold;">Products</legend>
            <asp:GridView ID="GridView1" runat="server" Width="800px" SkinID="Professional" CellPadding="4" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd" OnRowDataBound="GridView1_RowDataBound"/>
        </fieldset>
        <fieldset>
            <legend style="font-weight: bold;">Total</legend>
            <table style="font-size: 13px;">
            <tr>
                <td></td>
                <td style="padding-right: 20px;"><asp:Label ID="Label6" runat="server">Sub-Total: </asp:Label></td>
                <td><asp:Label ID="subTotalLB" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td></td>
                <td style="padding-right: 20px;"><asp:Label ID="Label7" runat="server">Tax: </asp:Label></td>
                <td><asp:Label ID="taxLB" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td></td>
                <td style="padding-right: 20px;"><asp:Label ID="Label13" runat="server">Shipping: </asp:Label></td>
                <td><asp:Label ID="shipTotalLB" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td></td>
                <td style="padding-right: 20px;"><asp:Label ID="Label8" runat="server" Font-Bold="true">Total: </asp:Label></td>
                <td><asp:Label ID="totalLB" runat="server" Font-Bold="true"></asp:Label></td>
            </tr>
        </table>        
        </fieldset>
    </asp:Panel>
</asp:Content>
