<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="MyAccount.aspx.cs" Inherits="GadgetFox.MyAccount" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="height: 0px; margin-top: 0px; font-style: normal; font-size: large; top: auto;">
        My Account
    </p>

    <div style="display: block; margin-bottom: 10px;"><asp:Label ID="returnLabel" runat="server" ForeColor="#CC0000"></asp:Label></div>
    <div style="display: block;">
        <asp:Panel ID="Panel1" runat="server" BorderColor="#999999" Height="390px" Width="800px" Direction="LeftToRight" HorizontalAlign="Left">
            <fieldset>
                <legend style="font-weight: bold;">Personal info</legend>
                <table>
                    <tr>
                        <td>Name:</td>
                        <td><asp:Label ID="nameLB" runat="server" Text=""></asp:Label></td>
                    </tr>
                        <td>Birthday:</td>
                        <td><asp:Label ID="birthdayLB" runat="server" Text=""></asp:Label></td>
                    <tr>
                        <td><asp:LinkButton ID="editPersonalInfoLK" runat="server" ForeColor="Blue" Text="Edit info" Width="75px" PostBackUrl="~/EditPersonalInformation.aspx" style="margin-top: 20px"/></td>
                        <td></td>
                    </tr>
                </table>
            </fieldset>
            <fieldset>
                <legend style="font-weight: bold;">Default credit card</legend>
                <table>
                    <tr>
                        <td>Credit card:</td>
                        <td><asp:Label ID="ccLB" runat="server" Text=""></asp:Label></td>
                    </tr>
                        <td>Expiration date:</td>
                        <td><asp:Label ID="ccExpDateLB" runat="server" Text=""></asp:Label></td>
                    <tr>
                        <td><asp:LinkButton ID="editCard" runat="server" ForeColor="Blue" Text="Edit card" Width="75px" PostBackUrl="~/EditDefaultCard.aspx" style="margin-top: 20px"/></td>
                        <td></td>
                    </tr>
                </table>
            </fieldset>
            <fieldset>
                <legend style="font-weight: bold;">Default address</legend>
                <table>
                    <tr>
                        <td style="vertical-align: top;">Address:</td>
                        <td><asp:Label ID="addressLB" runat="server" TextMode="MultiLine"></asp:Label></td>
                    </tr>
                    <tr>
                        <td><asp:LinkButton ID="editAddrLK" runat="server" ForeColor="Blue" Text="Edit address" Width="75px" PostBackUrl="~/EditShippingAddress.aspx" style="margin-top: 20px"/></td>
                        <td></td>
                    </tr>
                </table>
            </fieldset>
            <fieldset>
                <legend style="font-weight: bold;">Orders</legend>
                <asp:GridView ID="GridView1" runat="server" Width="800px" SkinID="Professional" CellPadding="4" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd" OnRowDataBound="GridView1_RowDataBound"/>      
            </fieldset>
        </asp:Panel>
        </div>
    </asp:Content>

