<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="AddEditDefaultCard.aspx.cs" Inherits="GadgetFox.AddEditDefaultCard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="height: 0px; margin-top: 0px; font-style: normal; font-size: x-large; top: auto;">
        Add / Edit Default Card
    </p>
    <asp:Label ID="returnLabel" runat="server" ForeColor="#CC0000"></asp:Label>
    <asp:Panel ID="Panel1" runat="server" BorderColor="#999999" Height="390px" style="margin-left: 250px" Width="479px" Direction="LeftToRight" HorizontalAlign="Left">
        <table>
            <tr>
                <td>Type of Card<asp:Label ID="Label6" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                <td>
                    <asp:DropDownList ID="typeOfcardDL" runat="server" style="margin-left: 0px">
                        <asp:ListItem>--Select--</asp:ListItem>
                        <asp:ListItem>American Express</asp:ListItem>
                        <asp:ListItem>Discover</asp:ListItem>
                        <asp:ListItem>Master Card</asp:ListItem>
                        <asp:ListItem>Visa</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Card Number<asp:Label ID="Label7" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                <td><asp:TextBox ID="cardNumberTB" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Expiration Date<asp:Label ID="Label8" runat="server" ForeColor="#CC0000" Text="*"></asp:Label></td>
                <td>
                    <asp:TextBox ID="expMonthTB" runat="server" style="margin-left: 0px" Height="16px" Width="26px"></asp:TextBox>
                    <asp:TextBox ID="expYearTB" runat="server" Height="20px" Width="57px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>CVV Number <asp:Label ID="Label9" runat="server" ForeColor="Red" Text="*"></asp:Label></td>
                <td><asp:TextBox ID="cvvNumberTB" runat="server" Height="18px" Width="67px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="saveButton" runat="server" Text="Submit" Width="75px" OnClick="saveButton_Clicked" style="margin-top: 20px"/>
                </td>
                <td>
                    <asp:Button ID="cancelButton" runat="server" Text="Cancel" Width="75px" PostBackUrl="~/Home.aspx" style="margin-top: 20px"/>
                </td>
            </tr>
        </table>
        <br />
    </asp:Panel>
    </asp:Content>

