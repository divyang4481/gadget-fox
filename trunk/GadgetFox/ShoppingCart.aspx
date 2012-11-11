<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="GadgetFox.ShoppingCart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Label ID="returnLabel" runat="server" ForeColor="#CC0000"></asp:Label>
    </div>
    <div>
        <p style="margin: 5px; font-style: normal; font-size: large; top: auto;">
            Shopping Cart
        </p>
        <asp:GridView ID="GridView1" runat="server" SkinID="Professional" CellPadding="4" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd" OnRowDataBound="GridView1_RowDataBound"/>
        <hr />
        <div>
            <table>
                <tr>
                    <td></td>
                    <td style="padding-right: 20px;"><asp:Label ID="Label1" runat="server">Sub-Total: </asp:Label></td>
                    <td><asp:Label ID="subTotalLB" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td></td>
                    <td style="padding-right: 20px;"><asp:Label ID="Label2" runat="server">Tax: </asp:Label></td>
                    <td><asp:Label ID="taxLB" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td></td>
                    <td style="padding-right: 20px;"><asp:Label ID="Label3" runat="server" Font-Bold="true">Total: </asp:Label></td>
                    <td><asp:Label ID="totalLB" runat="server" Font-Bold="true"></asp:Label></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="checkoutBtn" runat="server" Text="Checkout" Width="75px" OnClick="checkoutBtn_Clicked" style="margin-top: 20px"/>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
            </table>            
        </div>
    </div>
</asp:Content>
