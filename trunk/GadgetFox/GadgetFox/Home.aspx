<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="GadgetFox.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlCustomerHome" runat="server">
        <!-- CSS style should be in stylesheet (eventually) -->
        <table>
            <tr>
                <td style="padding: 30px 0px;">
                    <a href="SearchResults.aspx?"><label style="display: inline-block; font-size: 18px; font-weight: bold; text-align: center; color: #383838; column-span: 3; vertical-align: middle;">Promotions & Deals</label><image src="images/sale_tag.png" style="display: inline-block; width: 20%; vertical-align: middle;"/></a>
                </td>
            </tr>
            <tr>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Cameras & Camcorders</label><image src="images/categories/cameras.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Computers</label><image src="images/categories/computers.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Hardware</label><image src="images/categories/hardware.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
            </tr>
            <tr>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Software</label><image src="images/categories/software.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Media</label><image src="images/categories/media.png" style="display: block; width: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Games</label><image src="images/categories/games.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
            </tr>
            <tr>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Accessories</label><image src="images/categories/accessories.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;"></td>
                <td style="display: inline-block; height: 250px; width: 250px;"></td>
            </tr>
        </table>
    </asp:Panel>

     <!-- The followig elements should be removed -->
    <asp:Panel Visible="false" runat="server">
        <asp:LinkButton ID="productsPurchaseLinkButton" runat="server" PostBackUrl="~/ProductsPurchase.aspx">Products Purchase</asp:LinkButton>
        <asp:LinkButton ID="addToCartLinButton" runat="server" PostBackUrl="~/AddToCart.aspx">Add To Cart</asp:LinkButton>
    </asp:Panel>
    <asp:Panel ID="pnlInventoryManager" runat="server">
        <asp:LinkButton ID="btnMonitorInventory" runat="server" PostBackUrl="~/MonitorInventory.aspx">Monitor Inventory</asp:LinkButton> <br />
        <asp:LinkButton ID="btnViewInventory" runat="server" PostBackUrl="~/ViewInventory.aspx">View Inventory</asp:LinkButton>
    </asp:Panel>
</asp:Content>
