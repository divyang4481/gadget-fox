<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="Wishlist.aspx.cs" Inherits="GadgetFox.Wishlist" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Label ID="returnLabel" runat="server" ForeColor="#CC0000"></asp:Label>
    </div>
    <div>
        <p style="margin: 5px; font-style: normal; font-size: large; top: auto;">
            Wish List
        </p>
        <asp:GridView ID="GridView1" runat="server" SkinID="Professional" CellPadding="4" HeaderStyle-BackColor="#444444" HeaderStyle-ForeColor="White" AlternatingRowStyle-BackColor="#dddddd" OnRowDataBound="GridView1_RowDataBound"/>
        <hr />
        <div>
            <asp:Button ID="shopBtn" runat="server" Text="Continue shopping" Width="150px" OnClick="shopBtn_Click" style="margin-top: 20px" />         
        </div>
    </div>
</asp:Content>
