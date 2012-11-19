<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="GadgetFox.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlCustomerHome" runat="server">
        <!-- CSS style should be in stylesheet (eventually) -->
        <table>
            <tr>
                <td style="padding: 30px 0px;">
                    <a href="Sales.aspx"><label style="display: inline-block; font-size: 18px; font-weight: bold; text-align: center; color: #383838; column-span: 3; vertical-align: middle;">Promotions & Deals</label><image src="images/sale_tag.png" style="display: inline-block; width: 20%; vertical-align: middle;"/></a>
                </td>
            </tr>
            <tr>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?category=Camcorders"><label style="display: block; font-size: 16px; font-weight: bold; text-align: center; color: #383838;">Cameras & Camcorders</label><image src="images/categories/cameras.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?category=Computers"><label style="display: block; font-size: 16px; font-weight: bold; text-align: center; color: #383838;">Computers</label><image src="images/categories/computers.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?category=Hardware"><label style="display: block; font-size: 16px; font-weight: bold; text-align: center; color: #383838;">Hardware</label><image src="images/categories/hardware.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
            </tr>
            <tr>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?category=Software"><label style="display: block; font-size: 16px; font-weight: bold; text-align: center; color: #383838;">Software</label><image src="images/categories/software.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?category=Media"><label style="display: block; font-size: 16px; font-weight: bold; text-align: center; color: #383838;">Media</label><image src="images/categories/media.png" style="display: block; width: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?category=Games"><label style="display: block; font-size: 16px; font-weight: bold; text-align: center; color: #383838;">Games</label><image src="images/categories/games.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
            </tr>
            <tr>
                <td style="display: inline-block; height: 250px; width: 250px;">
                    <a href="SearchResults.aspx?category=Accessories"><label style="display: block; font-size: 16px; font-weight: bold; text-align: center; color: #383838;">Accessories</label><image src="images/categories/accessories.png" style="display: block; height: 80%; margin: auto;"/></a>
                </td>
                <td style="display: inline-block; height: 250px; width: 250px;"></td>
                <td style="display: inline-block; height: 250px; width: 250px;"></td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
